using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdSolicitud { get; set; }
        public int Cantidad { get; set; }
        public int Viajes { get; set; }
        public int IdOrigen { get; set; }
        public int IdDestino { get; set; }
        public string EstadoPedido { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaEntrega { get; set; }
    }
}
