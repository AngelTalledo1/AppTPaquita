namespace AppTransporte.Interfaces
{
    public partial class VEReportesClientes : ContentPage
    {
        private List<ClienteModel> _clientes;
        private int _idUsuario;
        private int _idTipoUsuario;
        public string ClienteNombre { get; private set; }

        public VEReportesClientes(int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            this._idUsuario = idUsuario;
            this._idTipoUsuario = idTipoUsuario;
            CargarDatosIniciales();
        }

        private async void CargarDatosIniciales()
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                // Cargar cliente actual
                await CargarClienteAsync();

                // Establecer el contexto de enlace para mostrar el nombre del cliente en la interfaz
                this.BindingContext = this;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar datos iniciales: {ex.Message}", "Ok");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async Task CargarClienteAsync()
        {
            _clientes = new List<ClienteModel>();

            try
            {
                // Obtener el cliente para el usuario actual
                var clienteData = await App.Database.ObtenerClientePorUsuarioAsync(_idUsuario);
                if (clienteData != null)
                {
                    var nombreCompleto = $"{clienteData.Nombre} {clienteData.ApePaterno} {clienteData.ApeMaterno}".Trim();
                    _clientes.Add(new ClienteModel
                    {
                        IdCliente = clienteData.IdCliente,
                        NombreCompleto = nombreCompleto
                    });

                    // Guardar el nombre del cliente para mostrarlo en la UI
                    ClienteNombre = nombreCompleto;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar cliente: {ex.Message}");
                // Propagar la excepción para que sea manejada por el método que llama
                throw;
            }
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        #region Event Handlers para Generar Reportes

        private async void ReporteSolicitudes_Clicked(object sender, EventArgs e)
        {
            if (_clientes.Count == 0)
            {
                await DisplayAlert("Error", "No se pudo identificar el cliente", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                var clienteSeleccionado = _clientes[0];

                // Navegar al reporte de atención de solicitudes
                await Navigation.PushAsync(new VistaReporteAtencionSolicitudes(
                    clienteSeleccionado.IdCliente, _idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al generar reporte: {ex.Message}", "Ok");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void ReporteTrabajadores_Clicked(object sender, EventArgs e)
        {
            if (_clientes.Count == 0)
            {
                await DisplayAlert("Error", "No se pudo identificar el cliente", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                var clienteSeleccionado = _clientes[0];

                // Navegar al reporte de trabajadores asignados al cliente
                await Navigation.PushAsync(new VistaReporteTrabajadoresCliente(
                    clienteSeleccionado.IdCliente, _idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al abrir el reporte de trabajadores: {ex.Message}", "Ok");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void ReportePedidos_Clicked(object sender, EventArgs e)
        {
            if (_clientes.Count == 0)
            {
                await DisplayAlert("Error", "No se pudo identificar el cliente", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                var clienteSeleccionado = _clientes[0];

                // Navegar al reporte de pedidos del cliente
                await Navigation.PushAsync(new VistaReportePedidosCliente(
                    clienteSeleccionado.IdCliente, _idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al abrir el reporte de pedidos: {ex.Message}", "Ok");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void ReporteDesvios_Clicked(object sender, EventArgs e)
        {
            if (_clientes.Count == 0)
            {
                await DisplayAlert("Error", "No se pudo identificar el cliente", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                var clienteSeleccionado = _clientes[0];

                // Navegar al reporte de desvíos del cliente
                await Navigation.PushAsync(new VistaReporteDesviosCliente(
                    clienteSeleccionado.IdCliente, _idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al abrir el reporte de desvíos: {ex.Message}", "Ok");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void ReporteProgramacion_Clicked(object sender, EventArgs e)
        {
            if (_clientes.Count == 0)
            {
                await DisplayAlert("Error", "No se pudo identificar el cliente", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                var clienteSeleccionado = _clientes[0];

                // Navegar al reporte de programación del cliente
                await Navigation.PushAsync(new VistaReporteProgramacionCliente(
                    clienteSeleccionado.IdCliente, _idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al abrir el reporte de programación: {ex.Message}", "Ok");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void ReporteTrabajosTrabajador_Clicked(object sender, EventArgs e)
        {
            if (_clientes.Count == 0)
            {
                await DisplayAlert("Error", "No se pudo identificar el cliente", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                var clienteSeleccionado = _clientes[0];

                // Navegar al reporte de trabajos por trabajador
                await Navigation.PushAsync(new VistaReporteTrabajosTrabajadorCliente(
                    clienteSeleccionado.IdCliente, _idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al abrir el reporte de trabajos por trabajador: {ex.Message}", "Ok");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        #endregion
    }

    #region Clases Modelo

    public class ClienteModel
    {
        public int IdCliente { get; set; }
        public string NombreCompleto { get; set; }
    }

    public class TrabajadorModel
    {
        public int IdTrabajador { get; set; }
        public string NombreCompleto { get; set; }
        public string Categoria { get; set; }
    }

    #endregion
}
