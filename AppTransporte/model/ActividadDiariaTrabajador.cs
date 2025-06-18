using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class ActividadDiariaTrabajador
    {
        public int IdViaje { get; set; }
        public int IdPedido { get; set; }
        public int? Cantidad { get; set; }
        public string EstadoActual { get; set; }
        public DateTime FechaHora { get; set; }
        public string Evidencia { get; set; }
        public string Comentario { get; set; }

        // Propiedades adicionales para visualización
        public string HoraFormateada => FechaHora.ToString("HH:mm");
        public string FechaFormateada => FechaHora.ToString("dd/MM/yyyy");
    }
}
