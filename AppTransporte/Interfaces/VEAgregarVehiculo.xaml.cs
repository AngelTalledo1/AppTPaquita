namespace AppTransporte.Interfaces;

public partial class VEAgregarVehiculo : ContentPage
{
	public VEAgregarVehiculo()
	{
		InitializeComponent();
	}

    private void Btn_aggVehiculo(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }
}