namespace AppTransporte.model
{
    /// <summary>
    /// Represents a category, typically for workers or services.
    /// </summary>
    public class Categoria
    {
        /// <summary>
        /// Gets or sets the unique identifier for the category.
        /// </summary>
        public int IdCategoria { get; set; }

        /// <summary>
        /// Gets or sets the description of the category (e.g., "Driver", "Mechanic").
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Categoria"/> class with an empty description.
        /// </summary>
        public Categoria()
        {
            Descripcion = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Categoria"/> class with a specified description.
        /// </summary>
        /// <param name="descripcion">The description of the category.</param>
        public Categoria(string descripcion)
        {
            Descripcion = descripcion;
        }
    }
}