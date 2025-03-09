namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;

public partial class VEUbicacion : ContentPage
{
    private readonly VMUbicacion _viewModel;
    private int _idUsuario;
    private int _idTipoUsuario;
    public VEUbicacion(int idUsuario, int idTipoUsuario)
    {
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        InitializeComponent();
        _viewModel = new VMUbicacion();
        BindingContext = _viewModel;

    }

    private void Btn_AtrasUbicacion(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal(_idUsuario,_idTipoUsuario));
    }

    private void Btn_AggUbicacion(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEAgregarUbicacion(_idUsuario, _idTipoUsuario));
    }

    private async void Btn_ModifUbicacion(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var ubicacion = button.CommandParameter as Ubicacion;

        if (ubicacion != null)
        {

            await Navigation.PushAsync(new VEAgregarUbicacion(ubicacion, _idUsuario, _idTipoUsuario));
        }
    }
    private async void OnMapButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        string coords = button.Text;
        string uri = $"geo:{coords}?q={coords}";
        await Launcher.OpenAsync(uri);
    }
}
