using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VESolicitudes : ContentPage
{
	public VESolicitudes()
	{
		InitializeComponent();
	}

    private void Btn_atrasSolicitudes(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    //private void btn_DetSolicitud(object sender, EventArgs e)
    //{
    //    Navigation.PushAsync(new VEDetalleSolicitud(solicitud));
    //}

    private async void OnPedidoSelected(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud; // Asegúrate de que "Pedidos" es el tipo de tu modelo de datos

        if (solicitud != null)
        {
            // Navegar a la página de VEProcesoPedido, pasando los datos del pedido seleccionado
            await Navigation.PushAsync(new VEDetalleSolicitud(solicitud));
        }
    }
}