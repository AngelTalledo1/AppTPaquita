using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Vehiculo
    {
        //UNE TRACTO Y CISTERNA
        public int IdVehiculo { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string? Modelo { get; set; } = string.Empty;
        public DateTime? EmisionCubicacion { get; set; }
        public DateTime? VencimientoCubicacion { get; set; }
        public string AñoFabricacion { get; set; } = string.Empty;
        public DateTime? EmisionPoliza { get; set; }
        public DateTime? VencimientoPoliza { get; set; }
        public DateTime? EmisionCITV { get; set; }
        public DateTime? VencimientoCITV { get; set; }
        public bool Estado { get; set; }
        public byte[]? Imagen { get; set; }
        public byte[]? Poliza { get; set; }
        public byte[]? CITV { get; set; }
        public byte[]? Cubicacion { get; set; }
        public byte[]? TarjetaPropiedad { get; set; }

        public string Tipo { get; set; } = string.Empty;


    }
}
