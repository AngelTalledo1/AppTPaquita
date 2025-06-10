using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class ListaPedidosProgramados : ContentPage
    {
        private int _idUsuario;
        private int _idTipoUsuario;
        private ObservableCollection<PedidoProgramado> _pedidosOriginales;
        private ObservableCollection<PedidoProgramado> _pedidosFiltrados;

        public ObservableCollection<PedidoProgramado> PedidosFiltrados
        {
            get => _pedidosFiltrados;
            set
            {
                _pedidosFiltrados = value;
                OnPropertyChanged();
            }
        }

        public ListaPedidosProgramados(int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();

            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;

            _pedidosOriginales = new ObservableCollection<PedidoProgramado>();
            _pedidosFiltrados = new ObservableCollection<PedidoProgramado>();

            // Configurar binding context
            BindingContext = this;

            // Inicializar filtros
            InicializarFiltros();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarPedidosProgramados();
        }

        private void InicializarFiltros()
        {
            // Configurar fechas por defecto
            FechaInicioFiltro.Date = DateTime.Today.AddMonths(-1);
            FechaFinFiltro.Date = DateTime.Today.AddMonths(3);

            // Seleccionar "Todos" por defecto
            EstadoFiltro.SelectedIndex = 0; // "Todos"
            FrecuenciaFiltro.SelectedIndex = 0; // "Todas"
        }

        private async Task CargarPedidosProgramados()
        {
            try
            {
                // Mostrar indicador de carga
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                MensajeSinDatos.IsVisible = false;

                // Obtener datos
                var pedidos = await App.Database.ObtenerPedidosProgramadosAsync(
                    idUsuario: _idUsuario,
                    incluirFinalizados: true
                );

                // Actualizar colección
                _pedidosOriginales.Clear();
                foreach (var pedido in pedidos)
                {
                    _pedidosOriginales.Add(pedido);
                }

                // Aplicar filtros
                AplicarFiltros();

                // Actualizar estadísticas
                await ActualizarEstadisticas();

                System.Diagnostics.Debug.WriteLine($"Cargados {pedidos.Count} pedidos programados");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar pedidos programados:\n{ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al cargar pedidos: {ex.Message}");
            }
            finally
            {
                // Ocultar indicador de carga
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private void AplicarFiltros()
        {
            try
            {
                var pedidosFiltrados = _pedidosOriginales.AsEnumerable();

                // Filtro por estado
                var estadoSeleccionado = EstadoFiltro.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(estadoSeleccionado) && estadoSeleccionado != "Todos")
                {
                    pedidosFiltrados = pedidosFiltrados.Where(p => p.Estado == estadoSeleccionado);
                }

                // Filtro por frecuencia
                var frecuenciaSeleccionada = FrecuenciaFiltro.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(frecuenciaSeleccionada) && frecuenciaSeleccionada != "Todas")
                {
                    pedidosFiltrados = pedidosFiltrados.Where(p => p.Frecuencia == frecuenciaSeleccionada);
                }

                // Filtro por fechas
                var fechaInicio = FechaInicioFiltro.Date;
                var fechaFin = FechaFinFiltro.Date;

                pedidosFiltrados = pedidosFiltrados.Where(p =>
                    p.FechaInicio >= fechaInicio && p.FechaFin <= fechaFin);

                // Ordenar por estado y próxima ejecución
                var resultado = pedidosFiltrados
                    .OrderBy(p => p.Estado == "Activo" ? 0 : 1)
                    .ThenBy(p => p.ProximaEjecucion ?? DateTime.MaxValue)
                    .ThenByDescending(p => p.FechaCreacion)
                    .ToList();

                // Actualizar colección
                PedidosFiltrados.Clear();
                foreach (var pedido in resultado)
                {
                    PedidosFiltrados.Add(pedido);
                }

                // Mostrar mensaje si no hay datos
                MensajeSinDatos.IsVisible = !PedidosFiltrados.Any();

                System.Diagnostics.Debug.WriteLine($"Filtros aplicados: {PedidosFiltrados.Count} pedidos mostrados");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Error al aplicar filtros: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al aplicar filtros: {ex.Message}");
            }
        }

        private async Task ActualizarEstadisticas()
        {
            try
            {
                var estadisticas = await App.Database.ObtenerEstadisticasPedidosProgramadosAsync(_idUsuario);

                var texto = $"?? Total: {estadisticas.GetValueOrDefault("total_pedidos", 0)} | " +
                           $"? Activos: {estadisticas.GetValueOrDefault("activos", 0)} | " +
                           $"?? Pausados: {estadisticas.GetValueOrDefault("pausados", 0)} | " +
                           $"? Pendientes: {estadisticas.GetValueOrDefault("pendientes_ejecucion", 0)} | " +
                           $"?? Ejecutados: {estadisticas.GetValueOrDefault("total_ejecuciones_realizadas", 0)}";

                EstadisticasLabel.Text = texto;
            }
            catch (Exception ex)
            {
                EstadisticasLabel.Text = "Error al cargar estadísticas";
                System.Diagnostics.Debug.WriteLine($"Error al cargar estadísticas: {ex.Message}");
            }
        }

        #region Eventos de UI

        private async void Btn_atras(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PedidoAuto(_idUsuario, _idTipoUsuario));
        }

        private void OnFiltroChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void Btn_limpiarFiltros(object sender, EventArgs e)
        {
            EstadoFiltro.SelectedIndex = 0;
            FrecuenciaFiltro.SelectedIndex = 0;
            FechaInicioFiltro.Date = DateTime.Today.AddMonths(-1);
            FechaFinFiltro.Date = DateTime.Today.AddMonths(3);

            AplicarFiltros();
        }

        private async void Btn_refrescar(object sender, EventArgs e)
        {
            await CargarPedidosProgramados();
        }

        #endregion

        #region Acciones de Pedidos

        private async void Btn_ejecutarPedido(object sender, EventArgs e)
        {
            var button = sender as Button;
            var pedido = button?.CommandParameter as PedidoProgramado;

            if (pedido == null) return;

            try
            {
                button.IsEnabled = false;
                button.Text = "Ejecutando...";

                bool confirmar = await DisplayAlert(
                    "Ejecutar Pedido Programado",
                    $"¿Está seguro que desea ejecutar manualmente el pedido programado?\n\n" +
                    $"Tipo: {pedido.TipoServicio}\n" +
                    $"Cantidad: {pedido.CantidadTexto}\n" +
                    $"Esto creará un pedido real en el sistema.",
                    "Ejecutar", "Cancelar");

                if (!confirmar) return;

                var resultado = await App.Database.EjecutarPedidosProgramadosAsync(
                    pedido.IdPedidoProgramado,
                    ejecutarManualmente: true);

                if (resultado.PedidosEjecutados > 0)
                {
                    await DisplayAlert("? Éxito",
                        $"Pedido ejecutado correctamente.\n\n" +
                        $"Se ha creado un nuevo pedido en el sistema.", "OK");

                    await CargarPedidosProgramados(); // Refrescar datos
                }
                else
                {
                    await DisplayAlert("? Error",
                        $"No se pudo ejecutar el pedido.\n{resultado.Mensaje}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al ejecutar pedido: {ex.Message}", "OK");
            }
            finally
            {
                if (button != null)
                {
                    button.IsEnabled = true;
                    button.Text = "?? Ejecutar";
                }
            }
        }

        private async void Btn_pausarReanudar(object sender, EventArgs e)
        {
            var button = sender as Button;
            var pedido = button?.CommandParameter as PedidoProgramado;

            if (pedido == null) return;

            try
            {
                button.IsEnabled = false;

                string nuevoEstado = pedido.Estado == "Activo" ? "Pausado" : "Activo";
                string accion = nuevoEstado == "Pausado" ? "pausar" : "reanudar";

                bool confirmar = await DisplayAlert(
                    $"{char.ToUpper(accion[0])}{accion.Substring(1)} Pedido",
                    $"¿Está seguro que desea {accion} este pedido programado?\n\n" +
                    $"{pedido.TipoServicio}",
                    char.ToUpper(accion[0]) + accion.Substring(1), "Cancelar");

                if (!confirmar) return;

                var mensaje = await App.Database.CambiarEstadoPedidoProgramadoAsync(
                    pedido.IdPedidoProgramado,
                    nuevoEstado,
                    $"Estado cambiado manualmente a {nuevoEstado}");

                await DisplayAlert("? Éxito", mensaje, "OK");
                await CargarPedidosProgramados(); // Refrescar datos
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cambiar estado: {ex.Message}", "OK");
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        private async void Btn_editarPedido(object sender, EventArgs e)
        {
            var button = sender as Button;
            var pedido = button?.CommandParameter as PedidoProgramado;

            if (pedido == null) return;

            try
            {
                // Navegar a página de edición
                await Navigation.PushAsync(new EditarPedidoProgramado(pedido, _idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al abrir editor: {ex.Message}", "OK");
            }
        }

        private async void Btn_eliminarPedido(object sender, EventArgs e)
        {
            var button = sender as Button;
            var pedido = button?.CommandParameter as PedidoProgramado;

            if (pedido == null) return;

            try
            {
                button.IsEnabled = false;

                bool confirmar = await DisplayAlert(
                    "?? Eliminar Pedido Programado",
                    $"¿Está seguro que desea eliminar este pedido programado?\n\n" +
                    $"Tipo: {pedido.TipoServicio}\n" +
                    $"Frecuencia: {pedido.FrecuenciaDescripcion}\n" +
                    $"Estado: {pedido.Estado}\n\n" +
                    $"Esta acción NO se puede deshacer.",
                    "??? Eliminar", "Cancelar");

                if (!confirmar) return;

                // Solicitar motivo si el pedido está activo
                string motivo = null;
                if (pedido.Estado == "Activo")
                {
                    motivo = await DisplayPromptAsync(
                        "Motivo de Eliminación",
                        "Por favor, ingrese el motivo de la eliminación:",
                        "OK", "Cancelar",
                        "Ejemplo: Ya no se necesita el servicio",
                        maxLength: 200);

                    if (string.IsNullOrWhiteSpace(motivo)) return;
                }

                var mensaje = await App.Database.EliminarPedidoProgramadoAsync(
                    pedido.IdPedidoProgramado,
                    motivo);

                await DisplayAlert("? Eliminado", mensaje, "OK");
                await CargarPedidosProgramados(); // Refrescar datos
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al eliminar pedido: {ex.Message}", "OK");
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        #endregion

        #region Eventos INotifyPropertyChanged

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}