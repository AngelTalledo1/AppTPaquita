namespace AppTransporte.model
{
    /// <summary>
    /// Represents a type of identification document (e.g., DNI, RUC, Passport).
    /// </summary>
    public class TipoDocumento
    {
        /// <summary>
        /// Gets or sets the unique identifier for the document type.
        /// </summary>
        public int IdTipoDoc { get; set; }

        /// <summary>
        /// Gets or sets the description of the document type.
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TipoDocumento"/> class with a default empty description.
        /// </summary>
        public TipoDocumento()
        {
            Descripcion = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TipoDocumento"/> class with a specified ID and description.
        /// </summary>
        /// <param name="idTipoDoc">The unique identifier for the document type.</param>
        /// <param name="descripcion">The description of the document type.</param>
        public TipoDocumento(int idTipoDoc, string descripcion)
        {
            IdTipoDoc = idTipoDoc;
            Descripcion = descripcion;
        }
    }
}