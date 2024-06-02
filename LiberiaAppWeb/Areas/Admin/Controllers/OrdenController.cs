using AccesoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelosApp.Models;
using ModelosApp.Models.ViewModels;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using Utilities;

namespace LiberiaAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrdenController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public OrdenController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? estado)
        {
            IEnumerable<OrdenCompra> ordenes;

            if (User.IsInRole("Admin") || User.IsInRole("Empleado"))
            {
                ordenes = await unitOfWork.OrdenCompra.GetAll(propiedadesIncluidas: "UsuarioAplicacion");
                ordenes = await OrdenarPorEstado(ordenes, estado, false);
            }
            else
            {
                var identity = (ClaimsIdentity)User.Identity;
                var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

                ordenes = await unitOfWork.OrdenCompra.GetAll(c => c.UsuarioAplicacionID == claim.Value, propiedadesIncluidas: "UsuarioAplicacion");
                ordenes = await OrdenarPorEstado(ordenes, estado, true, claim);
            }

            return View(ordenes);
        }

        public async Task<IActionResult> DetalleOrden(int id)
        {
            OrdenVM ordenVM = new()
            {
                OrdenCompra = await unitOfWork.OrdenCompra.Get(c => c.ID == id, propiedadesIncluidas: "UsuarioAplicacion"),
                OrdenDetalle = await unitOfWork.OrdenDetalle.GetAll(c => c.OrdenCompraID == id, propiedadesIncluidas: "Producto")
            };


            return View(ordenVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Empleado")]
        public async Task<IActionResult> DetalleOrden(OrdenVM ordenVM)
        {
            OrdenCompra orden = await unitOfWork.OrdenCompra.Get(c => c.ID == ordenVM.OrdenCompra.ID);

            orden.Nombre = ordenVM.OrdenCompra.Nombre;
            orden.Direccion = ordenVM.OrdenCompra.Direccion;
            orden.Ciudad = ordenVM.OrdenCompra.Ciudad;
            orden.Provincia = ordenVM.OrdenCompra.Provincia;
            orden.CodigoPostal = ordenVM.OrdenCompra.CodigoPostal;

            if (ordenVM.OrdenCompra.Transportista != null)
            {
                orden.Transportista = ordenVM.OrdenCompra.Transportista;
            }

            if (ordenVM.OrdenCompra.NumeroSeguimiento != null)
            {
                orden.NumeroSeguimiento = ordenVM.OrdenCompra.NumeroSeguimiento;
            }

            unitOfWork.OrdenCompra.Update(orden);
            await unitOfWork.Save();

            return RedirectToAction("DetalleOrden", "Orden", new { id = ordenVM.OrdenCompra.ID });
        }

        [Authorize(Roles = "Admin,Empleado")]
        public async Task<IActionResult> EnProceso(OrdenVM ordenVM)
        {
            await unitOfWork.OrdenCompra.UpdateEstado(ordenVM.OrdenCompra.ID, EstadoOrden.EstadoProcesando);
            await unitOfWork.Save();

            return RedirectToAction("DetalleOrden", "Orden", new { id = ordenVM.OrdenCompra.ID });
        }

        [Authorize(Roles = "Admin,Empleado")]
        public async Task<IActionResult> Entregado(OrdenVM ordenVM)
        {
            if (!string.IsNullOrWhiteSpace(ordenVM.OrdenCompra.Transportista) && !string.IsNullOrWhiteSpace(ordenVM.OrdenCompra.NumeroSeguimiento))
            {
                OrdenCompra orden = await unitOfWork.OrdenCompra.Get(c => c.ID == ordenVM.OrdenCompra.ID);

                await unitOfWork.OrdenCompra.UpdateEstado(ordenVM.OrdenCompra.ID, EstadoOrden.EstadoEnviado);
                orden.Transportista = ordenVM.OrdenCompra.Transportista;
                orden.NumeroSeguimiento = ordenVM.OrdenCompra.NumeroSeguimiento;
                orden.EstadoOrden = EstadoOrden.EstadoEnviado;
                orden.FechaDespacho = DateTime.Now;

                unitOfWork.OrdenCompra.Update(orden);
                await unitOfWork.Save();
            }
            else
            {
                TempData["ValoresNull"] = "Los valores de Transportista y Número de seguimiento no deben estar vacíos";
            }

            return RedirectToAction("DetalleOrden", "Orden", new { id = ordenVM.OrdenCompra.ID });
        }

        [Authorize(Roles = "Admin,Empleado")]
        public async Task<IActionResult> CancelarOrden(OrdenVM ordenVM)
        {
            OrdenCompra orden = await unitOfWork.OrdenCompra.Get(c => c.ID == ordenVM.OrdenCompra.ID);

            if (orden.EstadoPago == EstadoPago.EstadoAprobado)
            {
                var devolucionOpciones = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orden.PaymentIntentID
                };

                var servicio = new RefundService();

                Refund devolucion = await servicio.CreateAsync(devolucionOpciones);

                await unitOfWork.OrdenCompra.UpdateEstado(orden.ID, EstadoOrden.EstadoCancelado, EstadoOrden.EstadoReembolsado);
            }
            else
            {
                await unitOfWork.OrdenCompra.UpdateEstado(orden.ID, EstadoOrden.EstadoCancelado, EstadoOrden.EstadoCancelado);
            }

            await unitOfWork.Save();

            return RedirectToAction("DetalleOrden", "Orden", new { id = ordenVM.OrdenCompra.ID });
        }

        public async Task<IActionResult> PagarOrden(OrdenVM ordenVM)
        {
            OrdenCompra ordenCompra = await unitOfWork.OrdenCompra.Get(c => c.ID == ordenVM.OrdenCompra.ID, propiedadesIncluidas: "UsuarioAplicacion");
            var ordenDetalle = await unitOfWork.OrdenDetalle.GetAll(c => c.OrdenCompraID == ordenVM.OrdenCompra.ID, propiedadesIncluidas: "Producto");

            var dominio = HttpContext.Request.BaseUrl();

            var opciones = new SessionCreateOptions()
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = dominio + $"Usuario/Carrito/OrdenRealizada?id={ordenVM.OrdenCompra.ID}",
                CancelUrl = dominio + "Usuario/Carrito/Index"
            };

            foreach (var item in ordenDetalle)
            {
                var lineItemOptions = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Producto.Precio * 100),
                        Currency = "ARS",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Producto.Titulo
                        },
                    },
                    Quantity = item.Cantidad
                };
                opciones.LineItems.Add(lineItemOptions);
            }

            var servicio = new SessionService();
            Session session = servicio.Create(opciones);
            await unitOfWork.OrdenCompra.EstadoPago(ordenVM.OrdenCompra.ID, session.Id);
            await unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);

            await unitOfWork.Save();

            return new StatusCodeResult(303);
        }

        public async Task<IEnumerable<OrdenCompra>> OrdenarPorEstado(IEnumerable<OrdenCompra> ordenes, string? estado, bool esCliente, Claim? claim = null)
        {
            if(esCliente)
            {
                switch (estado)
                {
                    case EstadoPago.EstadoPendiente:
                        ordenes = await unitOfWork.OrdenCompra.GetAll(c => c.EstadoPago == EstadoPago.EstadoPendiente && c.UsuarioAplicacionID == claim.Value, propiedadesIncluidas: "UsuarioAplicacion");
                        break;
                    case EstadoPago.EstadoAprobado:
                        ordenes = await unitOfWork.OrdenCompra.GetAll(c => c.EstadoPago == EstadoPago.EstadoAprobado && c.UsuarioAplicacionID == claim.Value, propiedadesIncluidas: "UsuarioAplicacion");
                        break;
                    case EstadoOrden.EstadoProcesando:
                        ordenes = await unitOfWork.OrdenCompra.GetAll(c => c.EstadoOrden == EstadoOrden.EstadoProcesando && c.UsuarioAplicacionID == claim.Value, propiedadesIncluidas: "UsuarioAplicacion");
                        break;
                    case EstadoOrden.EstadoEnviado:
                        ordenes = await unitOfWork.OrdenCompra.GetAll(c => c.EstadoOrden == EstadoOrden.EstadoEnviado && c.UsuarioAplicacionID == claim.Value, propiedadesIncluidas: "UsuarioAplicacion");
                        break;
                }
            }
            else
            {
                switch (estado)
                {
                    case EstadoPago.EstadoPendiente:
                        ordenes = await unitOfWork.OrdenCompra.GetAll(c => c.EstadoPago == EstadoPago.EstadoPendiente, propiedadesIncluidas: "UsuarioAplicacion");
                        break;
                    case EstadoPago.EstadoAprobado:
                        ordenes = await unitOfWork.OrdenCompra.GetAll(c => c.EstadoPago == EstadoPago.EstadoAprobado, propiedadesIncluidas: "UsuarioAplicacion");
                        break;
                    case EstadoOrden.EstadoProcesando:
                        ordenes = await unitOfWork.OrdenCompra.GetAll(c => c.EstadoOrden == EstadoOrden.EstadoProcesando, propiedadesIncluidas: "UsuarioAplicacion");
                        break;
                    case EstadoOrden.EstadoEnviado:
                        ordenes = await unitOfWork.OrdenCompra.GetAll(c => c.EstadoOrden == EstadoOrden.EstadoEnviado, propiedadesIncluidas: "UsuarioAplicacion");
                        break;
                }
            }
            return ordenes;
        }
    }
}
