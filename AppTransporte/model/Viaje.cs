namespace AppTransporte.model
{
    /// <summary>
    /// Represents a single trip (Viaje) associated with an order.
    /// A single order may be fulfilled by multiple trips.
    /// </summary>
    public class Viaje
    {
        /// <summary>
        /// Gets or sets the unique identifier for the trip.
        /// </summary>
        public int IdViaje { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the order (<see cref="model.Pedido"/>) this trip belongs to.
        /// </summary>
        public int IdPedido { get; set; }

        /// <summary>
        /// Gets or sets the license plate of the assigned tractor vehicle. "S/A" indicates "Not Assigned".
        /// </summary>
        public string TractoAsig { get; set; } = "S/A";

        /// <summary>
        /// Gets or sets the license plate of the assigned tanker vehicle. "S/A" indicates "Not Assigned".
        /// </summary>
        public string CisternaAsig { get; set; } = "S/A";

        /// <summary>
        /// Gets or sets the name of the assigned transport driver. "S/A" indicates "Not Assigned".
        /// </summary>
        public string Transportista { get; set; } = "S/A";

        /// <summary>
        /// Gets or sets the name of the assigned assistant. "S/A" indicates "Not Assigned".
        /// </summary>
        public string Ayudante { get; set; } = "S/A";

        /// <summary>
        /// Gets or sets the quantity of goods transported in this specific trip.
        /// </summary>
        public int? Cantidad { get; set; }

        /// <summary>
        /// Gets or sets a string listing the workers assigned to the trip. "S/A" indicates "Not Assigned".
        /// </summary>
        public string TrabajadoresAsig { get; set; } = "S/A";

        /// <summary>
        /// Gets or sets a string describing the last known status of the trip. "S/A" indicates "Not Assigned".
        /// </summary>
        public string ultEstado { get; set; } = "S/A";

        /// <summary>
        /// Gets or sets the navigation property to the associated <see cref="model.Pedido"/>.
        /// </summary>
        public Pedido Pedido { get; set; } = new Pedido();

        /// <summary>
        /// Gets or sets the navigation property to the assigned tractor <see cref="model.Vehiculo"/>.
        /// </summary>
        public Vehiculo Tracto { get; set; } = new Vehiculo();

        /// <summary>
        /// Gets or sets the navigation property to the assigned tanker <see cref="model.Vehiculo"/>.
        /// </summary>
        public Vehiculo Cisterna { get; set; } = new Vehiculo();
    }
}