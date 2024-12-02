namespace AppTransporte.Interfaces;

using AppTransporte.model;
using AppTransporte.viewModel;

public partial class VEpedidos : ContentPage
{
	public VEpedidos()
	{
		InitializeComponent();
        BindingContext = new VMPedidos();
    }

    private void Btn_atrasPedidos(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void OnPedidoSelected(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var pedido = button.CommandParameter as Pedido; // Asegúrate de que "Pedidos" es el tipo de tu modelo de datos

        if (pedido != null)
        {
            // Navegar a la página de VEProcesoPedido, pasando los datos del pedido seleccionado
            await Navigation.PushAsync(new VEProcesoPedido());
        }
    }

}