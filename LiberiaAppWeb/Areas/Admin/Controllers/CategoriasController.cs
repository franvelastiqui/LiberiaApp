using AccesoDatos.Data;
using AccesoDatos.Repositorio;
using AccesoDatos.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelosApp.Models;
using Utilities;

namespace LiberiaAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Rol_Admin)]
    public class CategoriasController : Controller
    {
        public readonly IUnitOfWork unidadTrabajo;

        public CategoriasController(IUnitOfWork unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        public async Task<IActionResult> Index(string orden, string busqueda, string busquedaActual,int? indicePagina)
        {
            ViewData["OrdenActual"] = orden;
            ViewData["OrdenNombre"] = string.IsNullOrWhiteSpace(orden) ? "nombre_desc" : "";

            if (busqueda != null)
            {
                indicePagina = 1;
            }
            else
            {
                busqueda = busquedaActual;
            }

            ViewData["BusquedaActual"] = busqueda;

            IQueryable<Categoria> categorias = unidadTrabajo.Categoria.GetAllQuery();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                categorias = categorias.Where(c => c.Nombre.Contains(busqueda));
            }

            switch (orden)
            {
                case "nombre_desc":
                    categorias = categorias.OrderByDescending(p => p.Nombre);
                    break;
                default:
                    categorias = categorias.OrderBy(p => p.Nombre);
                    break;
            }

            int tamanioPagina = 10;
            return View(await ListaConPaginas<Categoria>.CrearAsync(categorias, indicePagina ?? 1, tamanioPagina));
        }

        public async Task<IActionResult> Actualizar(int? id)
        {
            Categoria categoria = new Categoria();

            if (id != null && id != 0)
            {
                categoria = await unidadTrabajo.Categoria.Get(c => c.ID == id);

                if (categoria == null)
                {
                    return NotFound();
                }
            }

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar([Bind("ID, Nombre")] Categoria categoria)
        {
            if (categoria == null)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if(categoria.ID == 0)
                    {
                        unidadTrabajo.Categoria.Add(categoria);
                    }
                    else
                    {
                        unidadTrabajo.Categoria.Update(categoria);
                    }

                    await unidadTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Se ha producido un error al intentar guardar los cambios. Intente de nuevo, y si el problema persiste, contacte a al administrador o proveedor de su sistema.");
            }

            return View(categoria);
        }

        public async Task<IActionResult> Eliminar(int? id, bool? guardarCambiosError)
        {
            if (id == null)
            {
                return NotFound();
            }

            Categoria? categoria = await unidadTrabajo.Categoria.GetAllQuery().FirstOrDefaultAsync(c => c.ID == id);

            if (categoria == null)
            {
                return NotFound();
            }

            if (guardarCambiosError.GetValueOrDefault())
            {
                ViewData["MensajeError"] = "Se ha producido un error al intentar eliminar el registro. Intente de nuevo, y si el problema persiste, contacte a al administrador o proveedor de su sistema.";
            }


            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            Categoria? categoria = await unidadTrabajo.Categoria.Get(c => c.ID == id);

            if (categoria == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                unidadTrabajo.Categoria.Remove(categoria);
                await unidadTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Eliminar), new { id, guardarCambiosError = true });
            }
        }
    }
}
