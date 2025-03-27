namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;
public partial class AgregarServicio : ContentPage
{

    private int _idUsuario;
    private int _idTipoUsuario;
    public AgregarServicio(int idUsuario, int idTipoUsuario)
	{
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        InitializeComponent();
	}
    private void Btn_atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Servicios(_idUsuario, _idTipoUsuario));
    }
    private void btn_cancelar(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Servicios(_idUsuario, _idTipoUsuario));
    }
}