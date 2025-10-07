namespace AppTransporte.model
{
    /// <summary>
    /// Represents the status of an order (Pedido).
    /// </summary>
    public class EstadoPedido
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order status.
        /// </summary>
        public int IdEstadoPedido { get; set; }

        /// <summary>
        /// Gets or sets the description of the order status (e.g., "Approved", "In-Transit", "Delivered").
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;
    }
}