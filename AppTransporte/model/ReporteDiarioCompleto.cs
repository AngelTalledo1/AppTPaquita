using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class ReporteDiarioCompleto
    {
        public int IdTrabajador { get; set; }
        public string NombreTrabajador { get; set; }
        public string Categoria { get; set; }
        public DateTime Fecha { get; set; }
        public List<ActividadDiariaTrabajador> Actividades { get; set; } = new List<ActividadDiariaTrabajador>();
        public List<TareaAdicionalTrabajador> TareasAdicionales { get; set; } = new List<TareaAdicionalTrabajador>();

        // Estadísticas del reporte
        public int TotalViajes => Actividades.Select(a => a.IdViaje).Distinct().Count();
        public int TotalTareas => TareasAdicionales.Count;
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");
    }
}
