using ModelosApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorioCarrito : IRepositorio<Carrito>
    {
        void IncrementarCantidad(Carrito itemCarrito, int cantidad);
        void Update(Carrito carrito);
    }
}
