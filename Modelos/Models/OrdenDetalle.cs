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
    public class OrdenDetalle
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int OrdenCompraID { get; set; }

        [ForeignKey("OrdenCompraID")]
        [ValidateNever]
        public OrdenCompra OrdenCompra { get; set; }

        public int ProductoID { get; set; }

        [ForeignKey("ProductoID")]
        [ValidateNever]
        public Producto Producto { get; set; }

        [Display(Name = "Precio")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
}
