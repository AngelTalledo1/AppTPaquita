namespace AppTransporte.model
{
    /// <summary>
    /// Represents a service that can be selected in the user interface.
    /// This class wraps the base <see cref="Servicio"/> properties and adds a flag
    /// to track whether it has been selected by the user, typically in a list.
    /// </summary>
    public class ServicioSeleccionable
    {
        /// <summary>
        /// Gets or sets the unique identifier for the service.
        /// </summary>
        public int IdServicio { get; set; }

        /// <summary>
        /// Gets or sets the description of the service.
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service is active.
        /// </summary>
        public bool Estado { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this service is selected in the UI.
        /// </summary>
        public bool IsSelected { get; set; } = false;
    }
}