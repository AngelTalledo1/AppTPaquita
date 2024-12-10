using AppTransporte.model;
using AppTransporte.viewModel;
namespace AppTransporte.Interfaces;

public partial class VEDetalleVehiculo : ContentPage
{
    public Vehiculo Vehiculoseleccionado { get; set; } = new();
    public VEDetalleVehiculo()
    {
        InitializeComponent();
    }
    public VEDetalleVehiculo(Vehiculo vehiculo)
	{
		InitializeComponent();
        Vehiculoseleccionado = vehiculo;
        BindingContext = new VMVehiculo();
        MostrarPlaca.Text = vehiculo.Placa;
        MostrarModelo.Text = vehiculo.Modelo;
        MostrarAño.Text = vehiculo.AñoFabricacion;
        MostrarEmisionPoliza.Text = vehiculo.EmisionPoliza.ToString();
        MostrarVencimientoPoliza.Text = vehiculo.VencimientoPoliza.ToString();
        MostrarEmisionCITV.Text = vehiculo.EmisionCITV.ToString();
        MostrarVencimientoCITV.Text = vehiculo.VencimientoCITV.ToString();
        MostrarEmisionCubicacion.Text = vehiculo.EmisionCubicacion.ToString();
        MostrarVencimientoCubicacion.Text = vehiculo.VencimientoCubicacion.ToString();
    }

    private void Btn_atrasDetVehiculo(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }


    private void Btn_EliminarVehiculo(object sender, EventArgs e)
    {
        
    }

    private async void Btn_ModificarVehiculo(object sender, EventArgs e)
    {
        var button = (Button)sender;
        await Navigation.PushAsync(new VEAgregarVehiculo(Vehiculoseleccionado));
       
    }
}