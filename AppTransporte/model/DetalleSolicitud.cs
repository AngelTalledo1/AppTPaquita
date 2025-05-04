using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class DetalleSolicitud
    {
        public int IdSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaPedido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string Cliente { get; set; }
        public string DescripcionSolicitud { get; set; }
        public string Comentario { get; set; }
        public string Estado { get; set; }
        public double HorasHastaPedido { get; set; }
        public double HorasHastaEntrega { get; set; }
        public double HorasTotales { get; set; }
        public string Completado { get; set; }
        public bool Row { get; set; }
    }
}
