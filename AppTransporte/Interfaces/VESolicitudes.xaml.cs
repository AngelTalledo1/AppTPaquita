using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VESolicitudes : ContentPage
{
	public VESolicitudes()
	{

        BindingContext = new VMmisSolicitudes();
        InitializeComponent();
    }

    private void Btn_atrasSolicitudes(object sender, EventArgs e)
    {
		Navigation.PushAsync(new MenuPrincipal());
    }


    private async void SolicitudSelected_detalle(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud; 

        if (solicitud != null)
        {
            var pedidosViewModel = this.BindingContext as VMPedidos;
            await Navigation.PushAsync(new VEDetalleSolicitud(solicitud, pedidosViewModel));
        }
    }

    
}