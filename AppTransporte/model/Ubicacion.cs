using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Ubicacion
    {
        public int IdUbicacion { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Referencias { get; set; } = string.Empty;
        public string CoordenadasMaps { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}
