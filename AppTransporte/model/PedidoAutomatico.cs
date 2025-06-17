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

        // Propiedades calculadas para mostrar en la interfaz
        public string FechaInicioFormateada => FechaInicio.ToString("dd/MM/yyyy");
        public string FechaFinFormateada => FechaFin.ToString("dd/MM/yyyy");
        public string HoraProgramadaFormateada => HoraProgramada.ToString(@"hh\:mm");
        public string PeriodoCompleto => $"{FechaInicioFormateada} - {FechaFinFormateada}";
        public string DetalleCompleto => $"{TipoServicio} - {DiasSemana} a las {HoraProgramadaFormateada}";

        // Para identificar el color del estado en la interfaz
        public string ColorEstado => EstadoDescripcion switch
        {
            "Activo" => "#28a745",
            "Pendiente" => "#ffc107",
            "Finalizado" => "#6c757d",
            "Inactivo" => "#dc3545",
            _ => "#6c757d"
        };
    }

    // Clase para respuestas de procedimientos almacenados
    public class RespuestaPedidoAutomatico
    {
        public int? IdPedidoAutomatico { get; set; }
        public string Mensaje { get; set; }
        public int FilasAfectadas { get; set; }
        public bool EsExitoso => !string.IsNullOrEmpty(Mensaje) && !Mensaje.StartsWith("Error");
    }
}