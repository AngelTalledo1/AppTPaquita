using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class TareaAdicionalTrabajador
    {
        public int IdTareaAdicional { get; set; }
        public DateTime FechaTarea { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }

        // Propiedades para visualización en UI
        public string FechaTareaFormateada => FechaTarea.ToString("dd/MM/yyyy");
        public string HorarioCompleto => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
        public string EstadoTexto => Estado ? "Completada" : "Pendiente";
        public string DuracionFormateada
        {
            get
            {
                var duracion = HoraFin - HoraInicio;
                return duracion.TotalMinutes > 0
                    ? $"{duracion.Hours}h {duracion.Minutes}m"
                    : "Duración inválida";
            }
        }
    }
}
