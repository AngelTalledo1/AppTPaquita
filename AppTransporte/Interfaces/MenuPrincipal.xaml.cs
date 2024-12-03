using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class MenuPrincipal : ContentPage,IMenuPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    public MenuPrincipal()
	{
		InitializeComponent();
	}
    private void pedidos_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEpedidos(_idUsuario, _idTipoUsuario));
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

    public void setUserData(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;
    }
}