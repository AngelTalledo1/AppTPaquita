using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using AppTransporte.model;


namespace AppTransporte.model
{
    public class TareaAdicional
    {
        public int id_tareaAdicional { get; set; }
        public DateTime fecha_tarea { get; set; }
        public TimeSpan hora_inicio { get; set; }
        public TimeSpan hora_fin { get; set; }
        public string descripcion { get; set; }
        public int id_usuario { get; set; }
        public DateTime fecha_creacion { get; set; }
        public DateTime fecha_modificacion { get; set; }
        public bool estado { get; set; } = true;
        public int duracion_minutos { get; set; }
        public string nombre_usuario { get; set; }

        // Propiedades para la vista
        public string FechaTareaString => fecha_tarea.ToString("dd/MM/yyyy");
        public string HoraInicioString => hora_inicio.ToString(@"hh\:mm");
        public string HoraFinString => hora_fin.ToString(@"hh\:mm");
        public string DuracionString => $"{duracion_minutos} minutos";
        public string FechaCreacionString => fecha_creacion.ToString("dd/MM/yyyy HH:mm");
        public string HorarioCompleto => $"{HoraInicioString} - {HoraFinString} ({DuracionString})";
    }
}
