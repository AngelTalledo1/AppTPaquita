using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEProcesoPedido : ContentPage
{

    
    public VEProcesoPedido(Pedido pedido)
    {
        InitializeComponent();
        var viajes = new VMViajes(pedido.IdPedido);
        BindingContext = viajes;
        TituloPedido.Text = $"Pedido {pedido.IdPedido}";
        Origen.Text = $"{pedido.Origen}";
        Cantidad.Text = $"222 / {pedido.Cantidad}";
        OrigenSector.Text = $"{pedido.OrigSector}";
        Estado.Text = $"{pedido.EstadoPedido}";
        Destino.Text = $"{pedido.Destino}";
        DestinoSector.Text = $"{pedido.DestSector}";
        NombreCliente.Text = $"Cliente: {pedido.Cliente}";
        ServiciosPedido.Text = $"Servicio: {pedido.Servicios}";
        cantidadTotalPedido.Text = $"Cantidad Pedido: {pedido.Cantidad} Barriles";
        creadorAdmin.Text = $"Creado por: {pedido.Usuario}";
        creacionPedido.Text = $"Pedido Creado: {pedido.FechaSolicitud}";
        cantidadViajes.Text = $"Numero de viajes: {pedido.Viajes}";


    }

    private void Btn_atrasEstado(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void expandir_Clicked(object sender, EventArgs e)
    {
        ExpanderViajes.IsExpanded = !ExpanderViajes.IsExpanded;
    }
    

    

    //public decimal CalcularCantidadFinalizada(Pedido pedido)
    //{
    //    decimal cantidadFinalizada = 0;

    //    foreach (var viaje in pedido.Viaje)
    //    {
    //         Obtener el último seguimiento (el más reciente) del viaje
    //        var ultimoSeguimiento = viaje.Seguimientos
    //            .OrderByDescending(s => s.FechaHora) // Ordenamos por FechaHora descendente
    //            .FirstOrDefault();

    //         Verificamos si el último estado es "Finalizado"
    //        if (ultimoSeguimiento != null && ultimoSeguimiento.Estado == "Finalizado")
    //        {
    //            cantidadFinalizada += viaje.Cantidad;  // Sumamos la cantidad de barriles del viaje
    //        }
    //    }

    //    return cantidadFinalizada;
    //}
}