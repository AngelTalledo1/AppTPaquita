namespace AppTransporte.model
{
    /// <summary>
    /// Represents a user type or role within the application.
    /// Examples include "Administrator", "Client", "Driver".
    /// </summary>
    public class TipoUsuario
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user type.
        /// </summary>
        public int IdTipoUsuario { get; set; }

        /// <summary>
        /// Gets or sets the description of the user type.
        /// </summary>
        public string descripcion { get; set; } = string.Empty;
    }
}