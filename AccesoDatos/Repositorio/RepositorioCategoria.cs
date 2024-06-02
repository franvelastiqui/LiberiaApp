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
    public class RepositorioCategoria : Repositorio<Categoria>, IRepositorioCategoria
    {
        private readonly LibreriaContexto contexto;

        public RepositorioCategoria(LibreriaContexto contexto) : base(contexto)
        {
            this.contexto = contexto;
        }

        public void Update(Categoria categoria)
        {
            contexto.Categorias.Update(categoria);
        }
    }
}
