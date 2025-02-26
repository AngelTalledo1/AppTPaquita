
namespace AppTransporte.Interfaces;

public partial class MenuCliente : ContentPage, IMenuPage
{
    public int _idUsuario;
    public int _idTipoUsuario;
    public MenuCliente(int idUsuario, int idTipoUsuario)
	{
        InitializeComponent();
        this._idUsuario = idUsuario;
        this._idTipoUsuario = idTipoUsuario;

    }

    public void setUserData(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;
        
    }
    private void NuevoPedido_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisSolicitudes(_idUsuario, _idTipoUsuario));
    }
    private void pedidosCliente_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisPedidos(_idUsuario, _idTipoUsuario));
    }

}