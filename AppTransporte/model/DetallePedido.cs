using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class DetallePedido
    {
        public int IdPedido { get; set; }
        public int IdSolicitud { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string Estado { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public int Volumen { get; set; }
        public int ViajesSolicitados { get; set; }
        public int ViajesRealizados { get; set; }
        public bool Row { get; set; }
    }
}
