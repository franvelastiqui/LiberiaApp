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
    public class OrdenCompra
    {
        [Key]
        public int ID { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        public string? EstadoOrden { get; set; }
        public string? EstadoPago { get; set; }
        public string? NumeroSeguimiento { get; set; }


        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de orden")]
        public DateTime FechaOrden { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de entrega")]
        public DateTime FechaDespacho { get; set; }

        public string? Transportista { get; set; }
        public string? SessionID { get; set; }
        public string? PaymentIntentID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de pago")]
        public DateTime FechaPago { get; set; }

        public string UsuarioAplicacionID { get; set; }
        [ForeignKey("UsuarioAplicacionID")]
        [ValidateNever]
        public UsuarioAplicacion? UsuarioAplicacion { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        public string Ciudad { get; set; }

        [Required]
        public string Provincia { get; set; }

        [Required]
        [Display(Name = "Código postal")]
        public string CodigoPostal { get; set; }
    }
}
