using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class DetalleViajeTrabajador
    {
        public int IdViaje { get; set; }
        public DateTime FechaProgramada { get; set; }
        public decimal Volumen { get; set; }
        public string Tracto { get; set; } = string.Empty;
        public string Cisterna { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public string UltimoEstado { get; set; } = string.Empty;
        public bool Completado { get; set; }

        // Propiedad calculada para facilitar el binding en la vista
        public string OrigenDestino => $"{Origen} → {Destino}";
    }
}
