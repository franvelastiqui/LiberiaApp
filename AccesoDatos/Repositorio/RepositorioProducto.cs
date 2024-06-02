using AccesoDatos.Data;
using AccesoDatos.Repositorio.IRepositorio;
using ModelosApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio
{
    public class RepositorioProducto : Repositorio<Producto>, IRepositorioProducto
    {
        private readonly LibreriaContexto contexto;
        public RepositorioProducto(LibreriaContexto contexto) : base(contexto)
        {
            this.contexto = contexto;
        }

        public void Update(Producto producto)
        {
            var productoAModificar = contexto.Productos.FirstOrDefault(p => p.ID == producto.ID);

            if(productoAModificar != null)
            {
                productoAModificar.Titulo = producto.Titulo;
                productoAModificar.Descripcion = producto.Descripcion;
                productoAModificar.ISBN = producto.ISBN;
                productoAModificar.Autor = producto.Autor;
                productoAModificar.Precio = producto.Precio;
                productoAModificar.CategoriaID = producto.CategoriaID;
                if(producto.RutaImagen != null)
                {
                    productoAModificar.RutaImagen = producto.RutaImagen;
                }
            }
        }
    }
}
