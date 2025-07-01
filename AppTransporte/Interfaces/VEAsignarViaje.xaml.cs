using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEAsignarViaje : ContentPage
{
    private int idUsuario;
    private int idtipousuario;
    private Pedido _pedido;
    private Viaje _viaje;
    private VMAsignarViaje _viewModel;
    private const int CANTIDAD_MAXIMA = 200; // Constante para el límite

    public VEAsignarViaje(Viaje viaje, Pedido pedido, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();

        this.idUsuario = idUsuario;
        this.idtipousuario = idTipoUsuario;
        this._pedido = pedido;
        this._viaje = viaje;

        _viewModel = new VMAsignarViaje();
        BindingContext = _viewModel;

        Id_Pedido.Text = $"ID Pedido: {viaje.IdPedido}";
        Id_viaje.Text = $"ID Viaje: {viaje.IdViaje}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel != null)
        {
            await _viewModel.CargarDatosAsync();
        }
    }

    // NUEVO MÉTODO: Validar cantidad en tiempo real
    private void CantidadEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;

        if (int.TryParse(entry.Text, out int cantidad))
        {
            if (cantidad > CANTIDAD_MAXIMA)
            {
                // Mostrar mensaje de error
                lblErrorCantidad.IsVisible = true;
                entry.TextColor = Colors.Red;
            }
            else
            {
                // Ocultar mensaje de error
                lblErrorCantidad.IsVisible = false;
                entry.TextColor = Colors.Black;
            }
        }
        else if (!string.IsNullOrEmpty(entry.Text))
        {
            // Si no es un número válido, mostrar error
            lblErrorCantidad.Text = "Ingresa un número válido";
            lblErrorCantidad.IsVisible = true;
            entry.TextColor = Colors.Red;
        }
        else
        {
            // Campo vacío, ocultar error
            lblErrorCantidad.IsVisible = false;
            entry.TextColor = Colors.Black;
        }
    }

    private async void Btn_AsignarViaje(object sender, EventArgs e)
    {
        try
        {
            // Validar que todos los campos estén completos y recursos disponibles
            if (!await ValidarFormulario())
                return;

            var button = sender as Button;
            button.IsEnabled = false;
            button.Text = "Verificando disponibilidad...";

            // Obtener los datos seleccionados
            var transportistaSeleccionado = transportistaPicker.SelectedItem as Trabajador;
            var ayudanteSeleccionado = ayudantePicker.SelectedItem as Trabajador;
            var cisternaSeleccionada = cisternaPicker.SelectedItem as Vehiculo;
            var tractoSeleccionado = tractoPicker.SelectedItem as Vehiculo;

            int cantidad = int.Parse(cantidadEntry.Text);

            button.Text = "Asignando viaje...";

            // Llamar al método para asignar en la BD
            var resultado = await App.Database.AsignarViajeAsync(
                _viaje.IdViaje,
                tractoSeleccionado?.IdVehiculo,
                cisternaSeleccionada?.IdVehiculo,
                cantidad,
                transportistaSeleccionado?.IdTrabajador,
                ayudanteSeleccionado?.IdTrabajador
            );

            if (resultado.Resultado > 0)
            {
                await DisplayAlert("Éxito",
                    $"Viaje asignado correctamente.\n\n{resultado.Mensaje}", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error",
                    $"No se pudo asignar el viaje.\n\n{resultado.Mensaje}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error inesperado: {ex.Message}", "OK");
        }
        finally
        {
            if (sender is Button btn)
            {
                btn.IsEnabled = true;
                btn.Text = "Asignar";
            }
        }
    }

    private async Task<bool> ValidarFormulario()

    {
        {
            // Validar cantidad
            if (string.IsNullOrWhiteSpace(cantidadEntry.Text) || !int.TryParse(cantidadEntry.Text, out int cantidad))
            {
                await DisplayAlert("Error", "Ingresa una cantidad válida", "OK");
                return false;
            }

            if (cantidad <= 0)
            {
                await DisplayAlert("Error", "La cantidad debe ser mayor a 0", "OK");
                return false;
            }

            // NUEVA VALIDACIÓN: Verificar límite máximo
            if (cantidad > CANTIDAD_MAXIMA)
            {
                await DisplayAlert("Error", $"El límite máximo es de {CANTIDAD_MAXIMA} barriles", "OK");
                return false;
            }

            // Validar transportista
            if (transportistaPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Selecciona un transportista", "OK");
                return false;
            }

            // Validar cisterna
            if (cisternaPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Selecciona una cisterna", "OK");
                return false;
            }

            // Validar tracto
            if (tractoPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Selecciona un tracto", "OK");
                return false;
            }

        

            var transportistaSeleccionado = transportistaPicker.SelectedItem as Trabajador;
            var ayudanteSeleccionado = ayudantePicker.SelectedItem as Trabajador;
            var cisternaSeleccionada = cisternaPicker.SelectedItem as Vehiculo;
            var tractoSeleccionado = tractoPicker.SelectedItem as Vehiculo;

            // Verificar recursos en viaje usando el método optimizado
            var verificacionesRecursos = await App.Database.VerificarRecursosEnViajeAsync(
                idTransportista: transportistaSeleccionado?.IdTrabajador,
                idAyudante: ayudanteSeleccionado?.IdTrabajador,
                idCisterna: cisternaSeleccionada?.IdVehiculo,
                idTracto: tractoSeleccionado?.IdVehiculo
            );

            // Validar transportista
            if (verificacionesRecursos.ContainsKey("Transportista") && verificacionesRecursos["Transportista"].EnViaje)
            {
                string nombreTransportista = $"{transportistaSeleccionado?.Nombre} {transportistaSeleccionado?.apePaterno}".Trim();
                await DisplayAlert("Recurso No Disponible",
                    $"No se puede asignar el transportista {nombreTransportista}.\n\n" +
                    $"Motivo: {verificacionesRecursos["Transportista"].MensajeDetalle}\n\n" +
                    "Por favor, selecciona otro transportista disponible.", "OK");
                return false;
            }

            // Validar ayudante (si está seleccionado)
            if (verificacionesRecursos.ContainsKey("Ayudante") && verificacionesRecursos["Ayudante"].EnViaje)
            {
                string nombreAyudante = $"{ayudanteSeleccionado?.Nombre} {ayudanteSeleccionado?.apePaterno}".Trim();
                await DisplayAlert("Recurso No Disponible",
                    $"No se puede asignar el ayudante {nombreAyudante}.\n\n" +
                    $"Motivo: {verificacionesRecursos["Ayudante"].MensajeDetalle}\n\n" +
                    "Por favor, selecciona otro ayudante disponible o continúa sin ayudante.", "OK");
                return false;
            }

            // Validar cisterna
            if (verificacionesRecursos.ContainsKey("Cisterna") && verificacionesRecursos["Cisterna"].EnViaje)
            {
                await DisplayAlert("? Recurso No Disponible",
                    $"No se puede asignar la cisterna {cisternaSeleccionada?.Placa}.\n\n" +
                    $"Motivo: {verificacionesRecursos["Cisterna"].MensajeDetalle}\n\n" +
                    "Por favor, selecciona otra cisterna disponible.", "OK");
                return false;
            }

            // Validar tracto
            if (verificacionesRecursos.ContainsKey("Tracto") && verificacionesRecursos["Tracto"].EnViaje)
            {
                await DisplayAlert("Recurso No Disponible",
                    $"No se puede asignar el tracto {tractoSeleccionado?.Placa}.\n\n" +
                    $"Motivo: {verificacionesRecursos["Tracto"].MensajeDetalle}\n\n" +
                    "Por favor, selecciona otro tracto disponible.", "OK");
                return false;
            }

            // Si llegamos aquí, todos los recursos están disponibles
            return true;
        }
    }
    private async void TransportistaPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (transportistaPicker.SelectedItem is Trabajador transportista)
        {
            await VerificarDisponibilidadRecurso("transportista", transportista.IdTrabajador,
                $"{transportista.Nombre} {transportista.apePaterno}".Trim());
        }
    }

    private async void AyudantePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ayudantePicker.SelectedItem is Trabajador ayudante)
        {
            await VerificarDisponibilidadRecurso("ayudante", ayudante.IdTrabajador,
                $"{ayudante.Nombre} {ayudante.apePaterno}".Trim());
        }
    }

    private async void CisternaPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cisternaPicker.SelectedItem is Vehiculo cisterna)
        {
            await VerificarDisponibilidadRecurso("cisterna", cisterna.IdVehiculo, cisterna.Placa);
        }
    }

    private async void TractoPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (tractoPicker.SelectedItem is Vehiculo tracto)
        {
            await VerificarDisponibilidadRecurso("tracto", tracto.IdVehiculo, tracto.Placa);
        }
    }
    private async Task VerificarDisponibilidadRecurso(string tipoRecurso, int idRecurso, string nombreRecurso)
    {
        try
        {
            (bool enViaje, string mensajeDetalle) resultado;

            switch (tipoRecurso.ToLower())
            {
                case "transportista":
                    resultado = await App.Database.VerificarTrabajadorEnViajeAsync(idRecurso);
                    break;
                case "ayudante":
                    resultado = await App.Database.VerificarTrabajadorEnViajeAsync(idRecurso);
                    break;
                case "cisterna":
                    resultado = await App.Database.VerificarCisternaEnViajeAsync(idRecurso);
                    break;
                case "tracto":
                    resultado = await App.Database.VerificarTractoEnViajeAsync(idRecurso);
                    break;
                default:
                    return;
            }

            if (resultado.enViaje)
            {
                // Mostrar alerta inmediata cuando el recurso no está disponible
                var respuesta = await DisplayAlert(
                    "Recurso No Disponible",
                    $"El {tipoRecurso} seleccionado ({nombreRecurso}) no está disponible.\n\n" +
                    $"Motivo: {resultado.mensajeDetalle}\n\n" +
                    "¿Deseas seleccionar otro recurso?",
                    "Sí, cambiar", "Mantener selección");

                if (respuesta)
                {
                    // Limpiar la selección para que el usuario elija otro
                    switch (tipoRecurso.ToLower())
                    {
                        case "transportista":
                            transportistaPicker.SelectedIndex = -1;
                            break;
                        case "ayudante":
                            ayudantePicker.SelectedIndex = -1;
                            break;
                        case "cisterna":
                            cisternaPicker.SelectedIndex = -1;
                            break;
                        case "tracto":
                            tractoPicker.SelectedIndex = -1;
                            break;
                    }
                }
            }
            else
            {
                // Recurso disponible - podrías mostrar un indicador visual opcional
                // Por ejemplo, cambiar el color del picker o mostrar un icono verde
                await DisplayAlert("Recurso Disponible",
                    $"El {tipoRecurso} {nombreRecurso} está disponible para asignación.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al verificar disponibilidad: {ex.Message}", "OK");
        }
    }


    private async void Cancelar_btn(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void Btn_atrasAsignarViaje(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al navegar: {ex.Message}", "OK");
        }
    }
}