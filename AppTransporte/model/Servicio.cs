namespace AppTransporte.model
{
    /// <summary>
    /// Represents a service that can be associated with an order.
    /// Examples might include "Loading", "Unloading", or "Special Handling".
    /// </summary>
    public class Servicio
    {
        /// <summary>
        /// Gets or sets the unique identifier for the service.
        /// </summary>
        public int IdServicio { get; set; }

        /// <summary>
        /// Gets or sets the description of the service.
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the service is currently active and available.
        /// </summary>
        public bool Estado { get; set; }
    }
}