using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEDetalleSolicitud : ContentPage
{
    
    public VEDetalleSolicitud(Solicitud solicitud)
	{
		InitializeComponent();
        BindingContext = solicitud;
    }

    private void Btn_atrasDetSolicitud(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    private async void Btn_CrearPedido(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud;

        if (solicitud != null)
        {

            await Navigation.PushAsync(new VECrearPedido(solicitud));
        }
    }

   
}