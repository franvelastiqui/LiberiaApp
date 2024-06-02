using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelosApp.Models.ViewModels
{
    public class CarritoVM
    {
        public IEnumerable<Carrito> ListaCarritos { get; set; }
        public OrdenCompra OrdenCompra { get; set; }
    }
}
