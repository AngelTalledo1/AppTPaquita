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
            // Validar que todos los campos estén completos
            if (!ValidarFormulario())
                return;

            var button = sender as Button;
            button.IsEnabled = false;
            button.Text = "Asignando...";

            // Obtener los datos seleccionados
            var transportistaSeleccionado = transportistaPicker.SelectedItem as Trabajador;
            var ayudanteSeleccionado = ayudantePicker.SelectedItem as Trabajador;
            var cisternaSeleccionada = cisternaPicker.SelectedItem as Vehiculo;
            var tractoSeleccionado = tractoPicker.SelectedItem as Vehiculo;

            int cantidad = int.Parse(cantidadEntry.Text);

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
                await DisplayAlert("Éxito", resultado.Mensaje, "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", resultado.Mensaje, "OK");
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
                btn.Text = "Asignar";
            }
        }
    }

    private bool ValidarFormulario()
    {
        // Validar cantidad
        if (string.IsNullOrWhiteSpace(cantidadEntry.Text) || !int.TryParse(cantidadEntry.Text, out int cantidad))
        {
            DisplayAlert("Error", "Ingresa una cantidad válida", "OK");
            return false;
        }

        if (cantidad <= 0)
        {
            DisplayAlert("Error", "La cantidad debe ser mayor a 0", "OK");
            return false;
        }

        // NUEVA VALIDACIÓN: Verificar límite máximo
        if (cantidad > CANTIDAD_MAXIMA)
        {
            DisplayAlert("Error", "El límite máximo es de 200 barriles", "OK");
            return false;
        }

        // Validar transportista
        if (transportistaPicker.SelectedItem == null)
        {
            DisplayAlert("Error", "Selecciona un transportista", "OK");
            return false;
        }

        // Validar cisterna
        if (cisternaPicker.SelectedItem == null)
        {
            DisplayAlert("Error", "Selecciona una cisterna", "OK");
            return false;
        }

        // Validar tracto
        if (tractoPicker.SelectedItem == null)
        {
            DisplayAlert("Error", "Selecciona un tracto", "OK");
            return false;
        }

        // Ayudante es opcional
        return true;
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