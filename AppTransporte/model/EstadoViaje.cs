namespace AppTransporte.model
{
    /// <summary>
    /// Represents the status of a trip (Viaje).
    /// </summary>
    public class EstadoViaje
    {
        /// <summary>
        /// Gets or sets the unique identifier for the trip status.
        /// </summary>
        public int IdEstadoViaje { get; set; }

        /// <summary>
        /// Gets or sets the description of the trip status (e.g., "At Origin", "En Route", "At Destination").
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;
    }
}