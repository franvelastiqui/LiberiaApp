using AccesoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelosApp.Models;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace LiberiaAppWeb.Areas.Usuario.Controllers
{
    [Area("Usuario")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Producto> listaProductos = await unitOfWork.Producto.GetAll(propiedadesIncluidas:"Categoria");
            return View(listaProductos);
        }

        public async Task<IActionResult> Detalles(int? id)
        {
            Carrito carrito = new Carrito()
            {
                Producto = await unitOfWork.Producto.Get(p => p.ID == id, propiedadesIncluidas: "Categoria"),
                Cantidad = 1,
                ProductoID = (int)id,
            };

            return View(carrito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Detalles([Bind("Cantidad, ProductoID")]Carrito carrito)
        {
            if(ModelState.IsValid)
            {
                var identity = (ClaimsIdentity)User.Identity;

                var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

                carrito.UsuarioAplicacionID = claim.Value;

                var itemCarrito = await unitOfWork.Carrito.Get(i => i.ProductoID == carrito.ProductoID && i.UsuarioAplicacionID == claim.Value);

                if (itemCarrito == null)
                {
                    unitOfWork.Carrito.Add(carrito);
                    await unitOfWork.Save();

                }
                else
                {
                    unitOfWork.Carrito.IncrementarCantidad(itemCarrito, carrito.Cantidad);
                    await unitOfWork.Save();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Compra()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
