using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class AtencionCliente
    {
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public int TotalSolicitudes { get; set; }
        public int SolicitudesCompletadas { get; set; }
        public double PorcentajeCompletadas { get; set; }
        public double PromedioHorasSolicitudPedido { get; set; }
        public double PromedioHorasPedidoEntrega { get; set; }
        public double PromedioHorasTotales { get; set; }
        public bool Row { get; set; }
        public string CompletadasFormateado => $"{SolicitudesCompletadas} ({PorcentajeCompletadas:P0})";
    }
}
