using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class ReporteServicio
    {
        public int IdServicio { get; set; }
        public string TipoServicio { get; set; }
        public int CantidadPedidos { get; set; }
        public int VolumenSolicitado { get; set; }
        public int VolumenTransportado { get; set; }
        public double PorcentajeCumplimiento { get; set; }
        public bool Row { get; set; } // Para alternar colores de filas
    }
}

