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
}