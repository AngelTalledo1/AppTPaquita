using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEAgregarVehiculo : ContentPage
{
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
        TipovehiculoPicker.SelectedIndex = 1;
        placaEntry.Text = vehiculo.Placa;
        modeloEntry.Text = vehiculo.Modelo;
        añofabricacionEntry.Text = vehiculo.AñoFabricacion;
        emipolizaEntry.Text = vehiculo.EmisionPoliza.ToString();
        venpolizaEntry.Text = vehiculo.VencimientoPoliza.ToString();
        emicitvEntry.Text = vehiculo.EmisionCITV.ToString();
        venCITVEntry.Text = vehiculo.EmisionCITV.ToString();
        venCITVEntry.Text = vehiculo.EmisionCITV.ToString();
        emicubEntry.Text = vehiculo.EmisionCubicacion.ToString();
        vencubEntry.Text = vehiculo.VencimientoCubicacion.ToString();

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


