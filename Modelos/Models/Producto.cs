using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ModelosApp.Models
{
    public class Producto
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "El campo Título es obligatorio")]
        [DisplayName("Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El campo Descripción es obligatorio")]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El campo ISBN es obligatorio")]
        [RegularExpression("^978[0-9]{10}", ErrorMessage = "El número debe contener 13 cifras y comenzar con 978")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "El campo Autor es obligatorio")]
        public string Autor { get; set; }

        [Display(Name = "Precio")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Precio { get; set; }

        public int CategoriaID { get; set; }

        [ForeignKey("CategoriaID")]
        public Categoria? Categoria { get; set; }

        [DisplayName("Imágen")]
        public string? RutaImagen { get; set; }
    }
}
