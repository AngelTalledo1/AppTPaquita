
namespace AppTransporte.Interfaces;

public partial class MenuCliente : ContentPage, IMenuPage
{
    private int _idUsuario;
    private int _idtipousuario;
    public MenuCliente()
	{
        InitializeComponent();
	}

    public void setUserData(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idtipousuario = idTipoUsuario;
    }
        
    private void Btn_atrasMenuCliente(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    
        private void NuevoPedido_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisSolicitudes());
    }
    private void pedidosCliente_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEpedidos(_idUsuario,_idtipousuario));
    }

}