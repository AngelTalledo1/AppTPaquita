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
}