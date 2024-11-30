namespace AppTransporte.Interfaces;

public partial class VEDetalleVehiculo : ContentPage
{
	public VEDetalleVehiculo()
	{
		InitializeComponent();
	}

    private void Btn_atrasDetVehiculo(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }
}