using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class EvidenciasCisterna
    {
        public int IdEvidenciaCisterna { get; set; }
        public int IdCisterna { get; set; }
        public byte[] Imagen { get; set; }
        public byte[] Poliza { get; set; }
        public byte[] CITV { get; set; }
        public byte[] Cubicacion { get; set; }
        public byte[] TarjetaPropiedad { get; set; }

        public Cisterna Cisterna { get; set; } // Relación con Cisterna
    }
}
