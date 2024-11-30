namespace AppTransporte.Interfaces;

public partial class VEDetalleSolicitud : ContentPage
{
	public VEDetalleSolicitud()
	{
		InitializeComponent();
	}

    private void Btn_atrasDetSolicitud(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }
}