using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class EvidenciasTracto
    {
        public int IdEvidenciaTracto { get; set; }
        public int IdTracto { get; set; }
        public byte[] Imagen { get; set; }
        public byte[] Poliza { get; set; }
        public byte[] CITV { get; set; }
        public byte[] Cubicacion { get; set; }
        public byte[] TarjetaPropiedad { get; set; }

        public Tracto Tracto { get; set; } // Relación con Tracto
    }
}
