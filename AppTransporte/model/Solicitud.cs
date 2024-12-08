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
        public string Descripcion { get; set; } = string.Empty;
        public string? Comentario { get; set; }
        public string EstadoSolicitud { get; set; } = string.Empty ;

        public DateTime Fecha { get; set; }
        public int IdEstadoSolicitud { get; set; }
        public int IdCliente { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public bool MostrarModificar => EstadoSolicitud == "Por revisar";
        public bool MostrarVerPedido => EstadoSolicitud == "Pedido Creado";


    }

}
