using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Destino
    {

        public int IdDestino { get; set; }
        public string Descripcion { get; set; }
        public string Sector { get; set; }
        public string Referencias { get; set; }
        public string CoordenadasMaps { get; set; }
        public bool Estado { get; set; }

    }
}
