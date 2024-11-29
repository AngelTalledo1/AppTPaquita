using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class MenuPrincipal : ContentPage
{
	public MenuPrincipal()
	{
		InitializeComponent();
	}
    private void pedidos_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEpedidos());
    }

    private void cliente_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEclientes());
    }

    private void Btn_atrasMenuPrincipal(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void transportista_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEtransportistas());
    }

    private void btn_Vehiculos(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEVehiculos());
    }
}