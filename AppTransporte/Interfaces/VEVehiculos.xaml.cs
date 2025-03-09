namespace AppTransporte.Interfaces;
using AppTransporte.model;

public partial class VEVehiculos : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    public VEVehiculos(int idUsuario, int idTipoUsuario)
	{
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        InitializeComponent();
	}

    private void Btn_atrasVehiculo(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal(_idUsuario, _idTipoUsuario));
    }

    private void btn_agregarVehiculo(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEAgregarVehiculo(_idUsuario, _idTipoUsuario));
    }



    private async void btn_DetallesVehiculo(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var vehiculo = button.CommandParameter as Vehiculo;

        if (vehiculo != null)
        {
            await Navigation.PushAsync(new VEDetalleVehiculo(vehiculo, _idUsuario, _idTipoUsuario));
        }
    }

    
}