using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Cisterna
    {
        public int IdCisterna { get; set; }
        public string Placa { get; set; }
        public string AñoFabricacion { get; set; }
        public DateTime EmisionCubicacion { get; set; }
        public DateTime VencimientoCubicacion { get; set; }
        public DateTime? EmisionPoliza { get; set; }
        public DateTime? VencimientoPoliza { get; set; }
        public DateTime? EmisionCITV { get; set; }
        public DateTime? VencimientoCITV { get; set; }
        public bool Estado { get; set; }
    }
}
