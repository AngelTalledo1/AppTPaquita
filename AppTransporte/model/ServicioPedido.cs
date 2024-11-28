using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class ServicioPedido
    {
        public int IdServicioPedido { get; set; }
        public int IdServicio { get; set; }
        public int IdPedido { get; set; }

        public Servicio Servicio { get; set; } = new Servicio(); // Relación con Servicio
        public Pedido Pedido { get; set; } = new Pedido(); // Relación con Pedido
    }
}
