using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelosApp.Models
{
    public class Categoria
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "El campo Categoría es obligatorio")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "El número de caracteres no es válido")]
        [DisplayName("Categoría")]
        public string Nombre { get; set; }
    }
}
