using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEAgregarVehiculo : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    private Vehiculo _vehiculoAModificar;

    public VEAgregarVehiculo(int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this._idUsuario = idUsuario;
        this._idTipoUsuario = idTipoUsuario;
        TituloVehiculo.Text = "Agregar Vehiculo";
        AgregarVehiculo.IsVisible = true;
        CancelarVehiculo.IsVisible = true;

        // Inicializar DatePickers con fecha actual
        InicializarDatePickers();
    }

    public VEAgregarVehiculo(Vehiculo vehiculo, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this._idUsuario = idUsuario;
        this._idTipoUsuario = idTipoUsuario;
        this._vehiculoAModificar = vehiculo;

        TituloVehiculo.Text = "Modificar Vehiculo";
        ModificarVehiculo.IsVisible = true;
        CancelarVehiculo.IsVisible = true;

        // Inicializar DatePickers
        InicializarDatePickers();

        // Cargar datos del vehículo
        CargarDatosVehiculo(vehiculo);
    }

    private void InicializarDatePickers()
    {
        var fechaActual = DateTime.Today;

        emipolizaDatePicker.Date = fechaActual;
        venpolizaDatePicker.Date = fechaActual;
        emicitvDatePicker.Date = fechaActual;
        venCITVDatePicker.Date = fechaActual;
        emicubDatePicker.Date = fechaActual;
        vencubDatePicker.Date = fechaActual;
    }

    private void CargarDatosVehiculo(Vehiculo vehiculo)
    {
        // Determinar tipo de vehículo
        if (vehiculo.EmisionCubicacion.HasValue || vehiculo.VencimientoCubicacion.HasValue)
        {
            TipovehiculoPicker.SelectedItem = "Cisterna";
            cisternaFields.IsVisible = true;
            tractoFields.IsVisible = false;
        }
        else
        {
            TipovehiculoPicker.SelectedItem = "Tracto";
            tractoFields.IsVisible = true;
            cisternaFields.IsVisible = false;
        }

        // Cargar datos básicos
        placaEntry.Text = vehiculo.Placa;
        modeloEntry.Text = vehiculo.Modelo;
        añofabricacionEntry.Text = vehiculo.AñoFabricacion;

        // Cargar fechas en DatePickers
        if (vehiculo.EmisionPoliza.HasValue)
            emipolizaDatePicker.Date = vehiculo.EmisionPoliza.Value;

        if (vehiculo.VencimientoPoliza.HasValue)
            venpolizaDatePicker.Date = vehiculo.VencimientoPoliza.Value;

        if (vehiculo.EmisionCITV.HasValue)
            emicitvDatePicker.Date = vehiculo.EmisionCITV.Value;

        if (vehiculo.VencimientoCITV.HasValue)
            venCITVDatePicker.Date = vehiculo.VencimientoCITV.Value;

        if (vehiculo.EmisionCubicacion.HasValue)
            emicubDatePicker.Date = vehiculo.EmisionCubicacion.Value;

        if (vehiculo.VencimientoCubicacion.HasValue)
            vencubDatePicker.Date = vehiculo.VencimientoCubicacion.Value;
    }

    private async void Btn_atrasAggVehiculo(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEVehiculos(_idUsuario, _idTipoUsuario));
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

    private async void AgregarVehiculo_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!ValidarFormulario())
                return;

            var button = sender as Button;
            button.IsEnabled = false;
            button.Text = "Agregando...";

            var tipoVehiculo = TipovehiculoPicker.SelectedItem?.ToString();
            var datosVehiculo = ObtenerDatosFormulario();

            var resultado = await App.Database.AgregarVehiculo(
                datosVehiculo.Placa,
                datosVehiculo.Modelo,
                datosVehiculo.AñoFabricacion,
                datosVehiculo.EmisionPoliza,
                datosVehiculo.VencimientoPoliza,
                datosVehiculo.EmisionCITV,
                datosVehiculo.VencimientoCITV,
                datosVehiculo.EmisionCubicacion,
                datosVehiculo.VencimientoCubicacion,
                new byte[0], // imagen vacía
                new byte[0], // poliza vacía
                new byte[0], // citv vacío
                new byte[0], // cubicacion vacía
                new byte[0], // tarjeta propiedad vacía
                tipoVehiculo
            );

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Vehículo agregado correctamente", "OK");
                await Navigation.PushAsync(new VEVehiculos(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar el vehículo", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
        finally
        {
            if (sender is Button btn)
            {
                btn.IsEnabled = true;
                btn.Text = "Agregar";
            }
        }
    }

    private async void ModificarVehiculo_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!ValidarFormulario())
                return;

            var button = sender as Button;
            button.IsEnabled = false;
            button.Text = "Modificando...";

            var tipoVehiculo = TipovehiculoPicker.SelectedItem?.ToString();
            var datosVehiculo = ObtenerDatosFormulario();

            var resultado = await App.Database.ModificarVehiculoAsync(
                _vehiculoAModificar.IdVehiculo,
                datosVehiculo.Placa,
                datosVehiculo.Modelo,
                datosVehiculo.AñoFabricacion,
                datosVehiculo.EmisionPoliza,
                datosVehiculo.VencimientoPoliza,
                datosVehiculo.EmisionCITV,
                datosVehiculo.VencimientoCITV,
                datosVehiculo.EmisionCubicacion,
                datosVehiculo.VencimientoCubicacion,
                tipoVehiculo
            );

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Vehículo modificado correctamente", "OK");
                await Navigation.PushAsync(new VEVehiculos(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo modificar el vehículo", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
        finally
        {
            if (sender is Button btn)
            {
                btn.IsEnabled = true;
                btn.Text = "Modificar";
            }
        }
    }

    private bool ValidarFormulario()
    {
        // Validar tipo de vehículo
        if (TipovehiculoPicker.SelectedItem == null)
        {
            DisplayAlert("Error", "Selecciona el tipo de vehículo", "OK");
            return false;
        }

        // Validar placa
        if (string.IsNullOrWhiteSpace(placaEntry.Text))
        {
            DisplayAlert("Error", "La placa es obligatoria", "OK");
            return false;
        }

        // Validar año de fabricación
        if (!string.IsNullOrWhiteSpace(añofabricacionEntry.Text))
        {
            if (!int.TryParse(añofabricacionEntry.Text, out int año) || año < 1900 || año > DateTime.Now.Year + 1)
            {
                DisplayAlert("Error", "Ingresa un año de fabricación válido", "OK");
                return false;
            }
        }

        return true;
    }

    private (string Placa, string Modelo, string AñoFabricacion, DateTime? EmisionPoliza,
             DateTime? VencimientoPoliza, DateTime? EmisionCITV, DateTime? VencimientoCITV,
             DateTime? EmisionCubicacion, DateTime? VencimientoCubicacion) ObtenerDatosFormulario()
    {
        return (
            Placa: placaEntry.Text?.Trim(),
            Modelo: modeloEntry.Text?.Trim(),
            AñoFabricacion: añofabricacionEntry.Text?.Trim(),
            EmisionPoliza: emipolizaDatePicker.Date,
            VencimientoPoliza: venpolizaDatePicker.Date,
            EmisionCITV: emicitvDatePicker.Date,
            VencimientoCITV: venCITVDatePicker.Date,
            EmisionCubicacion: emicubDatePicker.Date,
            VencimientoCubicacion: vencubDatePicker.Date
        );
    }
}