using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEDetalleVehiculo : ContentPage
{

    public VEDetalleVehiculo()
    {
        InitializeComponent();
    }
    public VEDetalleVehiculo(Vehiculo vehiculo)
	{
		InitializeComponent();
        BindingContext = vehiculo;
	}

    private void Btn_atrasDetVehiculo(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    private async void Btn_ModificarVehiculo(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var vehiculo = button.CommandParameter as Vehiculo;

        if (vehiculo != null)
        {
            await Navigation.PushAsync(new VEAgregarVehiculo(vehiculo));
        }
    }

    private void Btn_EliminarVehiculo(object sender, EventArgs e)
    {

    }
}