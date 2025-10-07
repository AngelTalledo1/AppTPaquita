using System;

namespace AppTransporte.model
{
    /// <summary>
    /// Represents a customer order (Pedido), detailing the specifics of a transport service.
    /// This class holds information about the order's quantity, origin, destination,
    /// associated client and user, and its current status.
    /// </summary>
    public class Pedido
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order.
        /// </summary>
        public int IdPedido { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the service request (<see cref="model.Solicitud"/>) that generated this order.
        /// </summary>
        public int IdSolicitud { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the user who created the order.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Gets or sets the quantity of goods to be transported.
        /// </summary>
        public int Cantidad { get; set; }

        /// <summary>
        /// Gets or sets the number of trips required for this order.
        /// </summary>
        public int Viajes { get; set; }

        /// <summary>
        /// Gets or sets the description of the origin location.
        /// </summary>
        public string Origen { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sector or district of the origin location.
        /// </summary>
        public string? OrigSector { get; set; }

        /// <summary>
        /// Gets or sets the name of the client associated with the order.
        /// </summary>
        public string Cliente { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username of the user who placed the order.
        /// </summary>
        public string Usuario { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a descriptive string of related services for the order.
        /// </summary>
        public string Servicios { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the destination location.
        /// </summary>
        public string Destino { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sector or district of the destination location.
        /// </summary>
        public string? DestSector { get; set; }

        /// <summary>
        /// Gets or sets the current status of the order (e.g., "Pending", "In Progress", "Completed").
        /// </summary>
        public string EstadoPedido { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the order was requested.
        /// </summary>
        public DateTime FechaSolicitud { get; set; }

        /// <summary>
        /// Gets or sets the nullable date and time for the order's expected or actual delivery.
        /// </summary>
        public DateTime? FechaEntrega { get; set; }
    }
}