using AccesoDatos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio.IRepositorio
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibreriaContexto contexto;
        public IRepositorioCategoria Categoria { get; private set; }
        public IRepositorioProducto Producto { get; private set; }
        public IRepositorioCarrito Carrito { get; private set; }
        public IRepositorioOrdenCompra OrdenCompra {  get; private set; }
        public IRepositorioOrdenDetalle OrdenDetalle { get; private set; }
        public IRepositorioUsuarioAplicacion UsuarioAplicacion { get; private set; }

        public UnitOfWork(LibreriaContexto contexto)
        {
            this.contexto = contexto;
            Categoria = new RepositorioCategoria(contexto);
            Producto = new RepositorioProducto(contexto);
            Carrito = new RepositorioCarrito(contexto);
            OrdenCompra = new RepositorioOrdenCompra(contexto);
            OrdenDetalle = new RepositorioOrdenDetalle(contexto);
            UsuarioAplicacion = new RepositorioUsuarioAplicacion(contexto);
        }

        public async Task Save()
        {
            await contexto.SaveChangesAsync();
        }
    }
}
