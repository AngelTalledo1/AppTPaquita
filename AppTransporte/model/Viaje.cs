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
        public string TractoAsig { get; set; } = "S/A";
        public string CisternaAsig { get; set; } = "S/A";
        public string Transportista { get; set; } = "S/A";
        public string Ayudante { get; set; } = "S/A";
        public int? Cantidad { get; set; } 
        public string TrabajadoresAsig { get; set; } = "S/A";
        public string ultEstado {  get; set; } = "S/A";

        public Pedido Pedido { get; set; } = new Pedido();// Relación con Pedido
        public Vehiculo Tracto { get; set; } = new Vehiculo(); // Relación con Tracto
        public Vehiculo Cisterna { get; set; } = new Vehiculo();// Relación con Cisterna
    }
}
