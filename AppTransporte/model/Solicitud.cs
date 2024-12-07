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


        public string MostrarIdSolicitud => $"Solicitud ID:     {IdSolicitud}".Trim();
        public string MostrarCliente => $"Cliente:  {Cliente}".Trim();

        public string MostrarDescripcion => $"Descripcion de Solicitud: \n  {Descripcion}".Trim();

        public string MostrarComentario => $"Comentario: \n {Comentario}".Trim();
        public string MostrarFecha => $"Fecha Solicitud:    {Fecha}".Trim();
        public string MostrarEstado => $"Estado de solicitud:   {EstadoSolicitud}".Trim();

        public bool MostrarModificar => EstadoSolicitud == "Pendiente";
        public bool MostrarVerPedido => EstadoSolicitud == "Pedido Creado";


    }

}
