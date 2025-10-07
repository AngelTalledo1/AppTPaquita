namespace AppTransporte.model
{
    /// <summary>
    /// A data transfer object (DTO) used to capture user credentials for authentication.
    /// </summary>
    public class UsuarioLogin
    {
        /// <summary>
        /// Gets or sets the username for the login attempt.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for the login attempt.
        /// </summary>
        public string Contraseña { get; set; }
    }
}