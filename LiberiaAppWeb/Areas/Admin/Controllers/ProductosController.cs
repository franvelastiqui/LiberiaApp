using AccesoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelosApp.Models;
using Utilities;

namespace LiberiaAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Rol_Admin)]
    public class ProductosController : Controller
    {
        public readonly IUnitOfWork unidadTrabajo;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductosController(IUnitOfWork unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            this.unidadTrabajo = unidadTrabajo;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string orden, string busqueda, string busquedaActual, int? indicePagina)
        {
            ViewData["OrdenActual"] = orden;
            ViewData["OrdenTitulo"] = string.IsNullOrWhiteSpace(orden) ? "titulo_desc" : "";
            ViewData["OrdenISBN"] = orden=="isbn" ? "isbn_desc" : "isbn";
            ViewData["OrdenPrecio"] = orden == "precio" ? "precio_desc" : "precio";
            ViewData["OrdenAutor"] = orden == "autor" ? "autor_desc" : "autor";
            ViewData["OrdenCategoria"] = orden == "categoria" ? "categoria_desc" : "categoria";

            if(busqueda != null)
            {
                indicePagina = 1;
            }
            else
            {
                busqueda = busquedaActual;
            }

            ViewData["BusquedaActual"] = busqueda;

            IQueryable<Producto> productos = unidadTrabajo.Producto.GetAllQuery(propiedadesIncluidas:"Categoria");

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                productos = productos.Where(c => c.Titulo.Contains(busqueda) || c.Autor.Contains(busqueda));
            }

            productos = ElegirOrden(productos, orden);

            int registros = 10;
            return View(await ListaConPaginas<Producto>.CrearAsync(productos, indicePagina ?? 1, registros));
        }

        public async Task<IActionResult> Actualizar(int? id)
        {
            Producto producto = new Producto();

            LlenarListaCategorias();

            if (id != null && id != 0)
            {
                producto = await unidadTrabajo.Producto.Get(p => p.ID == id);

                if (producto == null)
                {
                    return NotFound();
                }

                LlenarListaCategorias(producto.ID);
            }

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar([Bind("ID,Titulo, Descripcion, ISBN, Autor, PrecioLista, Precio, Precio20, Precio50, CategoriaID, RutaImagen")] Producto producto, IFormFile? imagen)
        {
            if (producto == null)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if(imagen != null)
                    {
                        CargarImagen(producto, imagen);
                    }

                    if (producto.ID == 0)
                    {
                        unidadTrabajo.Producto.Add(producto);
                    }
                    else
                    {
                        unidadTrabajo.Producto.Update(producto);
                    }

                    await unidadTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Se ha producido un error al intentar guardar los cambios. Intente de nuevo, y si el problema persiste, contacte a al administrador o proveedor de su sistema.");
            }

            LlenarListaCategorias(producto.ID);
            return View(producto);
        }


        public async Task<IActionResult> Eliminar(int? id, bool? guardarCambiosError)
        {
            if (id == null)
            {
                return NotFound();
            }

            Producto? producto = await unidadTrabajo.Producto.GetAllQuery().FirstOrDefaultAsync(p => p.ID == id);

            if (producto == null)
            {
                return NotFound();
            }

            if (guardarCambiosError.GetValueOrDefault())
            {
                ViewData["MensajeError"] = "Se ha producido un error al intentar eliminar el registro. Intente de nuevo, y si el problema persiste, contacte a al administrador o proveedor de su sistema.";
            }

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            Producto? producto = await unidadTrabajo.Producto.Get(p => p.ID == id);

            if (producto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                unidadTrabajo.Producto.Remove(producto);
                await unidadTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Eliminar), new { id, guardarCambiosError = true });
            }
        }

        private IQueryable<Producto> ElegirOrden(IQueryable<Producto> productos, string orden)
        {
            switch (orden)
            {
                case "titulo_desc":
                    productos = productos.OrderByDescending(p => p.Titulo);
                    break;
                case "isbn":
                    productos = productos.OrderBy(p => p.ISBN);
                    break;
                case "isbn_desc":
                    productos = productos.OrderByDescending(p => p.ISBN);
                    break;
                case "precio":
                    productos = productos.OrderBy(p => p.Precio);
                    break;
                case "precio_desc":
                    productos = productos.OrderByDescending(p => p.Precio);
                    break;
                case "autor":
                    productos = productos.OrderBy(p => p.Autor);
                    break;
                case "autor_desc":
                    productos = productos.OrderByDescending(p => p.Autor);
                    break;
                case "categoria":
                    productos = productos.OrderBy(p => p.Categoria!.Nombre);
                    break;
                case "categoria_desc":
                    productos = productos.OrderByDescending(p => p.Categoria!.Nombre);
                    break;
                default:
                    productos = productos.OrderBy(p => p.Titulo);
                    break;
            }

            return productos;
        }

        public void LlenarListaCategorias(object categoriaSeleccionada = null)
        {
            IQueryable<Categoria> lista = unidadTrabajo.Categoria.GetAllQuery();

            ViewBag.ListaCategorias = new SelectList(lista, "ID", "Nombre", categoriaSeleccionada);
        }

        public void CargarImagen(Producto producto, IFormFile imagen)
        {
            string wwwRuta = webHostEnvironment.WebRootPath;

            string archivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            string rutaProducto = Path.Combine(wwwRuta, @"imagenes\producto");

            if(!string.IsNullOrWhiteSpace(producto.RutaImagen))
            {
                var imagenVieja = Path.Combine(wwwRuta, producto.RutaImagen.TrimStart('\\'));

                if(System.IO.File.Exists(imagenVieja))
                {
                    System.IO.File.Delete(imagenVieja);
                }
            }

            using (var fileStream = new FileStream(Path.Combine(rutaProducto, archivo), FileMode.Create))
            {
                imagen.CopyTo(fileStream);
            }

            producto.RutaImagen = @"\imagenes\producto\" + archivo;
        }
    }
}
