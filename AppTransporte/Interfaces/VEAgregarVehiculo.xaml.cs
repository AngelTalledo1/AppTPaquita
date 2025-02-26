using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEAgregarVehiculo : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    public VEAgregarVehiculo(int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        TituloVehiculo.Text = "Agregar Vehiculo";
        AgregarVehiculo.IsVisible = true;
        CancelarVehiculo.IsVisible = true;
    }
    
    public VEAgregarVehiculo(Vehiculo vehiculo, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        TituloVehiculo.Text = "Modificar Vehiculo";
        ModificarVehiculo.IsVisible = true;
        TipovehiculoPicker.SelectedIndex = 1;
        placaEntry.Text = vehiculo.Placa;
        modeloEntry.Text = vehiculo.Modelo;
        añofabricacionEntry.Text = vehiculo.AñoFabricacion;
        emipolizaEntry.Text = vehiculo.EmisionPoliza?.ToString("dd/MM/yyyy");
        venpolizaEntry.Text = vehiculo.VencimientoPoliza?.ToString("dd/MM/yyyy");
        emicitvEntry.Text = vehiculo.EmisionCITV?.ToString("dd/MM/yyyy");
        venCITVEntry.Text = vehiculo.VencimientoCITV?.ToString("dd/MM/yyyy");
        emicubEntry.Text = vehiculo.EmisionCubicacion?.ToString("dd/MM/yyyy");
        vencubEntry.Text = vehiculo.VencimientoCubicacion?.ToString("dd/MM/yyyy");

    }
    private void Btn_atrasAggVehiculo(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEVehiculos(_idUsuario,_idTipoUsuario));
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


