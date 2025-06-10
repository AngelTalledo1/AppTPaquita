using System;

namespace AppTransporte.model
{
    public class PedidoProgramado
    {
        public int IdPedidoProgramado { get; set; }
        public int IdUsuario { get; set; }
        public int? IdCliente { get; set; }
        public string TipoServicio { get; set; }
        public string Frecuencia { get; set; }
        public string DiasSeleccionados { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public TimeSpan HoraProgramada { get; set; }
        public int CantidadBarriles { get; set; }
        public decimal CantidadLitros { get; set; }
        public string Estado { get; set; }
        public string Descripcion { get; set; }
        public int? IdOrigen { get; set; }
        public int? IdDestino { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public DateTime? UltimaEjecucion { get; set; }
        public DateTime? ProximaEjecucion { get; set; }
        public int TotalEjecuciones { get; set; }

        // Propiedades adicionales
        public string UsuarioCreador { get; set; }
        public string NombreUsuario { get; set; }
        public string CodigoCliente { get; set; }
        public string NombreCliente { get; set; }
        public string OrigenDescripcion { get; set; }
        public string DestinoDescripcion { get; set; }
        public string EstadoActual { get; set; }
        public string EstadoDescripcion { get; set; }
        public int? DiasHastaEjecucion { get; set; }

        // Propiedades calculadas
        public string FrecuenciaDescripcion => $"{Frecuencia}{(Frecuencia == "Personalizada" && !string.IsNullOrEmpty(DiasSeleccionados) ? $" ({DiasSeleccionados})" : "")}";

        public string ProximaEjecucionTexto => ProximaEjecucion?.ToString("dd/MM/yyyy HH:mm") ?? "No programada";

        public string UltimaEjecucionTexto => UltimaEjecucion?.ToString("dd/MM/yyyy HH:mm") ?? "Nunca";

        public string RangoFechas => $"{FechaInicio:dd/MM/yyyy} - {FechaFin:dd/MM/yyyy}";

        public string CantidadTexto => $"{CantidadBarriles} barriles ({CantidadLitros:N0} L)";
    }
}