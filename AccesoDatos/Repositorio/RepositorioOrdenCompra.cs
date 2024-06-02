using AccesoDatos.Data;
using AccesoDatos.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using ModelosApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio
{
    public class RepositorioOrdenCompra : Repositorio<OrdenCompra>, IRepositorioOrdenCompra
    {
        private readonly LibreriaContexto contexto;

        public RepositorioOrdenCompra(LibreriaContexto contexto) : base(contexto)
        {
            this.contexto = contexto;
        }

        public async Task EstadoPago(int id, string sessionID, string? PaymentIntentID = null)
        {
            var orden = await contexto.OrdenesCompra.FirstOrDefaultAsync(o => o.ID == id);

            orden.FechaPago = DateTime.Now;
            orden.SessionID = sessionID;

            if(!string.IsNullOrWhiteSpace(PaymentIntentID))
            {
                orden.PaymentIntentID = PaymentIntentID;
            }
        }

        public void Update(OrdenCompra ordenCompra)
        {
            contexto.OrdenesCompra.Update(ordenCompra);
        }

        public async Task UpdateEstado(int id, string estadoOrden, string? estadoPago = null)
        {
            var orden = await contexto.OrdenesCompra.FirstOrDefaultAsync(o => o.ID == id);

            if (orden != null)
            {
                orden.EstadoOrden = estadoOrden;
            }
            if(estadoPago != null)
            {
                orden.EstadoPago = estadoPago;
            }
        }
    }
}
