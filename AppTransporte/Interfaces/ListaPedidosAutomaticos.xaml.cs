using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class ListaPedidosAutomaticos : ContentPage
    {
        private int idUsuario;
        private int idtipousuario;
        private List<PedidoAutomatico> _pedidos;
        private string _tipoFiltroActual = "Todos"; // "Todos" o "Hoy"

        public ListaPedidosAutomaticos(int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            this.idUsuario = idUsuario;
            this.idtipousuario = idTipoUsuario;

        
            _pedidos = new List<PedidoAutomatico>();
            InicializarFiltros();
            ActualizarEstadoBotonesFiltro();
        }

        private void InicializarFiltros()
        {
            // Inicializar con filtro "Todos" activo
            _tipoFiltroActual = "Todos";
        }

        private void ActualizarEstadoBotonesFiltro()
        {
            if (_tipoFiltroActual == "Todos")
            {
                BtnFiltroTodos.BackgroundColor = Color.FromArgb("#007AFF"); // Azul activo
                BtnFiltroHoy.BackgroundColor = Color.FromArgb("#6c757d");   // Gris inactivo
            }
            else // "Hoy"
            {
                BtnFiltroTodos.BackgroundColor = Color.FromArgb("#6c757d");  // Gris inactivo
                BtnFiltroHoy.BackgroundColor = Color.FromArgb("#007AFF");    // Azul activo
            }
        }

        private async void Btn_filtroTodos(object sender, EventArgs e)
        {
            _tipoFiltroActual = "Todos";
            ActualizarEstadoBotonesFiltro();
            await CargarPedidosAsync();
        }

        private async void Btn_filtroHoy(object sender, EventArgs e)
        {
            _tipoFiltroActual = "Hoy";
            ActualizarEstadoBotonesFiltro();
            await CargarPedidosAsync();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarPedidosAsync();
        }

        private async Task CargarPedidosAsync()
        {
            try
            {
             

                // Obtener todos los pedidos del usuario
                var todosPedidos = await App.Database.ObtenerPedidosAutomaticosAsync();

                // Aplicar filtro según el tipo seleccionado
                if (_tipoFiltroActual == "Hoy")
                {
                    _pedidos = FiltrarPedidosHoy(todosPedidos);
                }
                else // "Todos"
                {
                    _pedidos = todosPedidos;
                }

                if (_pedidos.Count == 0)
                {
                    EmptyStateLayout.IsVisible = true;
                    PedidosScrollView.IsVisible = false;

                    // Actualizar mensaje según el tipo de filtro
                    var emptyLabel = EmptyStateLayout.Children.OfType<Label>().FirstOrDefault();
                    if (emptyLabel != null)
                    {
                        if (_tipoFiltroActual == "Hoy")
                        {
                            emptyLabel.Text = "No hay pedidos automáticos programados para hoy";
                        }
                        else
                        {
                            emptyLabel.Text = "No hay pedidos automáticos programados";
                        }
                    }
                }
                else
                {
                    EmptyStateLayout.IsVisible = false;
                    PedidosScrollView.IsVisible = true;
                    CrearListaPedidos();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar pedidos: {ex.Message}", "OK");
                EmptyStateLayout.IsVisible = true;
                PedidosScrollView.IsVisible = false;
            }
        }

        private List<PedidoAutomatico> FiltrarPedidosHoy(List<PedidoAutomatico> todosPedidos)
        {
            var hoy = DateTime.Today;
            var diaSemanaHoy = ObtenerDiaSemanaEspanol(hoy.DayOfWeek);

            return todosPedidos.Where(pedido =>
            {
                // 1. Verificar que el pedido esté activo
                if (!pedido.Estado) return false;

                // 2. Verificar que hoy esté dentro del rango de fechas del pedido
                if (hoy < pedido.FechaInicio.Date || hoy > pedido.FechaFin.Date) return false;

                // 3. Verificar que hoy sea uno de los días programados
                var diasProgramados = pedido.DiasSemana.Split(',').Select(d => d.Trim()).ToList();
                return diasProgramados.Contains(diaSemanaHoy);

            }).ToList();
        }

        private string ObtenerDiaSemanaEspanol(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => ""
            };
        }

        private void CrearListaPedidos()
        {
            try
            {
                PedidosStackLayout.Children.Clear();

                foreach (var pedido in _pedidos)
                {
                    var frame = new Frame
                    {
                        BackgroundColor = Colors.White,
                        CornerRadius = 15,
                        HasShadow = true,
                        Padding = 15,
                        Margin = new Thickness(5)
                    };

                    var grid = new Grid
                    {
                        RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto }
                        },
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto }
                        }
                    };

                    // Estado y tipo de servicio
                    var estadoFrame = new Frame
                    {
                        BackgroundColor = Color.FromArgb(pedido.ColorEstado),
                        CornerRadius = 10,
                        HasShadow = false,
                        Padding = new Thickness(8, 4),
                        VerticalOptions = LayoutOptions.Center
                    };

                    var estadoLabel = new Label
                    {
                        Text = pedido.EstadoDescripcion,
                        TextColor = Colors.White,
                        FontFamily = "Comf-Bold",
                        FontSize = 12
                    };
                    estadoFrame.Content = estadoLabel;

                    var tipoLabel = new Label
                    {
                        Text = pedido.TipoServicio,
                        FontFamily = "Comf-Bold",
                        FontSize = 16,
                        TextColor = Color.FromArgb("#cb4335"),
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(10, 0, 0, 0)
                    };

                    var headerStack = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children = { estadoFrame, tipoLabel }
                    };

                    // Información principal
                    var detalleLabel = new Label
                    {
                        Text = pedido.DetalleCompleto,
                        FontFamily = "Comf-Medium",
                        FontSize = 14,
                        TextColor = Colors.Black
                    };

                    var periodoLabel = new Label
                    {
                        Text = pedido.PeriodoCompleto,
                        FontFamily = "Comf-Regular",
                        FontSize = 12,
                        TextColor = Colors.Gray,
                        Margin = new Thickness(0, 5, 0, 0)
                    };

                    var infoStack = new StackLayout
                    {
                        Margin = new Thickness(0, 10, 0, 0),
                        Children = { detalleLabel, periodoLabel }
                    };

                    // Descripción (si existe)
                    Label descripcionLabel = null;
                    if (!string.IsNullOrWhiteSpace(pedido.Descripcion))
                    {
                        descripcionLabel = new Label
                        {
                            Text = pedido.Descripcion,
                            FontFamily = "Comf-Regular",
                            FontSize = 12,
                            TextColor = Colors.DarkGray,
                            Margin = new Thickness(0, 5, 0, 0)
                        };
                    }

                    // Botones de acción
                    var activarBtn = new Button
                    {
                        Text = pedido.Estado ? "Desactivar" : "Activar",
                        BackgroundColor = pedido.Estado ? Color.FromArgb("#ffc107") : Color.FromArgb("#28a745"),
                        TextColor = Colors.White,
                        FontFamily = "Comf-Medium",
                        FontSize = 10,
                        Padding = new Thickness(10, 5),
                        CornerRadius = 15
                    };
                    activarBtn.Clicked += async (s, e) => await CambiarEstadoPedido(pedido);

                    var editarBtn = new Button
                    {
                        Text = "Editar",
                        BackgroundColor = Color.FromArgb("#007AFF"),
                        TextColor = Colors.White,
                        FontFamily = "Comf-Medium",
                        FontSize = 10,
                        Padding = new Thickness(10, 5),
                        CornerRadius = 15,
                        Margin = new Thickness(5, 0, 0, 0)
                    };
                    editarBtn.Clicked += async (s, e) => await EditarPedido(pedido);

                    var eliminarBtn = new Button
                    {
                        Text = "Eliminar",
                        BackgroundColor = Color.FromArgb("#dc3545"),
                        TextColor = Colors.White,
                        FontFamily = "Comf-Medium",
                        FontSize = 10,
                        Padding = new Thickness(10, 5),
                        CornerRadius = 15,
                        Margin = new Thickness(5, 0, 0, 0)
                    };
                    eliminarBtn.Clicked += async (s, e) => await EliminarPedido(pedido);

                    var botonesStack = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.End,
                        Margin = new Thickness(0, 10, 0, 0),
                        Children = { activarBtn, editarBtn, eliminarBtn }
                    };

                    // Agregar elementos al grid
                    Grid.SetRow(headerStack, 0);
                    Grid.SetColumnSpan(headerStack, 2);
                    grid.Children.Add(headerStack);

                    Grid.SetRow(infoStack, 1);
                    Grid.SetColumnSpan(infoStack, 2);
                    grid.Children.Add(infoStack);

                    if (descripcionLabel != null)
                    {
                        Grid.SetRow(descripcionLabel, 2);
                        Grid.SetColumnSpan(descripcionLabel, 2);
                        grid.Children.Add(descripcionLabel);
                    }

                    Grid.SetRow(botonesStack, 3);
                    Grid.SetColumnSpan(botonesStack, 2);
                    grid.Children.Add(botonesStack);

                    frame.Content = grid;
                    PedidosStackLayout.Children.Add(frame);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Error al crear lista: {ex.Message}", "OK");
            }
        }

        private async Task EditarPedido(PedidoAutomatico pedido)
        {
            try
            {
                await Navigation.PushAsync(new PedidoAuto(idUsuario, idtipousuario, pedido));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al abrir edición: {ex.Message}", "OK");
            }
        }

        private async Task EliminarPedido(PedidoAutomatico pedido)
        {
            try
            {
                bool confirmar = await DisplayAlert(
                    "Confirmar eliminación",
                    $"¿Está seguro de que desea eliminar el pedido automático de {pedido.TipoServicio}?",
                    "Sí", "No");

                if (!confirmar) return;

                var respuesta = await App.Database.EliminarPedidoAutomaticoAsync(pedido.IdPedidoAutomatico);

                if (respuesta.EsExitoso)
                {
                    await DisplayAlert("Éxito", "Pedido automático eliminado exitosamente.", "OK");
                    await CargarPedidosAsync(); // Recargar con el filtro actual
                }
                else
                {
                    await DisplayAlert("Error", respuesta.Mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al eliminar pedido: {ex.Message}", "OK");
            }
        }

        private async Task CambiarEstadoPedido(PedidoAutomatico pedido)
        {
            try
            {
                bool nuevoEstado = !pedido.Estado;
                string accion = nuevoEstado ? "activar" : "desactivar";

                bool confirmar = await DisplayAlert(
                    $"Confirmar {accion}",
                    $"¿Está seguro de que desea {accion} este pedido automático?",
                    "Sí", "No");

                if (!confirmar) return;

                var respuesta = await App.Database.CambiarEstadoPedidoAutomaticoAsync(
                    pedido.IdPedidoAutomatico, nuevoEstado);

                if (respuesta.EsExitoso)
                {
                    await DisplayAlert("Éxito", respuesta.Mensaje, "OK");
                    await CargarPedidosAsync(); // Recargar con el filtro actual
                }
                else
                {
                    await DisplayAlert("Error", respuesta.Mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cambiar estado: {ex.Message}", "OK");
            }
        }

        private async void Btn_atras(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MenuPrincipal(idUsuario, idtipousuario));
        }

        private async void Btn_crearNuevo(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PedidoAuto(idUsuario, idtipousuario));
        }
    }
}