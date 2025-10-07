namespace AppTransporte.model
{
    /// <summary>
    /// Represents a geographical location, used as an origin or destination for trips.
    /// </summary>
    public class Ubicacion
    {
        /// <summary>
        /// Gets or sets the unique identifier for the location.
        /// </summary>
        public int IdUbicacion { get; set; }

        /// <summary>
        /// Gets or sets the primary description of the location (e.g., "Main Warehouse", "Client Facility A").
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sector, district, or general area of the location.
        /// </summary>
        public string Sector { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional references or landmarks to help find the location.
        /// </summary>
        public string Referencias { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the geographical coordinates (e.g., latitude, longitude) for mapping purposes.
        /// </summary>
        public string CoordenadasMaps { get; set; } = string.Empty;
    }
}