using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Pedidos
    {
        public string Numero { get; set; }
        public ImageSource imagen { get; set; }
        public double cantidad { get; set; }
        public string estado { get; set; }
    }
}
