namespace AppTransporte.model
{
    /// <summary>
    /// A data transfer object (DTO) that represents the response from a successful user authentication.
    /// It contains essential identifiers for the authenticated user.
    /// </summary>
    public class UsuarioResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsuarioResponse"/> class.
        /// </summary>
        /// <param name="idUsuario">The unique identifier of the authenticated user.</param>
        /// <param name="idTipoUsuario">The identifier for the user's type or role.</param>
        public UsuarioResponse(int idUsuario, int idTipoUsuario)
        {
            this.idUsuario = idUsuario;
            this.idTipoUsuario = idTipoUsuario;
        }

        /// <summary>
        /// Gets or sets the unique identifier of the authenticated user.
        /// </summary>
        public int idUsuario { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the user's type or role.
        /// </summary>
        public int idTipoUsuario { get; set; }
    }
}