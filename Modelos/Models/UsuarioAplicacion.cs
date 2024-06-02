using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelosApp.Models
{
    public class UsuarioAplicacion : IdentityUser
    {
        [Required]
        public string Nombre { get; set; }

        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Provincia { get; set; }
        public string? CodigoPostal { get; set; }
    }
}
