using System;

namespace AppTransporte.model
{
    /// <summary>
    /// Represents a tracking event (Seguimiento) for a specific trip (Viaje).
    /// This class logs the status, time, and other details at a point in time during a trip.
    /// </summary>
    public class Seguimiento
    {
        /// <summary>
        /// Gets or sets the unique identifier for the tracking event.
        /// </summary>
        public int IdSeguimiento { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the trip (<see cref="model.Viaje"/>) this event belongs to.
        /// </summary>
        public int IdViaje { get; set; }

        /// <summary>
        /// Gets or sets the description of the trip's status at the time of the event.
        /// </summary>
        public string EstadoViaje { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the tracking event occurred.
        /// </summary>
        public DateTime FechaHora { get; set; }

        /// <summary>
        /// Gets or sets a byte array representing evidence for the tracking event, such as a photo or document scan.
        /// This corresponds to a VARBINARY(MAX) in the database.
        /// </summary>
        public byte[]? Evidencia { get; set; }

        /// <summary>
        /// Gets or sets an optional comment associated with the tracking event.
        /// </summary>
        public string? Comentario { get; set; }
    }
}