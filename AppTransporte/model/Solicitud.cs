using System;

namespace AppTransporte.model
{
    /// <summary>
    /// Represents a service request (Solicitud) made by a client.
    /// This is the initial step before an order (Pedido) is created.
    /// </summary>
    public class Solicitud
    {
        /// <summary>
        /// Gets or sets the unique identifier for the service request.
        /// </summary>
        public int IdSolicitud { get; set; }

        /// <summary>
        /// Gets or sets the description of the requested service.
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the request was made.
        /// </summary>
        [Obsolete("This property is redundant. Use 'Fecha' instead.")]
        public DateTime FechaSolicitud { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the request.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Gets or sets any additional comments provided by the client.
        /// </summary>
        public string? Comentario { get; set; }

        /// <summary>
        /// Gets or sets the description of the current status of the request (e.g., "Por revisar").
        /// </summary>
        public string EstadoSolicitud { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the foreign key for the status of the request (<see cref="model.EstadoSolicitud"/>).
        /// </summary>
        public int IdEstadoSolicitud { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the client who made the request.
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Gets or sets the full name of the client who made the request.
        /// </summary>
        public string Cliente { get; set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the 'Modify' action should be visible in the UI.
        /// This is true only if the request status is "Por revisar" (Pending Review).
        /// </summary>
        public bool MostrarModificar => EstadoSolicitud == "Por revisar";

        /// <summary>
        /// Gets a value indicating whether the 'View Order' action should be visible in the UI.
        /// This is true only if the request status is "Pedido Creado" (Order Created).
        /// </summary>
        public bool MostrarVerPedido => EstadoSolicitud == "Pedido Creado";
    }
}