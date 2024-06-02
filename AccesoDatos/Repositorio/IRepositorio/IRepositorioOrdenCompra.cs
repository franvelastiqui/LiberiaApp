using Microsoft.EntityFrameworkCore.Update.Internal;
using ModelosApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorioOrdenCompra : IRepositorio<OrdenCompra>
    {
        void Update(OrdenCompra ordenCompra);
        Task UpdateEstado(int id, string estadoOrden, string? estadoPago = null);
        Task EstadoPago(int id, string sessionID, string? PaymentIntentID = null);

    }
}
