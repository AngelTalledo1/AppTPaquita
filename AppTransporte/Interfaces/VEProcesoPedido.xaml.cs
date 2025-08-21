using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEProcesoPedido : ContentPage
{
    private int idUsuario;
    private int idtipousuario;
    private Pedido _pedido;
    private VMSeguimientoViaje _viaje;
    private VMViajes _viewModelViajes;

    public VEProcesoPedido(Pedido pedido, int idUsuario, int idTipoUsuario, VMSeguimientoViaje? viaje)
    {
        InitializeComponent();
        this.idUsuario = idUsuario;
        this.idtipousuario = idTipoUsuario;
        this._pedido = pedido;
        this._viaje = viaje;

        // Crear y asignar el ViewModel
        if (idTipoUsuario == 1 || idTipoUsuario == 2)
        {
            _viewModelViajes = new VMViajes(pedido.IdPedido);
        }
        else if (idTipoUsuario == 3)
        {
            _viewModelViajes = new VMViajes(idUsuario: idUsuario);
        }

        this.BindingContext = _viewModelViajes;

        // Configurar labels iniciales
        InicializarLabels();
    }

    private void InicializarLabels()
    {
        TituloPedido.Text = $"Pedido {_pedido.IdPedido}";
        Origen.Text = $"{_pedido.Origen}";
        Destino.Text = $"{_pedido.Destino}";
        NombreCliente.Text = $"Cliente: {_pedido.Cliente}";
        ServiciosPedido.Text = $"Servicio: {_pedido.Servicios}";
        cantidadTotalPedido.Text = $"Cantidad Pedido: {_pedido.Cantidad} Barriles";
        creadorAdmin.Text = $"Creado por: {_pedido.Usuario}";
        creacionPedido.Text = $"Pedido Creado: {_pedido.FechaSolicitud}";
        cantidadViajes.Text = $"Numero de viajes: {_pedido.Viajes}";

        // Estado inicial
        Estado.Text = $"{_pedido.EstadoPedido}";
    }

    // Método para actualizar el estado del pedido
    // Método mejorado para actualizar el estado del pedido
    private async Task ActualizarEstadoPedidoAsync()
    {
        try
        {
            var estadoActualizado = await App.Database.ObtenerEstadoActualPedidoAsync(_pedido.IdPedido);

            // Actualizar en el hilo principal
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Estado.Text = estadoActualizado;

                // Cambiar color según el estado con mejor mapeo
                var colorEstado = estadoActualizado.ToLower() switch
                {
                    "completado" => Color.FromArgb("#498c96"),     // Verde
                    "en progreso" => Color.FromArgb("#FF9800"),    // Naranja
                    "asignado" => Color.FromArgb("#2196F3"),       // Azul
                    "pendiente" => Color.FromArgb("#F44336"),      // Rojo
                    "sin viajes" => Color.FromArgb("#9E9E9E"),     // Gris
                    _ => Color.FromArgb("#F44336")                 // Rojo por defecto
                };

                Estado.TextColor = colorEstado;
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al actualizar estado: {ex.Message}");

            // Estado de error
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Estado.Text = "Error al cargar";
                Estado.TextColor = Color.FromArgb("#F44336");
            });
        }
    }

    // Actualizar el método OnAppearing
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Mostrar indicador de carga
        if (_viewModelViajes != null)
        {
            _viewModelViajes.IsBusy = true;
        }

        try
        {
            // Recargar datos de viajes y estado del pedido
            if (_viewModelViajes != null)
            {
                await Task.Delay(500); // Pequeña pausa para asegurar que la BD se actualizó
                _viewModelViajes.InicializarViajes(); // Recargar viajes
            }

            // Actualizar el estado del pedido en la parte superior
            await ActualizarEstadoPedidoAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en OnAppearing: {ex.Message}");
        }
        finally
        {
            // Ocultar indicador de carga
            if (_viewModelViajes != null)
            {
                _viewModelViajes.IsBusy = false;
            }
        }
    }

    private async void Btn_atrasEstado(object sender, EventArgs e)
    {
        if (idtipousuario == 2)
        {
            await Navigation.PushAsync(new VCMisPedidos(idUsuario, idtipousuario));
        }
        else if (idtipousuario == 1)//No sé porque pero el idtipousuario siempre es 0 
            //y como este método solo es llamado desde las interfaces de administrador, entonces le
            //puse que sea igual a 0 y si o si va a entrar a esta condicional
        {
            await Navigation.PushAsync(new VEpedidos(idUsuario, idtipousuario));
        }
        else if (idtipousuario == 3)
        {
            await Navigation.PushAsync(new VTMisViajes(idUsuario, idtipousuario));
        }
    }

    private void expandir_Clicked(object sender, EventArgs e)
    {
        ExpanderViajes.IsExpanded = !ExpanderViajes.IsExpanded;
    }

    private async void Btn_SeguimientoViajes(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var viaje = button.CommandParameter as Viaje;

        if (viaje != null)
        {
            bool noAsignado =
                string.IsNullOrWhiteSpace(viaje.TractoAsig) || viaje.TractoAsig == "S/A" ||
                string.IsNullOrWhiteSpace(viaje.CisternaAsig) || viaje.CisternaAsig == "S/A" ||
                viaje.Cantidad <= 0 ||
                string.IsNullOrWhiteSpace(viaje.TrabajadoresAsig) || viaje.TrabajadoresAsig == "S/A";

            if (noAsignado)
            {
                if (idtipousuario == 1)
                {
                    await Navigation.PushAsync(new VEAsignarViaje(viaje, _pedido, idUsuario, idtipousuario));
                }
                else if (idtipousuario == 2)
                {
                    return;
                }
                else if (idtipousuario == 3)
                {
                    return;
                }
            }
            else
            {
                await Navigation.PushAsync(new VESeguimientoViaje(viaje, _pedido, idUsuario, idtipousuario));
            }
        }
    }
}