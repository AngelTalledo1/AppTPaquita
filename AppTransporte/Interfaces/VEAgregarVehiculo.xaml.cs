using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEAgregarVehiculo : ContentPage
{
    public Vehiculo vehiculoSeleccionado { get; set; }
    public VEAgregarVehiculo()
    {
        InitializeComponent();
        TituloVehiculo.Text = "Agregar Vehiculo";
        AgregarVehiculo.IsVisible = true;
        CancelarVehiculo.IsVisible = true;
    }

    public VEAgregarVehiculo(Vehiculo vehiculo)
    {
        InitializeComponent();
        TituloVehiculo.Text = "Modificar Vehiculo";
        ModificarVehiculo.IsVisible = true;
        vehiculoSeleccionado = vehiculo;
        BindingContext = vehiculoSeleccionado;
        placaEntry.Text = vehiculoSeleccionado.Placa;
        modeloEntry.Text = vehiculoSeleccionado.Modelo;
        añofabricacionEntry.Text = vehiculoSeleccionado.AñoFabricacion;
        emipolizaEntry.Text = vehiculoSeleccionado.EmisionPoliza.ToString();

        venpolizaEntry.Text = vehiculoSeleccionado.VencimientoPoliza.ToString();
        emicitvEntry.Text = vehiculoSeleccionado.EmisionCITV.ToString();
        venCITVEntry.Text = vehiculoSeleccionado.EmisionCITV.ToString();
        venCITVEntry.Text = vehiculoSeleccionado.EmisionCITV.ToString();
        emicubEntry.Text = vehiculoSeleccionado.EmisionCubicacion.ToString();
        vencubEntry.Text = vehiculoSeleccionado.VencimientoCubicacion.ToString();

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Seleccionar el tipo de vehículo en el Picker
        if (vehiculoSeleccionado != null)
        {
            TipovehiculoPicker.SelectedItem = vehiculoSeleccionado.Tipo;
        }
    }
    private void Btn_atrasAggVehiculo(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void PickerIndexChanged(object sender, EventArgs e)
    {
        var selectedOption = TipovehiculoPicker.SelectedItem?.ToString();

        if (selectedOption == "Tracto")
        {
            tractoFields.IsVisible = true;
            cisternaFields.IsVisible = false;
        }
        else if (selectedOption == "Cisterna")
        {
            tractoFields.IsVisible = false;
            cisternaFields.IsVisible = true;
        }


    }

    private void AgregarVehiculo_Clicked(object sender, EventArgs e)
    {

    }

    private void ModificarVehiculo_Clicked(object sender, EventArgs e)
    {

    }

    private void EliminarVehiculo_Clicked(object sender, EventArgs e)
    {

    }
}


