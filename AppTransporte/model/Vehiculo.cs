using System;

namespace AppTransporte.model
{
    /// <summary>
    /// Represents a generic vehicle, which can be a tractor or a tanker.
    /// This class consolidates properties common to different types of vehicles used in transport operations,
    /// including identification, specifications, and legal documentation.
    /// </summary>
    public class Vehiculo
    {
        /// <summary>
        /// Gets or sets the unique identifier for the vehicle.
        /// </summary>
        public int IdVehiculo { get; set; }

        /// <summary>
        /// Gets or sets the vehicle's license plate number.
        /// </summary>
        public string Placa { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the model of the vehicle.
        /// </summary>
        public string? Modelo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the issue date of the vehicle's cubication certificate.
        /// (Applies mainly to tankers).
        /// </summary>
        public DateTime? EmisionCubicacion { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the vehicle's cubication certificate.
        /// (Applies mainly to tankers).
        /// </summary>
        public DateTime? VencimientoCubicacion { get; set; }

        /// <summary>
        /// Gets or sets the year of manufacture for the vehicle.
        /// </summary>
        public string AñoFabricacion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the issue date of the vehicle's insurance policy.
        /// </summary>
        public DateTime? EmisionPoliza { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the vehicle's insurance policy.
        /// </summary>
        public DateTime? VencimientoPoliza { get; set; }

        /// <summary>
        /// Gets or sets the issue date of the vehicle's Technical Inspection Certificate (CITV).
        /// </summary>
        public DateTime? EmisionCITV { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the vehicle's Technical Inspection Certificate (CITV).
        /// </summary>
        public DateTime? VencimientoCITV { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the vehicle is active or not.
        /// </summary>
        public bool Estado { get; set; }

        /// <summary>
        /// Gets or sets a byte array representing an image of the vehicle.
        /// </summary>
        public byte[]? Imagen { get; set; }

        /// <summary>
        /// Gets or sets a byte array representing the scanned insurance policy document.
        /// </summary>
        public byte[]? Poliza { get; set; }

        /// <summary>
        /// Gets or sets a byte array representing the scanned CITV document.
        /// </summary>
        public byte[]? CITV { get; set; }

        /// <summary>
        /// Gets or sets a byte array representing the scanned cubication certificate.
        /// </summary>
        public byte[]? Cubicacion { get; set; }

        /// <summary>
        /// Gets or sets a byte array representing the scanned vehicle property card.
        /// </summary>
        public byte[]? TarjetaPropiedad { get; set; }

        /// <summary>
        /// Gets or sets the type of the vehicle (e.g., "Tracto", "Cisterna").
        /// </summary>
        public string Tipo { get; set; } = string.Empty;
    }
}