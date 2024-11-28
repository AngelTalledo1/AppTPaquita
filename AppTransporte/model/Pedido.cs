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
        public int IdUsuario { get; set; }
        public int Cantidad { get; set; }
        public int Viajes { get; set; }
        public int IdOrigen { get; set; }
        public int IdDestino { get; set; }
        public int IdEstadoPedido { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaEntrega { get; set; }

        public Solicitud Solicitud { get; set; } = new Solicitud();// Relación con Solicitud
        public Usuario Usuario { get; set; } = new Usuario();// Relación con Usuario
        public Destino Destino { get; set; } = new Destino();// Relación con Destino
        public Origen Origen { get; set; } = new Origen();// Relación con Origen
        public EstadoPedido EstadoPedido { get; set; } = new EstadoPedido();// Relación con EstadoPedido
    }
}
