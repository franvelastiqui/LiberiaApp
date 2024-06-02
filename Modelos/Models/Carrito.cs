using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelosApp.Models
{
    public class Carrito
    {
        [Key]
        public int ID { get; set; }

        [Range(1, 100)]
        public int Cantidad { get; set; }

        public int ProductoID { get; set; }

        [ValidateNever]
        [ForeignKey("ProductoID")]
        public Producto Producto { get; set; }


        [ValidateNever]
        public string UsuarioAplicacionID { get; set; }
        [ForeignKey("UsuarioAplicacionID")]
        [ValidateNever]
        public UsuarioAplicacion? UsuarioAplicacion { get; set; }
    }
}
