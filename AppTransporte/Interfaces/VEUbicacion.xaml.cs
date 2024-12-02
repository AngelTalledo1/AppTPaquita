namespace AppTransporte.Interfaces;
using AppTransporte.model;

public partial class VEUbicacion : ContentPage
{
	public VEUbicacion()
	{
		InitializeComponent();
	}

    private void Btn_AtrasUbicacion(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    private void Btn_AggUbicacion(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEAgregarUbicacion());
    }

    private async void Btn_ModifUbicacion(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var ubicacion = button.CommandParameter as Ubicacion;

        if (ubicacion != null)
        {

            await Navigation.PushAsync(new VEAgregarUbicacion(ubicacion));
        }
    }
}