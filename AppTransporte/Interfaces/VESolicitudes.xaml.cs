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

    private void btn_DetSolicitud(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEDetalleSolicitud());
    }
}