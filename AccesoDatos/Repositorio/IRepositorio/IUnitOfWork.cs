using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio.IRepositorio
{
    public interface IUnitOfWork
    {
        IRepositorioCategoria Categoria { get; }
        IRepositorioProducto Producto { get; }
        IRepositorioCarrito Carrito { get; }
        IRepositorioOrdenCompra OrdenCompra { get; }
        IRepositorioOrdenDetalle OrdenDetalle { get; }
        IRepositorioUsuarioAplicacion UsuarioAplicacion { get; }

        Task Save();
    }
}
