using System;

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
        public int DiasDuracion { get; set; }
        public string EstadoDescripcion { get; set; }

        // Nuevas propiedades agregadas para asignaciones
        public int? IdTransportista { get; set; }
        public int? IdAyudante { get; set; }
        public int? IdTracto { get; set; }
        public int? IdCisterna { get; set; }

        // Propiedades para mostrar nombres en la interfaz
        public string NombreTransportista { get; set; }
        public string NombreAyudante { get; set; }
        public string PlacaTracto { get; set; }
        public string PlacaCisterna { get; set; }

        // Propiedades calculadas para mostrar en la interfaz (las que ya tenías)
        public string FechaInicioFormateada => FechaInicio.ToString("dd/MM/yyyy");
        public string FechaFinFormateada => FechaFin.ToString("dd/MM/yyyy");
        public string HoraProgramadaFormateada => HoraProgramada.ToString(@"hh\:mm");
        public string PeriodoCompleto => $"{FechaInicioFormateada} - {FechaFinFormateada}";
        public string DetalleCompleto => $"{TipoServicio} - {DiasSemana} a las {HoraProgramadaFormateada}";

        // Nuevas propiedades calculadas para mostrar asignaciones
        public string AsignacionTransportista => string.IsNullOrEmpty(NombreTransportista) ? "No asignado" : NombreTransportista;
        public string AsignacionAyudante => string.IsNullOrEmpty(NombreAyudante) ? "No asignado" : NombreAyudante;
        public string AsignacionTracto => string.IsNullOrEmpty(PlacaTracto) ? "No asignado" : PlacaTracto;
        public string AsignacionCisterna => string.IsNullOrEmpty(PlacaCisterna) ? "No asignada" : PlacaCisterna;

        // Resumen de asignaciones
        public string ResumenAsignaciones
        {
            get
            {
                var asignaciones = new List<string>();
                if (!string.IsNullOrEmpty(NombreTransportista)) asignaciones.Add($"Transp: {NombreTransportista}");
                if (!string.IsNullOrEmpty(NombreAyudante)) asignaciones.Add($"Ayud: {NombreAyudante}");
                if (!string.IsNullOrEmpty(PlacaTracto)) asignaciones.Add($"Tracto: {PlacaTracto}");
                if (!string.IsNullOrEmpty(PlacaCisterna)) asignaciones.Add($"Cisterna: {PlacaCisterna}");

                return asignaciones.Count > 0 ? string.Join(" | ", asignaciones) : "Sin asignaciones";
            }
        }

        // Para identificar el color del estado en la interfaz (tu implementación original)
        public string ColorEstado => EstadoDescripcion switch
        {
            "Activo" => "#28a745",
            "Pendiente" => "#ffc107",
            "Finalizado" => "#6c757d",
            "Inactivo" => "#dc3545",
            _ => "#6c757d"
        };

        // Color para indicar si tiene asignaciones completas
        public string ColorAsignacion
        {
            get
            {
                if (IdTransportista.HasValue && IdAyudante.HasValue && IdTracto.HasValue && IdCisterna.HasValue)
                    return "#28a745"; // Verde - completamente asignado
                else if (IdTransportista.HasValue || IdAyudante.HasValue || IdTracto.HasValue || IdCisterna.HasValue)
                    return "#ffc107"; // Amarillo - parcialmente asignado
                else
                    return "#dc3545"; // Rojo - sin asignaciones
            }
        }

        public string DescripcionAsignacion
        {
            get
            {
                if (IdTransportista.HasValue && IdAyudante.HasValue && IdTracto.HasValue && IdCisterna.HasValue)
                    return "Completamente asignado";
                else if (IdTransportista.HasValue || IdAyudante.HasValue || IdTracto.HasValue || IdCisterna.HasValue)
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