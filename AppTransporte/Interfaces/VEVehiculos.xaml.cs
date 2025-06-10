using AppTransporte.model;
using AppTransporte.viewModel;
namespace AppTransporte.Interfaces;
public partial class VEVehiculos : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    private VMVehiculo _viewModel;
    public VEVehiculos(int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this._idUsuario = idUsuario;
        this._idTipoUsuario = idTipoUsuario;
        _viewModel = BindingContext as VMVehiculo;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Recargar vehículos al aparecer la página
        if (_viewModel != null)
        {
            await Task.Delay(500); // Pausa para asegurar que la BD se actualizó
            _viewModel.CargarVehiculos(); // Necesitarás este método en tu ViewModel
        }
    }
    private async void Btn_atrasVehiculo(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MenuPrincipal(_idUsuario, _idTipoUsuario));
    }
    private async void btn_agregarVehiculo(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEAgregarVehiculo(_idUsuario, _idTipoUsuario));
    }
    private async void btn_DetallesVehiculo(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var vehiculo = button.CommandParameter as Vehiculo;
        if (vehiculo != null)
        {
            await Navigation.PushAsync(new VEAgregarVehiculo(vehiculo, _idUsuario, _idTipoUsuario));
        }
    }
    private async void btn_EliminarVehiculo(object sender, EventArgs e)
    {
        var button = (Button)sender;
        try
        {
            var vehiculo = button.CommandParameter as Vehiculo;

            if (vehiculo == null)
            {
                await DisplayAlert("Error", "No se pudo obtener la información del vehículo", "OK");
                return;
            }

            // Determinar el tipo de vehículo basado en si tiene datos de cubicación
            string tipoVehiculo = (vehiculo.EmisionCubicacion.HasValue || vehiculo.VencimientoCubicacion.HasValue)
                ? "Cisterna"
                : "Tracto";

            // Deshabilitar el botón mientras se procesa
            button.IsEnabled = false;

            // PASO 1: Verificar dependencias primero
            try
            {
                var dependencias = await App.Database.VerDependenciasVehiculoAsync(vehiculo.IdVehiculo, tipoVehiculo);

                if (dependencias != null && dependencias.Count > 0)
                {
                    // Hay dependencias, mostrar opciones al usuario
                    bool eliminarConDependencias = await DisplayAlert(
                        "Vehículo en Uso",
                        $"El vehículo con placa {vehiculo.Placa} está siendo usado en viajes activos.\n\n" +
                        $"¿Qué desea hacer?\n\n" +
                        $"• CANCELAR: No eliminar nada\n" +
                        $"• FORZAR: Eliminar el vehículo y TODOS sus viajes asociados\n\n" +
                        $"ADVERTENCIA: Si selecciona FORZAR se eliminarán permanentemente:\n" +
                        $"- El vehículo\n" +
                        $"- Todos los viajes donde está asignado\n" +
                        $"- Las asignaciones de trabajadores\n" +
                        $"- Las evidencias del vehículo",
                        "FORZAR ELIMINACIÓN",
                        "Cancelar");

                    if (!eliminarConDependencias)
                    {
                        await DisplayAlert("Cancelado", "La eliminación fue cancelada", "OK");
                        return;
                    }

                    // Usuario confirmó forzar eliminación
                    await ForzarEliminacionVehiculo(vehiculo, tipoVehiculo);
                }
                else
                {
                    // No hay dependencias, eliminación normal
                    await EliminacionNormalVehiculo(vehiculo, tipoVehiculo);
                }
            }
            catch (Exception ex)
            {
                // Si falla la verificación de dependencias, intentar eliminación normal
                System.Diagnostics.Debug.WriteLine($"Error al verificar dependencias: {ex.Message}");
                await EliminacionNormalVehiculo(vehiculo, tipoVehiculo);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error inesperado:\n{ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"Error general en eliminación: {ex.Message}");
        }
        finally
        {
            // Rehabilitar el botón
            if (button != null)
            {
                button.IsEnabled = true;
            }
        }
    }

    // MÉTODO PARA ELIMINACIÓN NORMAL (SIN DEPENDENCIAS)
    private async Task EliminacionNormalVehiculo(Vehiculo vehiculo, string tipoVehiculo)
    {
        try
        {
            // Confirmar eliminación normal
            bool confirmar = await DisplayAlert(
                "Confirmar Eliminación",
                $"¿Está seguro que desea eliminar el vehículo con placa {vehiculo.Placa}?\n\n" +
                $"Tipo: {tipoVehiculo}\n" +
                $"Esta acción no se puede deshacer.",
                "Eliminar",
                "Cancelar");

            if (!confirmar)
                return;

            System.Diagnostics.Debug.WriteLine($"Eliminando vehículo (normal) - ID: {vehiculo.IdVehiculo}, Tipo: {tipoVehiculo}, Placa: {vehiculo.Placa}");

            // Llamar al método de eliminación normal
            var resultado = await App.Database.EliminarVehiculoAsync(vehiculo.IdVehiculo, tipoVehiculo);

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Vehículo eliminado correctamente", "OK");

                // Recargar la lista de vehículos
                if (_viewModel != null)
                {
                    _viewModel.CargarVehiculos();
                }
            }
            else
            {
                await DisplayAlert("Error",
                    "No se pudo eliminar el vehículo.\n\n" +
                    "Posibles causas:\n" +
                    "• El vehículo está asignado a viajes activos\n" +
                    "• Error de conexión a la base de datos\n" +
                    "• El vehículo ya fue eliminado", "OK");
            }
        }
        catch (Exception ex)
        {
            string mensajeError = ex.Message;

            // Personalizar mensaje según el tipo de error
            if (mensajeError.Contains("viaje"))
            {
                mensajeError = $"No se puede eliminar el vehículo porque está asignado a viajes activos.\n\n" +
                              $"Para eliminarlo debe:\n" +
                              $"1. Finalizar o eliminar los viajes donde está asignado\n" +
                              $"2. Intentar eliminar el vehículo nuevamente\n\n" +
                              $"O puede usar la opción 'Forzar Eliminación' para eliminar todo automáticamente.";
            }

            await DisplayAlert("? Error al Eliminar", mensajeError, "OK");
            System.Diagnostics.Debug.WriteLine($"Error en eliminación normal: {ex.Message}");
        }
    }

    // MÉTODO PARA FORZAR ELIMINACIÓN (CON DEPENDENCIAS)
    private async Task ForzarEliminacionVehiculo(Vehiculo vehiculo, string tipoVehiculo)
    {
        try
        {
            // Confirmación adicional para forzar
            bool confirmarForzar = await DisplayAlert(
                "CONFIRMACIÓN FINAL",
                $"ÚLTIMA ADVERTENCIA:\n\n" +
                $"Está a punto de eliminar PERMANENTEMENTE:\n" +
                $"• El vehículo {vehiculo.Placa}\n" +
                $"• TODOS los viajes donde está asignado\n" +
                $"• TODAS las asignaciones de trabajadores\n" +
                $"• TODAS las evidencias del vehículo\n\n" +
                $"Esta acción NO SE PUEDE DESHACER.\n\n" +
                $"¿Está completamente seguro?",
                "SÍ, ELIMINAR TODO",
                "NO, Cancelar");

            if (!confirmarForzar)
            {
                await DisplayAlert("Cancelado", "La eliminación forzada fue cancelada", "OK");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Forzando eliminación - ID: {vehiculo.IdVehiculo}, Tipo: {tipoVehiculo}, Placa: {vehiculo.Placa}");

            // Llamar al método de eliminación forzada
            var resultado = await App.Database.EliminarVehiculoForzarAsync(vehiculo.IdVehiculo, tipoVehiculo);

            if (resultado > 0)
            {
                await DisplayAlert("Eliminación Completa",
                    $"El vehículo {vehiculo.Placa} y todos sus datos asociados fueron eliminados correctamente.", "OK");

                // Recargar la lista de vehículos
                if (_viewModel != null)
                {
                    _viewModel.CargarVehiculos();
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo completar la eliminación forzada", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error en Eliminación Forzada",
                $"Ocurrió un error durante la eliminación forzada:\n\n{ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"Error en eliminación forzada: {ex.Message}");
        }
    }

}