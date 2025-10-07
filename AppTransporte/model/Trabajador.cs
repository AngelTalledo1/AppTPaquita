namespace AppTransporte.model
{
    /// <summary>
    /// Represents a worker (Trabajador) entity, such as a driver or other employee.
    /// This class holds personal details, job-related information, and account credentials.
    /// </summary>
    public class Trabajador
    {
        /// <summary>
        /// Gets or sets the unique identifier for the worker.
        /// </summary>
        public int IdTrabajador { get; set; }

        /// <summary>
        /// Gets or sets the worker's first name.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the worker's paternal last name.
        /// </summary>
        public string? apePaterno { get; set; }

        /// <summary>
        /// Gets or sets the worker's maternal last name.
        /// </summary>
        public string? apeMaterno { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the foreign key for the worker's document type.
        /// </summary>
        public int idtipoDoc { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the worker's job category.
        /// </summary>
        public int idcategoria { get; set; }

        /// <summary>
        /// Gets or sets the worker's document number.
        /// </summary>
        public string numDoc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the worker's phone number.
        /// </summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the worker's address.
        /// </summary>
        public string direccion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the worker's email address.
        /// </summary>
        public string? email { get; set; }

        /// <summary>
        /// Gets or sets the description of the worker's job category.
        /// </summary>
        public string categoria { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the worker's driver's license number, if applicable.
        /// </summary>
        public string? licencia { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username for the worker's account.
        /// </summary>
        public string usuario { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for the worker's account.
        /// Note: Storing passwords in plain text is a security risk.
        /// </summary>
        public string password { get; set; } = string.Empty;

        /// <summary>
        /// Gets the worker's full name, created by combining first and last names.
        /// </summary>
        public string NombreCompleto => $"{Nombre} {apePaterno} {apeMaterno}".Trim();

        /// <summary>
        /// Gets the worker's first name and paternal last name.
        /// </summary>
        public string NombreTrabajador => $"{Nombre} {apePaterno}".Trim();

        /// <summary>
        /// Gets the worker's full last name.
        /// </summary>
        public string apellidoTrabajador => $"{apePaterno} {apeMaterno}".Trim();
    }
}