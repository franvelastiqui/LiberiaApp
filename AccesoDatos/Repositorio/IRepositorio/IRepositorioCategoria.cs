using ModelosApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorioCategoria : IRepositorio<Categoria>
    {
        void Update(Categoria categoria);
    }
}
