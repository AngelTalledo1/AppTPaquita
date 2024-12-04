namespace AppTransporte.Interfaces;
using AppTransporte.model;

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



    private async void btn_DetallesVehiculo(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var vehiculo = button.CommandParameter as Vehiculo;

        if (vehiculo != null)
        {
            await Navigation.PushAsync(new VEDetalleVehiculo(vehiculo));
        }
    }

    //private async void btn_DetallesVehiculo(object sender, EventArgs e)
    //{
    //    var button = (Button)sender;
    //    var vehiculo = button.CommandParameter as Vehiculo;

    //    if (vehiculo != null)
    //    {
    //        await Navigation.PushAsync(new VEDetalleVehiculo(vehiculo));
    //    }
    //}
}