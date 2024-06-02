using AccesoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using ModelosApp.Models;
using ModelosApp.Models.ViewModels;
using Stripe.Checkout;
using Stripe.Issuing;
using System.Security.Claims;
using Utilities;

namespace LiberiaAppWeb.Areas.Usuario.Controllers
{
    [Area("Usuario")]
    [Authorize]
    public class CarritoController : Controller
    {
        private IUnitOfWork unitOfWork;
        private CarritoVM CarritoVM;

        public CarritoController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var claim = GetUsuario();

            CarritoVM = new CarritoVM()
            {
                ListaCarritos = await unitOfWork.Carrito.GetAll(c => c.UsuarioAplicacionID == claim.Value, propiedadesIncluidas: "Producto"),
                OrdenCompra = new OrdenCompra()
            };

            SetTotal(CarritoVM);

            return View(CarritoVM);
        }

        public async Task<IActionResult> Suma()
        {
            var claim = GetUsuario();

            CarritoVM = new CarritoVM()
            {
                ListaCarritos = await unitOfWork.Carrito.GetAll(c => c.UsuarioAplicacionID == claim.Value, propiedadesIncluidas: "Producto"),
                OrdenCompra = new OrdenCompra()
            };
            CarritoVM.OrdenCompra.UsuarioAplicacion = await unitOfWork.UsuarioAplicacion.Get(u => u.Id == claim.Value);
            CarritoVM.OrdenCompra.Nombre = CarritoVM.OrdenCompra.UsuarioAplicacion.Nombre;
            CarritoVM.OrdenCompra.Direccion = CarritoVM.OrdenCompra.UsuarioAplicacion.Direccion;
            CarritoVM.OrdenCompra.Ciudad = CarritoVM.OrdenCompra.UsuarioAplicacion.Ciudad;
            CarritoVM.OrdenCompra.Provincia = CarritoVM.OrdenCompra.UsuarioAplicacion.Provincia;
            CarritoVM.OrdenCompra.CodigoPostal = CarritoVM.OrdenCompra.UsuarioAplicacion.CodigoPostal;

            SetTotal(CarritoVM);

            return View(CarritoVM);
        }

        [HttpPost]
        public async Task<IActionResult> Suma(CarritoVM carrito)
        {
            var claim = GetUsuario();

            carrito.ListaCarritos = await unitOfWork.Carrito.GetAll(c => c.UsuarioAplicacionID == claim.Value, propiedadesIncluidas: "Producto");

            carrito.OrdenCompra.EstadoOrden = EstadoOrden.EstadoPendiente;
            carrito.OrdenCompra.EstadoPago = EstadoPago.EstadoPendiente;
            carrito.OrdenCompra.FechaOrden = DateTime.Now;
            carrito.OrdenCompra.UsuarioAplicacionID = claim.Value;


            SetTotal(carrito);

            unitOfWork.OrdenCompra.Add(carrito.OrdenCompra);
            await unitOfWork.Save();

            foreach (var item in carrito.ListaCarritos)
            {
                OrdenDetalle orden = new OrdenDetalle()
                {
                    ProductoID = item.ProductoID,
                    OrdenCompraID = carrito.OrdenCompra.ID,
                    Cantidad = item.Cantidad,
                    Precio = item.Producto.Precio
                };
                unitOfWork.OrdenDetalle.Add(orden);
                await unitOfWork.Save();
            }

            var dominio = HttpContext.Request.BaseUrl();

            var opciones = new SessionCreateOptions()
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = dominio + $"Usuario/Carrito/OrdenRealizada?id={carrito.OrdenCompra.ID}",
                CancelUrl = dominio + "Usuario/Carrito/Index"
            };

            foreach (var carro in carrito.ListaCarritos)
            {
                var lineItemOptions = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(carro.Producto.Precio * 100),
                        Currency = "ARS",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = carro.Producto.Titulo
                        },
                    },
                    Quantity = carro.Cantidad
                };
                opciones.LineItems.Add(lineItemOptions);
            }

            var servicio = new SessionService();
            Session session = servicio.Create(opciones);
            await unitOfWork.OrdenCompra.EstadoPago(carrito.OrdenCompra.ID, session.Id);
            await unitOfWork.Save();

            unitOfWork.Carrito.RemoveRange(carrito.ListaCarritos);
            await unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

        [HttpGet]
        public async Task<IActionResult> OrdenRealizada(int id)
        {
            var orden = await unitOfWork.OrdenCompra.Get(c => c.ID == id);

            var servicio = new SessionService();
            Session session = servicio.Get(orden.SessionID);

            if(session.PaymentStatus.ToLower() == "paid")
            {
                await unitOfWork.OrdenCompra.EstadoPago(id, session.Id, session.PaymentIntentId);
                await unitOfWork.OrdenCompra.UpdateEstado(id, EstadoOrden.EstadoAprobado, EstadoPago.EstadoAprobado);
                await unitOfWork.Save();
            }

            var carritos = await unitOfWork.Carrito.GetAll(c => c.UsuarioAplicacionID == orden.UsuarioAplicacionID, propiedadesIncluidas: "Producto");
            unitOfWork.Carrito.RemoveRange(carritos);
            await unitOfWork.Save();

            return View(id);
        }

        public async Task<IActionResult> Agregar(int id)
        {
            var carrito = await unitOfWork.Carrito.Get(c => c.ID == id);
            unitOfWork.Carrito.IncrementarCantidad(carrito, 1);

            await unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Quitar(int id)
        {
            var carrito = await unitOfWork.Carrito.Get(c => c.ID == id);
            if(carrito.Cantidad <= 1)
            {
                unitOfWork.Carrito.Remove(carrito);
            }
            {
                unitOfWork.Carrito.IncrementarCantidad(carrito, -1);
            }

            await unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var carrito = await unitOfWork.Carrito.Get(c => c.ID == id);

            unitOfWork.Carrito.Remove(carrito);

            await unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        private Claim GetUsuario()
        {
            var identity = (ClaimsIdentity)User.Identity;

            return identity.FindFirst(ClaimTypes.NameIdentifier);
        }

        private void SetTotal(CarritoVM carrito)
        {
            foreach (var item in carrito.ListaCarritos)
            {
                carrito.OrdenCompra.Total += (item.Producto.Precio * item.Cantidad);
            }
        }
    }
}
