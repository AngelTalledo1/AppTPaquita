using System;
using System.Collections.Generic;

namespace AppTransporte.model
{
    public class PedidoAutomatico
    {
        public int IdPedidoAutomatico { get; set; }
        public int IdUsuario { get; set; }
        public int IdTipoServicio { get; set; }
        public string TipoServicio { get; set; }
        public string DiasSemana { get; set; }
        public TimeSpan HoraProgramada { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public DateTime? UltimoProcesamiento { get; set; }
        public string Descripcion { get; set; }

        // Campos principales según la nueva estructura de tabla
        public int? IdCliente { get; set; }
        public int? IdOrigen { get; set; }
        public int? IdDestino { get; set; }

        // Propiedades para mostrar nombres en la interfaz (información de JOINs)
        public string NombreCliente { get; set; }
        public string DescripcionOrigen { get; set; }
        public string DescripcionDestino { get; set; }

        // Propiedades calculadas para mostrar en la interfaz
        public string FechaInicioFormateada => FechaInicio.ToString("dd/MM/yyyy");
        public string FechaFinFormateada => FechaFin.ToString("dd/MM/yyyy");
        public string HoraProgramadaFormateada => HoraProgramada.ToString(@"hh\:mm");
        public string PeriodoCompleto => $"{FechaInicioFormateada} - {FechaFinFormateada}";
        public string DetalleCompleto => $"{TipoServicio} - {DiasSemana} a las {HoraProgramadaFormateada}";

        // Propiedades calculadas para mostrar información específica
        public string AsignacionCliente => string.IsNullOrEmpty(NombreCliente) ? "No asignado" : NombreCliente;
        public string AsignacionOrigen => string.IsNullOrEmpty(DescripcionOrigen) ? "No asignado" : DescripcionOrigen;
        public string AsignacionDestino => string.IsNullOrEmpty(DescripcionDestino) ? "No asignado" : DescripcionDestino;

        // Propiedades calculadas útiles
        public int DiasDuracion => (FechaFin - FechaInicio).Days + 1;
        public string EstadoDescripcion => Estado ? "Activo" : "Inactivo";

        // Resumen de información principal
        public string ResumenAsignaciones
        {
            get
            {
                var asignaciones = new List<string>();
                if (!string.IsNullOrEmpty(NombreCliente)) asignaciones.Add($"Cliente: {NombreCliente}");
                if (!string.IsNullOrEmpty(DescripcionOrigen)) asignaciones.Add($"Origen: {DescripcionOrigen}");
                if (!string.IsNullOrEmpty(DescripcionDestino)) asignaciones.Add($"Destino: {DescripcionDestino}");

                return asignaciones.Count > 0 ? string.Join(" | ", asignaciones) : "Sin asignaciones";
            }
        }

        // Para identificar el color del estado en la interfaz
        public string ColorEstado => EstadoDescripcion switch
        {
            "Activo" => "#28a745",
            "Inactivo" => "#dc3545",
            _ => "#6c757d"
        };

        // Color para indicar si tiene asignaciones completas
        public string ColorAsignacion
        {
            get
            {
                if (IdCliente.HasValue && IdOrigen.HasValue && IdDestino.HasValue)
                    return "#28a745"; // Verde - completamente asignado
                else if (IdCliente.HasValue || IdOrigen.HasValue || IdDestino.HasValue)
                    return "#ffc107"; // Amarillo - parcialmente asignado
                else
                    return "#dc3545"; // Rojo - sin asignaciones
            }
        }

        public string DescripcionAsignacion
        {
            get
            {
                if (IdCliente.HasValue && IdOrigen.HasValue && IdDestino.HasValue)
                    return "Completamente asignado";
                else if (IdCliente.HasValue || IdOrigen.HasValue || IdDestino.HasValue)
                    return "Parcialmente asignado";
                else
                    return "Sin asignaciones";
            }
        }
    }

    // Clase para respuestas de procedimientos almacenados (tu implementación original)
    public class RespuestaPedidoAutomatico
    {
        public int? IdPedidoAutomatico { get; set; }
        public string Mensaje { get; set; }
        public int FilasAfectadas { get; set; }
        public bool EsExitoso => !string.IsNullOrEmpty(Mensaje) && !Mensaje.StartsWith("Error");
    }
}