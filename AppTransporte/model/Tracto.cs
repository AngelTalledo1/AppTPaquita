using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Tracto
    {
        public int IdTracto { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string AñoFabricacion { get; set; } = string.Empty;
        public DateTime? EmisionPoliza { get; set; }
        public DateTime? VencimientoPoliza { get; set; }
        public DateTime? EmisionCITV { get; set; }
        public DateTime? VencimientoCITV { get; set; }
        public bool Estado { get; set; }

        public string MostrarPlaca => $"Placa: {Placa}".Trim();
        public string MostrarModelo => $"Modelo: {Modelo}".Trim();
        public string MostrarAño => $"Año Fabricacion: {AñoFabricacion}".Trim();
        public string MostrarEmisionPoliza => $"Emision Poliza {EmisionPoliza}";
        public string MostrarVencimientoPoliza => $"Vencimiento Poliza: {VencimientoPoliza}";
        public string MostrarEmisionCITV => $"Emision CITV: {EmisionCITV}";
        public string MostrarVencimientoCITV => $"Vencimiento CITV: {VencimientoCITV}";
        public static string MostrarEmisionCubicacion => $"Emision Cubicacion: -- ";
        public static string MostrarVencimientoCubicacion => $"Vencimiento Cubicacion: --";
    }
}
