using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Seguimiento
    {
        public int IdSeguimiento { get; set; }
        public int IdViaje { get; set; }
        public int IdEstadoViaje { get; set; }
        public DateTime FechaHora { get; set; }

        // La columna "evidencia" es de tipo VARBINARY(MAX), se representa como byte[]
        public byte[]? Evidencia { get; set; }

        // Comentario asociado al seguimiento
        public string? Comentario { get; set; }

        // Relaciones con otras entidades
        public Viaje Viaje { get; set; } = new Viaje();// Relación con la clase Viaje
        public EstadoViaje EstadoViaje { get; set; } = new EstadoViaje(); // Relación con la clase EstadoViaje
    }
}
