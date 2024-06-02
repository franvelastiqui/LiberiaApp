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
    public class RepositorioUsuarioAplicacion : Repositorio<UsuarioAplicacion>, IRepositorioUsuarioAplicacion
    {
        private readonly LibreriaContexto contexto;

        public RepositorioUsuarioAplicacion(LibreriaContexto contexto) : base(contexto)
        {
            this.contexto = contexto;
        }
        public void Update(UsuarioAplicacion usuarioAplicacion)
        {
            contexto.UsuariosAplicacion.Update(usuarioAplicacion);
        }
    }
}
