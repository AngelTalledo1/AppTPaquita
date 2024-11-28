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
        public string IdEstadoSolicitud { get; set; } = string.Empty;
        public int IdCliente { get; set; }

        public Cliente Cliente { get; set; } = new Cliente();// Relación con Cliente
        public EstadoSolicitud EstadoSolicitud { get; set; } = new EstadoSolicitud();// Relación con EstadoSolicitud


        


    }

}
