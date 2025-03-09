using AppTransporte.model;
using AppTransporte.viewModel;
namespace AppTransporte.Interfaces;

public partial class VEDetalleVehiculo : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    public Vehiculo Vehiculoseleccionado { get; set; } = new();
    public VEDetalleVehiculo(int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
    }
    public VEDetalleVehiculo(Vehiculo vehiculo, int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        Vehiculoseleccionado = vehiculo;
        BindingContext = new VMVehiculo();
        MostrarPlaca.Text = vehiculo.Placa;
        MostrarModelo.Text = vehiculo.Modelo;
        MostrarAño.Text = vehiculo.AñoFabricacion;
        MostrarEmisionPoliza.Text = vehiculo.EmisionPoliza?.ToString("dd, MMMM yyyy");
        MostrarVencimientoPoliza.Text = vehiculo.VencimientoPoliza?.ToString("dd, MMMM yyyy");
        MostrarEmisionCITV.Text = vehiculo.EmisionCITV?.ToString("dd, MMMM yyyy");
        MostrarVencimientoCITV.Text = vehiculo.VencimientoCITV?.ToString("dd, MMMM yyyy");
        MostrarEmisionCubicacion.Text = vehiculo.EmisionCubicacion?.ToString("dd, MMMM yyyy");
        MostrarVencimientoCubicacion.Text = vehiculo.VencimientoCubicacion?.ToString("dd, MMMM yyyy");
    }

    private void Btn_atrasDetVehiculo(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEVehiculos(_idUsuario,_idTipoUsuario));
    }


    private void Btn_EliminarVehiculo(object sender, EventArgs e)
    {
        
    }

    private async void Btn_ModificarVehiculo(object sender, EventArgs e)
    {
        var button = (Button)sender;
        await Navigation.PushAsync(new VEAgregarVehiculo(Vehiculoseleccionado, _idUsuario, _idTipoUsuario));
       
    }
}