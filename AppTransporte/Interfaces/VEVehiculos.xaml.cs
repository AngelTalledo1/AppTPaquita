namespace AppTransporte.Interfaces;

public partial class VEVehiculos : ContentPage
{
	public VEVehiculos()
	{
		InitializeComponent();
	}

    private void Btn_atrasVehiculo(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    private void btn_agregarVehiculo(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEAgregarVehiculo());
    }
}