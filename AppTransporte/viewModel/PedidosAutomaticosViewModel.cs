using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using AppTransporte.model;
using AppTransporte.Interfaces;

public class PedidosAutomaticosViewModel : INotifyPropertyChanged
{
    private SqlServerService _sqlService;
    private int _idUsuario;
    private int _idTipoUsuario;
    private bool _isRefreshing;

    public ObservableCollection<PedidoAutomatico> PedidosAutomaticos { get; set; }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged();
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand EliminarCommand { get; }
    public ICommand CambiarEstadoCommand { get; }

    public PedidosAutomaticosViewModel(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;

        // Inicializar el servicio SQL
        string connectionString = "Data Source=SQL8011.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123;Connection Timeout=60"; // Reemplaza con tu connection string
        _sqlService = new SqlServerService(connectionString);

        PedidosAutomaticos = new ObservableCollection<PedidoAutomatico>();

        // Inicializar comandos
        RefreshCommand = new Command(async () => await CargarPedidosAsync());
        EditarCommand = new Command<PedidoAutomatico>(async (pedido) => await EditarPedido(pedido));
        EliminarCommand = new Command<PedidoAutomatico>(async (pedido) => await EliminarPedido(pedido));
        CambiarEstadoCommand = new Command<PedidoAutomatico>(async (pedido) => await CambiarEstadoPedido(pedido));
    }

    public async Task CargarPedidosAsync()
    {
        try
        {
            IsRefreshing = true;

            var pedidos = await _sqlService.ObtenerPedidosAutomaticosAsync();

            PedidosAutomaticos.Clear();
            foreach (var pedido in pedidos)
            {
                PedidosAutomaticos.Add(pedido);
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error",
                $"Error al cargar pedidos: {ex.Message}", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    public async Task EditarPedido(PedidoAutomatico pedido)
    {
        try
        {
            var paginaEdicion = new PedidoAuto(_idUsuario, _idTipoUsuario, pedido);
            await Application.Current.MainPage.Navigation.PushAsync(paginaEdicion);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error",
                $"Error al abrir edición: {ex.Message}", "OK");
        }
    }

    private async Task EliminarPedido(PedidoAutomatico pedido)
    {
        try
        {
            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Confirmar eliminación",
                $"¿Está seguro de que desea eliminar el pedido automático de {pedido.TipoServicio}?",
                "Sí", "No");

            if (!confirmar) return;

            var respuesta = await _sqlService.EliminarPedidoAutomaticoAsync(pedido.IdPedidoAutomatico);

            if (respuesta.EsExitoso)
            {
                PedidosAutomaticos.Remove(pedido);
                await Application.Current.MainPage.DisplayAlert("Éxito",
                    "Pedido automático eliminado exitosamente.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", respuesta.Mensaje, "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error",
                $"Error al eliminar pedido: {ex.Message}", "OK");
        }
    }

    private async Task CambiarEstadoPedido(PedidoAutomatico pedido)
    {
        try
        {
            bool nuevoEstado = !pedido.Estado;
            string accion = nuevoEstado ? "activar" : "desactivar";

            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                $"Confirmar {accion}",
                $"¿Está seguro de que desea {accion} este pedido automático?",
                "Sí", "No");

            if (!confirmar) return;

            var respuesta = await _sqlService.CambiarEstadoPedidoAutomaticoAsync(
                pedido.IdPedidoAutomatico, nuevoEstado);

            if (respuesta.EsExitoso)
            {
                pedido.Estado = nuevoEstado;


                // Refrescar la lista para actualizar la interfaz
                await CargarPedidosAsync();

                await Application.Current.MainPage.DisplayAlert("Éxito", respuesta.Mensaje, "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", respuesta.Mensaje, "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error",
                $"Error al cambiar estado: {ex.Message}", "OK");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// =============================================================================
// CONVERSORES PARA LA INTERFAZ
// =============================================================================
public class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return !string.IsNullOrWhiteSpace(value as string);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToActivarTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return (bool)value ? "Desactivar" : "Activar";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToActivarColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return (bool)value ? Color.FromArgb("#ffc107") : Color.FromArgb("#28a745");
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}