using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Solicitud
    {
        public int IdSolicitud { get; set; }
        public string Descripcion { get; set; }
        public string Comentario { get; set; }
        public int IdEstadoSolicitud { get; set; }
        public int IdCliente { get; set; }

        public Cliente Cliente { get; set; } // Relación con Cliente
        public EstadoSolicitud EstadoSolicitud { get; set; } // Relación con EstadoSolicitud
    }
}
