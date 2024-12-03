using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Viaje
    {
        public int IdViaje { get; set; }
        public int IdPedido { get; set; }
        public int? IdTracto { get; set; }
        public int? IdCisterna { get; set; }
        public int? Cantidad { get; set; }

        public Pedido Pedido { get; set; } = new Pedido();// Relación con Pedido
        public Vehiculo Tracto { get; set; } = new Vehiculo(); // Relación con Tracto
        public Vehiculo Cisterna { get; set; } = new Vehiculo();// Relación con Cisterna
    }
}
