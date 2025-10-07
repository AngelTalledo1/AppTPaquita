namespace AppTransporte.model
{
    /// <summary>
    /// Represents the assignment of a worker to a trip (a join entity).
    /// This class links a <see cref="model.Trabajador"/> to a <see cref="model.Viaje"/>.
    /// </summary>
    public class TrabajadorViaje
    {
        /// <summary>
        /// Gets or sets the unique identifier for the worker-trip assignment.
        /// </summary>
        public int IdTrabajadorViaje { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the associated trip.
        /// </summary>
        public int IdViaje { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the associated worker.
        /// </summary>
        public int IdTrabajador { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated <see cref="model.Viaje"/>.
        /// </summary>
        public Viaje Viaje { get; set; } = new Viaje();

        /// <summary>
        /// Gets or sets the navigation property to the associated <see cref="model.Trabajador"/>.
        /// </summary>
        public Trabajador Trabajador { get; set; } = new Trabajador();
    }
}