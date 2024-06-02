using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelosApp.Models.ViewModels
{
    public class OrdenVM
    {
        public OrdenCompra OrdenCompra { get; set; }
        public IEnumerable<OrdenDetalle> OrdenDetalle { get; set; }
    }
}
