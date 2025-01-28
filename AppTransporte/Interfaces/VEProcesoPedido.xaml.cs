using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEProcesoPedido : ContentPage
{

    public VEProcesoPedido(Pedido pedido)
    {
        InitializeComponent();
        this.BindingContext = new VMViajes(pedido.IdPedido); 
        TituloPedido.Text = $"Pedido {pedido.IdPedido}";
        Origen.Text = $"{pedido.Origen}";
        Estado.Text = $"{pedido.EstadoPedido}";
        Destino.Text = $"{pedido.Destino}";
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

    private async void Btn_SeguimientoViajes(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var viaje = button.CommandParameter as Viaje;
        if (viaje != null)
        {
            // Validar si algún campo está sin asignar
            if (string.IsNullOrWhiteSpace(viaje.TractoAsig) || viaje.TractoAsig == "S/A" ||
                string.IsNullOrWhiteSpace(viaje.CisternaAsig) || viaje.CisternaAsig == "S/A" ||
                viaje.Cantidad <= 0 ||
                string.IsNullOrWhiteSpace(viaje.TrabajadoresAsig) || viaje.TrabajadoresAsig == "S/A")
            {
                // Navegar a la interfaz para asignar el viaje
                await Navigation.PushAsync(new VEAsignarViaje(viaje));
            }
            else
            {
                // Navegar a la interfaz de seguimiento del viaje
                await Navigation.PushAsync(new VESeguimientoViaje(viaje));
            }
        }
    }

}