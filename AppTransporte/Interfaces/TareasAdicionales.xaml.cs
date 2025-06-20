using Microsoft.Maui.Controls;
using AppTransporte.viewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using AppTransporte.Interfaces;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class TareasAdicionales : ContentPage
    {
        private SqlServerService _sqlService;
        private int _idTipoUsuario;
        private int _idUsuario;
        private bool _cargandoDatos = false;
        private bool _paginaInicializada = false;

        public TareasAdicionales(int idTipoUsuario, int idUsuario)
        {
            InitializeComponent();

            _idTipoUsuario = idTipoUsuario;
            _idUsuario = idUsuario;

            System.Diagnostics.Debug.WriteLine($"=== CONSTRUCTOR TareasAdicionales ===");
            System.Diagnostics.Debug.WriteLine($"Usuario: {_idUsuario}, TipoUsuario: {_idTipoUsuario}");
        }

        private void InicializarServicioSQL()
        {
            try
            {
                // Crear una nueva instancia cada vez para evitar problemas de estado
                string connectionString = "Data Source=SQL8011.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123;Connection Timeout=60";
                _sqlService = new SqlServerService(connectionString);
                System.Diagnostics.Debug.WriteLine("SqlServerService inicializado correctamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inicializando SqlServerService: {ex.Message}");
                _sqlService = null;
            }
        }

        private void InicializarControles()
        {
            try
            {
                // Desactivar eventos temporalmente
                DatePickerFiltro.DateSelected -= DatePickerFiltro_DateSelected;

                // Resetear controles a estado inicial
                DatePickerFiltro.Date = DateTime.Today;
                LabelSinTareas.IsVisible = false;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;

                // Limpiar lista de tareas
                if (StackTareas != null)
                {
                    StackTareas.Children.Clear();
                }

                // Reactivar eventos
                DatePickerFiltro.DateSelected += DatePickerFiltro_DateSelected;

                System.Diagnostics.Debug.WriteLine("Controles inicializados correctamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inicializando controles: {ex.Message}");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            System.Diagnostics.Debug.WriteLine($"=== OnAppearing INICIO ===");
            System.Diagnostics.Debug.WriteLine($"PaginaInicializada: {_paginaInicializada}");
            System.Diagnostics.Debug.WriteLine($"CargandoDatos: {_cargandoDatos}");

            // Evitar múltiples cargas concurrentes
            if (_cargandoDatos)
            {
                System.Diagnostics.Debug.WriteLine("Ya se están cargando datos, saliendo...");
                return;
            }

            try
            {
                _cargandoDatos = true;

                // RESETEAR COMPLETAMENTE el estado cada vez
                InicializarServicioSQL();
                InicializarControles();

                // Pequeña pausa para asegurar que la UI esté lista
                await Task.Delay(100);

                // Cargar tareas para HOY por defecto
                await CargarTareasPorFechaAsync(DateTime.Today);

                _paginaInicializada = true;
                System.Diagnostics.Debug.WriteLine("=== OnAppearing COMPLETADO ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en OnAppearing: {ex.Message}");
                await DisplayAlert("Error", $"Error al cargar la página: {ex.Message}", "OK");
            }
            finally
            {
                _cargandoDatos = false;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            System.Diagnostics.Debug.WriteLine("=== OnDisappearing ===");

            // Limpiar estado al salir
            _paginaInicializada = false;
            _cargandoDatos = false;

            // Detener indicador de carga si está activo
            if (LoadingIndicator != null)
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private async void Btn_Atras(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== Navegando ATRÁS ===");
            try
            {
                await Navigation.PushAsync(new MenuTransportista(_idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navegando atrás: {ex.Message}");
            }
        }

        private async void Btn_NuevaTarea(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== Navegando a NUEVA TAREA ===");
                await Navigation.PushAsync(new VTNuevaTareaAdicional(_idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error navegando a nueva tarea: {ex.Message}");
                await DisplayAlert("Error", $"Error al navegar: {ex.Message}", "OK");
            }
        }

        private async void DatePickerFiltro_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (_cargandoDatos) return;

            System.Diagnostics.Debug.WriteLine($"=== FECHA SELECCIONADA: {e.NewDate:yyyy-MM-dd} ===");
            await CargarTareasPorFechaAsync(e.NewDate);
        }

        private async void Btn_FiltrarHoy(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;

            System.Diagnostics.Debug.WriteLine("=== FILTRO HOY ===");
            DatePickerFiltro.Date = DateTime.Today;
            await CargarTareasPorFechaAsync(DateTime.Today);
        }

        private async void Btn_MostrarTodas(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;

            System.Diagnostics.Debug.WriteLine("=== FILTRO TODAS ===");
            await CargarTodasLasTareasAsync();
        }

        private async Task CargarTodasLasTareasAsync()
        {
            if (_cargandoDatos) return;

            try
            {
                _cargandoDatos = true;
                System.Diagnostics.Debug.WriteLine($">>> CARGANDO TODAS LAS TAREAS - Usuario: {_idUsuario}");

                MostrarCargando(true);

                if (_sqlService == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: _sqlService es null, reinicializando...");
                    InicializarServicioSQL();
                }

                var todasLasTareas = await _sqlService.ObtenerTareasUsuarioAsync(_idUsuario);

                System.Diagnostics.Debug.WriteLine($">>> RESULTADO: {todasLasTareas?.Count ?? 0} tareas obtenidas");

                // Log detallado de cada tarea
                if (todasLasTareas != null)
                {
                    for (int i = 0; i < todasLasTareas.Count; i++)
                    {
                        var tarea = todasLasTareas[i];
                        System.Diagnostics.Debug.WriteLine($"  [{i}] ID:{tarea.id_tareaAdicional} - {tarea.fecha_tarea:yyyy-MM-dd} - {tarea.descripcion}");
                    }
                }

                MostrarTareasEnVista(todasLasTareas);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en CargarTodasLasTareasAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                await DisplayAlert("Error", $"Error al cargar tareas: {ex.Message}", "OK");
            }
            finally
            {
                MostrarCargando(false);
                _cargandoDatos = false;
            }
        }

        private async Task CargarTareasPorFechaAsync(DateTime fecha)
        {
            if (_cargandoDatos) return;

            try
            {
                _cargandoDatos = true;
                System.Diagnostics.Debug.WriteLine($">>> CARGANDO TAREAS POR FECHA: {fecha:yyyy-MM-dd} - Usuario: {_idUsuario}");

                MostrarCargando(true);

                if (_sqlService == null)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: _sqlService es null, reinicializando...");
                    InicializarServicioSQL();
                }

                var tareasFecha = await _sqlService.ObtenerTareasPorFechaAsync(fecha, _idUsuario);

                System.Diagnostics.Debug.WriteLine($">>> RESULTADO: {tareasFecha?.Count ?? 0} tareas obtenidas para {fecha:yyyy-MM-dd}");

                // Log detallado de cada tarea
                if (tareasFecha != null)
                {
                    for (int i = 0; i < tareasFecha.Count; i++)
                    {
                        var tarea = tareasFecha[i];
                        System.Diagnostics.Debug.WriteLine($"  [{i}] ID:{tarea.id_tareaAdicional} - {tarea.fecha_tarea:yyyy-MM-dd} - {tarea.descripcion}");
                    }
                }

                MostrarTareasEnVista(tareasFecha);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en CargarTareasPorFechaAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                await DisplayAlert("Error", $"Error al cargar tareas: {ex.Message}", "OK");
            }
            finally
            {
                MostrarCargando(false);
                _cargandoDatos = false;
            }
        }

        private void MostrarCargando(bool mostrar)
        {
            try
            {
                if (LoadingIndicator != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        LoadingIndicator.IsRunning = mostrar;
                        LoadingIndicator.IsVisible = mostrar;
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en MostrarCargando: {ex.Message}");
            }
        }

        private void MostrarTareasEnVista(List<TareaAdicional> tareas)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($">>> MOSTRANDO TAREAS EN VISTA: {tareas?.Count ?? 0} tareas");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (StackTareas == null)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: StackTareas es null");
                        return;
                    }

                    // Limpiar lista actual
                    StackTareas.Children.Clear();
                    System.Diagnostics.Debug.WriteLine("StackTareas limpiado");

                    if (tareas == null || tareas.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("No hay tareas para mostrar");
                        if (LabelSinTareas != null)
                        {
                            LabelSinTareas.IsVisible = true;
                            LabelSinTareas.Text = "No hay tareas para mostrar";
                        }
                        return;
                    }

                    // Ocultar mensaje de "sin tareas"
                    if (LabelSinTareas != null)
                    {
                        LabelSinTareas.IsVisible = false;
                    }

                    // Agregar cada tarea a la vista
                    for (int i = 0; i < tareas.Count; i++)
                    {
                        var tarea = tareas[i];
                        System.Diagnostics.Debug.WriteLine($">>> Agregando tarea [{i}]: {tarea.descripcion}");

                        try
                        {
                            var frameItem = CrearFrameTarea(tarea);
                            StackTareas.Children.Add(frameItem);
                            System.Diagnostics.Debug.WriteLine($"  ? Tarea [{i}] agregada exitosamente");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"  ? Error agregando tarea [{i}]: {ex.Message}");
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($">>> TOTAL en StackTareas: {StackTareas.Children.Count} elementos");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR en MostrarTareasEnVista: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        private Frame CrearFrameTarea(TareaAdicional tarea)
        {
            var frame = new Frame
            {
                BackgroundColor = Color.FromArgb("#f8f9fa"),
                CornerRadius = 8,
                HasShadow = true,
                Margin = new Thickness(5),
                Padding = new Thickness(15),
                BorderColor = Color.FromArgb("#e9ecef")
            };

            var stackLayout = new VerticalStackLayout { Spacing = 8 };

            // Cabecera con fecha y horario
            var labelCabecera = new Label
            {
                Text = $"{tarea.FechaTareaString} | {tarea.HorarioCompleto}",
                FontSize = 14,
                FontFamily = "Comf-Medium",
                TextColor = Color.FromArgb("#2c3e50")
            };

            // Descripción
            var labelDescripcion = new Label
            {
                Text = tarea.descripcion,
                FontSize = 13,
                FontFamily = "Comf-Regular",
                TextColor = Color.FromArgb("#34495e"),
                LineBreakMode = LineBreakMode.WordWrap,
                Margin = new Thickness(0, 5)
            };

            // Información adicional
            var labelInfo = new Label
            {
                Text = $"Creada: {tarea.FechaCreacionString}",
                FontSize = 11,
                FontFamily = "Comf-Regular",
                TextColor = Color.FromArgb("#7f8c8d")
            };

            // Botón eliminar
            var btnEliminar = new Button
            {
                Text = "Eliminar",
                FontSize = 12,
                BackgroundColor = Color.FromArgb("#e74c3c"),
                TextColor = Colors.White,
                FontFamily = "Comf-Medium",
                CornerRadius = 5,
                HeightRequest = 35,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 5, 0, 0)
            };

            btnEliminar.Clicked += async (s, e) => await EliminarTarea(tarea);

            stackLayout.Children.Add(labelCabecera);
            stackLayout.Children.Add(labelDescripcion);
            stackLayout.Children.Add(labelInfo);
            stackLayout.Children.Add(btnEliminar);

            frame.Content = stackLayout;
            return frame;
        }

        private async Task EliminarTarea(TareaAdicional tarea)
        {
            if (_cargandoDatos) return;

            bool confirmacion = await DisplayAlert(
                "Confirmar Eliminación",
                $"¿Estás seguro de que deseas eliminar esta tarea?\n\n" +
                $"{tarea.FechaTareaString}\n" +
                $"{tarea.HorarioCompleto}\n\n" +
                $"{tarea.descripcion}",
                "Eliminar",
                "Cancelar");

            if (confirmacion)
            {
                try
                {
                    _cargandoDatos = true;
                    System.Diagnostics.Debug.WriteLine($"=== ELIMINANDO TAREA ID: {tarea.id_tareaAdicional} ===");

                    MostrarCargando(true);

                    var resultado = await _sqlService.EliminarTareaAsync(tarea.id_tareaAdicional);

                    if (resultado.EsExitoso)
                    {
                        await DisplayAlert("Éxito", resultado.Mensaje, "OK");
                        System.Diagnostics.Debug.WriteLine("Tarea eliminada exitosamente, recargando...");

                        // Recargar tareas para la fecha actual del DatePicker
                        await CargarTareasPorFechaAsync(DatePickerFiltro.Date);
                    }
                    else
                    {
                        await DisplayAlert("Error", resultado.Mensaje, "OK");
                        System.Diagnostics.Debug.WriteLine($"Error al eliminar: {resultado.Mensaje}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al eliminar tarea: {ex.Message}");
                    await DisplayAlert("Error", $"Error al eliminar tarea: {ex.Message}", "OK");
                }
                finally
                {
                    MostrarCargando(false);
                    _cargandoDatos = false;
                }
            }
        }
    }
}