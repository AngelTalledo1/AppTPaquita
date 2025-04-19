using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class ReporteTrabajador
    {
        public int IdTrabajador { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int TotalViajes { get; set; }
        public int TotalSeguimientos { get; set; }
        public int VolumenTransportado { get; set; }
        public string Periodo { get; set; } = string.Empty;
    }

}
