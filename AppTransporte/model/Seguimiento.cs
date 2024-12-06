using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Seguimiento
    {
        public int IdSeguimiento { get; set; }
        public int IdViaje { get; set; }
        public string EstadoViaje { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        

        // La columna "evidencia" es de tipo VARBINARY(MAX), se representa como byte[]
        public byte[]? Evidencia { get; set; }

        // Comentario asociado al seguimiento
        public string? Comentario { get; set; }


    }

}
