namespace AppTransporte.model
{
    /// <summary>
    /// Represents the association between a service and an order (a join entity).
    /// This class links a <see cref="model.Servicio"/> to a <see cref="model.Pedido"/>.
    /// </summary>
    public class ServicioPedido
    {
        /// <summary>
        /// Gets or sets the unique identifier for the service-order link.
        /// </summary>
        public int IdServicioPedido { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the associated service.
        /// </summary>
        public int IdServicio { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the associated order.
        /// </summary>
        public int IdPedido { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated <see cref="model.Servicio"/>.
        /// </summary>
        public Servicio Servicio { get; set; } = new Servicio();

        /// <summary>
        /// Gets or sets the navigation property to the associated <see cref="model.Pedido"/>.
        /// </summary>
        public Pedido Pedido { get; set; } = new Pedido();
    }
}