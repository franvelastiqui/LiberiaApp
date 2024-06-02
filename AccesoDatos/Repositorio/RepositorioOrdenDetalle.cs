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
    public class RepositorioOrdenDetalle : Repositorio<OrdenDetalle>, IRepositorioOrdenDetalle
    {
        private readonly LibreriaContexto contexto;

        public RepositorioOrdenDetalle(LibreriaContexto contexto) : base(contexto)
        {
            this.contexto = contexto;
        }

        public void Update(OrdenDetalle ordenDetalle)
        {
            contexto.OrdenesDetalle.Update(ordenDetalle);
        }
    }
}
