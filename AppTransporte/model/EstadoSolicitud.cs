namespace AppTransporte.model
{
    /// <summary>
    /// Represents the status of a service request (Solicitud).
    /// </summary>
    public class EstadoSolicitud
    {
        /// <summary>
        /// Gets or sets the unique identifier for the request status.
        /// </summary>
        public int IdEstadoSolicitud { get; set; }

        /// <summary>
        /// Gets or sets the description of the request status (e.g., "Pending Review", "Approved", "Rejected").
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;
    }
}