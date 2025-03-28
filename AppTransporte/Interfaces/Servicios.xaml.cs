namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;
public partial class Servicios : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    public Servicios(int idUsuario, int idTipoUsuario)
	{
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        InitializeComponent();
	}
    private void Btn_Atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal(_idUsuario, _idTipoUsuario));
    }
    private void Btn_aggserv(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AgregarServicio(_idUsuario, _idTipoUsuario));
    }
    private async void Editar_servicio(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var servicioSelect = button.CommandParameter as Servicio;

        if (servicioSelect != null)
        {
            await Navigation.PushAsync(new AgregarServicio(servicioSelect, _idUsuario, _idTipoUsuario));
        }
    }

}