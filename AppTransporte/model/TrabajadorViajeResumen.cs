using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class TrabajadorViajeResumen
    {
        public int IdTrabajador { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string? NumLicencia { get; set; }
        public int TotalViajesRealizados { get; set; }
        public int TotalPedidosAtendidos { get; set; }
        public decimal VolumenTotalTransportado { get; set; }
    }
}
