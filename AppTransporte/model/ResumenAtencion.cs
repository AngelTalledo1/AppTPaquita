using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class ResumenAtencion
    {
        public double PromedioHorasSolicitudPedido { get; set; }
        public double PromedioHorasPedidoEntrega { get; set; }
        public double PromedioHorasTotalesCompletados { get; set; }
        public double PromedioHorasTotalesTodos { get; set; }
        public int TotalSolicitudes { get; set; }
        public int SolicitudesCompletadas { get; set; }
        public double PorcentajeCompletado { get; set; }
    }
}
