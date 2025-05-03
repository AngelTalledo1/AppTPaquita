using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class ResumenPedido
    {
        public int IdEstadoPedido { get; set; }
        public string EstadoPedido { get; set; }
        public int CantidadPedidos { get; set; }
        public int VolumenTotal { get; set; }
        public int ViajesSolicitados { get; set; }
        public int PedidosEntregados { get; set; }
        public double PromedioDiasAtencion { get; set; }
        public bool Row { get; set; }
    }
}
