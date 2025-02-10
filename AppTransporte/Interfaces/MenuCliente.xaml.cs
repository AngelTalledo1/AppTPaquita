
namespace AppTransporte.Interfaces;

public partial class MenuCliente : ContentPage, IMenuPage
{
    public int _idUsuario;
    public int _idtipousuario;
    public MenuCliente()
	{
        InitializeComponent();
        
    }

    public void setUserData(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idtipousuario = idTipoUsuario;
        
    }
    private void NuevoPedido_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisSolicitudes(_idUsuario));
    }
    private void pedidosCliente_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisPedidos(_idUsuario,_idtipousuario));
    }

}