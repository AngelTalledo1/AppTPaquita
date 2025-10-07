namespace AppTransporte.model
{
    /// <summary>
    /// Represents a user account in the system.
    /// This class contains login credentials and links to the user's personal information and role.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user account.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Gets or sets the username for the account.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for the account.
        /// <para>SECURITY NOTE: Storing passwords in plain text is a major security risk.
        /// Passwords should always be hashed and salted.</para>
        /// </summary>
        public string Contraseña { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the foreign key for the user's type or role (<see cref="model.TipoUsuario"/>).
        /// </summary>
        public int IdTipoUsuario { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user account is active.
        /// </summary>
        public bool Estado { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the associated person record (<see cref="model.Persona"/>).
        /// </summary>
        public int IdPersona { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated <see cref="model.Persona"/> object,
        /// which contains detailed personal information.
        /// </summary>
        public Persona Persona { get; set; } = new Persona();

        /// <summary>
        /// Gets or sets the navigation property to the associated <see cref="model.TipoUsuario"/> object,
        /// which defines the user's role.
        /// </summary>
        public TipoUsuario TipoUsuario { get; set; } = new TipoUsuario();
    }
}