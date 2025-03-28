using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    internal class Pedido1 : Pedido
    {
        public int IdPedido { get; set; }
        public string EstadoPedido { get; set; }
        public DateTime FechaProgramada { get; set; }
        public TimeSpan HoraProgramada { get; set; }
        public string Descripcion { get; set; }
        public string Detalle { get; set; }
    }
}