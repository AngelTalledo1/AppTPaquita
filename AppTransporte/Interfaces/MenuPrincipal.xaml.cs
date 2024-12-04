using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class MenuPrincipal : ContentPage
{
    private int _idUsuario;
    private int _idtipousuario;
    public MenuPrincipal()
	{
		InitializeComponent();
	}
    public void setUserData(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idtipousuario = idTipoUsuario;
    }
    private async void pedidos_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEpedidos(_idUsuario, _idtipousuario));
    }

    private async void cliente_Clicked(object sender, EventArgs e)
    {
       await Navigation.PushAsync(new VEclientes());
    }

    private void Btn_atrasMenuPrincipal(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void transportista_Clicked(object sender, EventArgs e)
    {
       await Navigation.PushAsync(new VEtransportistas());
    }

    private async void btn_Vehiculos(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEVehiculos());
    }

    private async void btn_Solicitudes(object sender, EventArgs e)
    {
       await Navigation.PushAsync(new VESolicitudes());
    }

    private async void Btn_DestinosClicket(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEUbicacion());
    }
}
