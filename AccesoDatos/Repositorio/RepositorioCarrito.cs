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
    public class RepositorioCarrito : Repositorio<Carrito>, IRepositorioCarrito
    {
        private readonly LibreriaContexto contexto;

        public RepositorioCarrito(LibreriaContexto contexto) : base(contexto)
        {
            this.contexto = contexto;
        }

        public void IncrementarCantidad(Carrito itemCarrito, int cantidad)
        {
            itemCarrito.Cantidad += cantidad;
        }

        public void Update(Carrito carrito)
        {
            contexto.Carritos.Update(carrito);
        }
    }
}
