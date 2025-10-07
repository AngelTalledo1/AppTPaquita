namespace AppTransporte.model
{
    /// <summary>
    /// Represents a base person entity, containing common personal information.
    /// This class is used as a base for more specific entities like Client or Worker.
    /// </summary>
    public class Persona
    {
        /// <summary>
        /// Gets or sets the unique identifier for the person.
        /// </summary>
        public int IdPersona { get; set; }

        /// <summary>
        /// Gets or sets the person's first name.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the person's paternal last name.
        /// </summary>
        public string? ApePaterno { get; set; }

        /// <summary>
        /// Gets or sets the person's maternal last name.
        /// </summary>
        public string? ApeMaterno { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the person's document type.
        /// </summary>
        public int IdTipoDoc { get; set; }

        /// <summary>
        /// Gets or sets the person's document number.
        /// </summary>
        public string NumDoc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the person's phone number.
        /// </summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the person's address.
        /// </summary>
        public string Direccion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the person's email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="model.TipoDocumento"/> object.
        /// This represents the type of document the person has.
        /// </summary>
        public TipoDocumento TipoDocumento { get; set; } = new TipoDocumento();

        /// <summary>
        /// Gets the person's full name by combining the first name and last names.
        /// </summary>
        public string NombreCompleto => $"{Nombre} {ApePaterno} {ApeMaterno}".Trim();
    }
}