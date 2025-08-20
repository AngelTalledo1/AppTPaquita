using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class TrabajadorCliente
    {
        public int IdTrabajador { get; set; }
        public string NombreCompleto { get; set; }
        public string Categoria { get; set; }
        public string NumLicencia { get; set; }
        public int TotalViajes { get; set; }
        public int TotalSeguimientos { get; set; }
        public decimal VolumenTransportado { get; set; }
        public int PedidosAtendidos { get; set; }
        public DateTime? UltimoViaje { get; set; }
        public bool Row { get; set; } // Para alternar colores de filas

        // Propiedades para compatibilidad con el XAML existente
        public string Nombre => NombreCompleto;
        public string categoria_desc => Categoria;
        public int total_viajes => TotalViajes;
        public int total_seguimientos => TotalSeguimientos;
        public string volumen_transportado => $"{VolumenTransportado:N0} L";
    }
}