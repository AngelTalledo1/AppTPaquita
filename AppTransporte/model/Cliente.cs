namespace AppTransporte.model
{
    /// <summary>
    /// Represents a client entity, containing personal and account information.
    /// This class models the data structure for a client in the system.
    /// </summary>
    public class Cliente
    {
        /// <summary>
        /// Gets or sets the unique identifier for the person record.
        /// </summary>
        public int IdPersona { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the client record.
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Gets or sets the client's first name.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the client's paternal last name.
        /// </summary>
        public string ApePaterno { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the client's maternal last name.
        /// </summary>
        public string ApeMaterno { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the client's document number (e.g., DNI, RUC).
        /// </summary>
        public string NumDoc { get; set; } = "N/A";

        /// <summary>
        /// Gets or sets the client's phone number.
        /// </summary>
        public string Telefono { get; set; } = "N/A";

        /// <summary>
        /// Gets or sets the client's address.
        /// </summary>
        public string Direccion { get; set; } = "N/A";

        /// <summary>
        /// Gets or sets the client's email address.
        /// </summary>
        public string Email { get; set; } = "N/A";

        /// <summary>
        /// Gets or sets the nullable foreign key for the associated user account.
        /// </summary>
        public int? IdUsuario { get; set; }

        /// <summary>
        /// Gets or sets the username for the client's account.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for the client's account.
        /// Note: Storing passwords in plain text is a security risk.
        /// </summary>
        public string Contraseña { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the nullable foreign key for the user type.
        /// </summary>
        public int? IdTipoUsuario { get; set; }

        /// <summary>
        /// Gets or sets the status of the client record (e.g., 1 for active, 0 for inactive).
        /// </summary>
        public int Estado { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="model.Persona"/> object.
        /// </summary>
        public Persona Persona { get; set; } = new Persona();

        /// <summary>
        /// Gets the full name of the client, concatenating the first name, paternal name, and maternal name.
        /// </summary>
        public string NombreCompleto => $"{Nombre} {ApePaterno} {ApeMaterno}";
    }
}