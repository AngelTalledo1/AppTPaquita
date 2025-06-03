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
        private readonly SqlServerService _sqlService;
        private List<TareaAdicional> _todasLasTareas;
        private bool _mostrandoTodasLasTareas = false;
        private int _idTipoUsuario;
        private int _idUsuario;

        public TareasAdicionales(int idTipoUsuario, int idUsuario)
        {
            InitializeComponent();

            // ACTUALIZAR CON TU CONNECTION STRING
            string connectionString = ("Data Source=SQL8011.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123;Connection Timeout=60");
            _sqlService = new SqlServerService(connectionString);

            _todasLasTareas = new List<TareaAdicional>();

            // Inicializar con fecha de hoy
            DatePickerFiltro.Date = DateTime.Today;
            _idTipoUsuario = idTipoUsuario;
            _idUsuario = idUsuario;
        }
        public void setUserData(int idUsuario, int idTipoUsuario)
        {
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarTareasAsync();
        }

        private async void Btn_Atras(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MenuTransportista(_idUsuario, _idTipoUsuario));
        }
        public class TareasAdicionalesService
        {
            private readonly string _connectionString;

            public TareasAdicionalesService(string connectionString)
            {
                _connectionString = connectionString;
            }
        }

            private async void Btn_NuevaTarea(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la página de nueva tarea
                await Navigation.PushAsync(new VTNuevaTareaAdicional(_idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al navegar: {ex.Message}", "OK");
            }
        }

        private async void DatePickerFiltro_DateSelected(object sender, DateChangedEventArgs e)
        {
            _mostrandoTodasLasTareas = false;
            await CargarTareasPorFechaAsync(e.NewDate);
        }

        private async void Btn_FiltrarHoy(object sender, EventArgs e)
        {
            DatePickerFiltro.Date = DateTime.Today;
            _mostrandoTodasLasTareas = false;
            await CargarTareasPorFechaAsync(DateTime.Today);
        }

        private async void Btn_MostrarTodas(object sender, EventArgs e)
        {
            _mostrandoTodasLasTareas = true;
            await CargarTodasLasTareasAsync();
        }

        private async Task CargarTareasAsync()
        {
            if (_mostrandoTodasLasTareas)
            {
                await CargarTodasLasTareasAsync();
            }
            else
            {
                await CargarTareasPorFechaAsync(DatePickerFiltro.Date);
            }
        }

        private async Task CargarTodasLasTareasAsync()
        {
            try
            {
                MostrarCargando(true);

                int idUsuario = ObtenerIdUsuarioActual();
                _todasLasTareas = await _sqlService.ObtenerTareasUsuarioAsync(idUsuario);

                MostrarTareasEnVista(_todasLasTareas);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar tareas: {ex.Message}", "OK");
            }
            finally
            {
                MostrarCargando(false);
            }
        }

        private async Task CargarTareasPorFechaAsync(DateTime fecha)
        {
            try
            {
                MostrarCargando(true);

                int idUsuario = ObtenerIdUsuarioActual();
                var tareasFecha = await _sqlService.ObtenerTareasPorFechaAsync(fecha, idUsuario);

                MostrarTareasEnVista(tareasFecha);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar tareas: {ex.Message}", "OK");
            }
            finally
            {
                MostrarCargando(false);
            }
        }

        private void MostrarCargando(bool mostrar)
        {
            LoadingIndicator.IsRunning = mostrar;
            LoadingIndicator.IsVisible = mostrar;
        }

        private void MostrarTareasEnVista(List<TareaAdicional> tareas)
        {
            StackTareas.Children.Clear();

            if (tareas.Count == 0)
            {
                LabelSinTareas.IsVisible = true;
                return;
            }

            LabelSinTareas.IsVisible = false;

            foreach (var tarea in tareas)
            {
                var frameItem = CrearFrameTarea(tarea);
                StackTareas.Children.Add(frameItem);
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
                Text = $" {tarea.FechaTareaString} | {tarea.HorarioCompleto}",
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
                    MostrarCargando(true);

                    var resultado = await _sqlService.EliminarTareaAsync(tarea.id_tareaAdicional);

                    if (resultado.EsExitoso)
                    {
                        await DisplayAlert("Éxito", resultado.Mensaje, "OK");
                        await CargarTareasAsync(); // Esto recarga toda la interfaz
                    }
                    else
                    {
                        await DisplayAlert("Error", resultado.Mensaje, "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Error al eliminar tarea: {ex.Message}", "OK");
                }
                finally
                {
                    MostrarCargando(false);
                }
            }
        }

        private int ObtenerIdUsuarioActual()
        {
            // IMPLEMENTA SEGÚN TU SISTEMA DE AUTENTICACIÓN
            // Ejemplo usando Preferences:
            // return Preferences.Get("UsuarioId", 1);

            // Ejemplo usando SecureStorage:
            // var userId = await SecureStorage.GetAsync("UsuarioId");
            // return int.TryParse(userId, out int id) ? id : 1;

            // POR AHORA RETORNA 1 PARA PRUEBAS
            return 1;
        }
    }
}