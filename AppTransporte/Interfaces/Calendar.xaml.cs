using AppTransporte.model;
using System.Collections.ObjectModel;
using Microsoft.Maui.Dispatching;

namespace AppTransporte.Interfaces;

public partial class Calendar : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    private DateTime _periodoActual;
    private bool _esModoMes = false;
    private ObservableCollection<Pedido> _pedidos;
    private bool _isLoading = false;

    // Diccionario para almacenar los eventos por día
    private Dictionary<DateTime, List<Pedido>> _eventosPorDia = new Dictionary<DateTime, List<Pedido>>();

    // Caché para las vistas de los meses
    private Dictionary<string, List<Border>> _cacheMeses = new Dictionary<string, List<Border>>();

    // Lista completa de pedidos
    private List<Pedido> _todosPedidos = new List<Pedido>();

    // Flag para evitar bucle infinito en los cambios de mes/año
    private bool _isManualPeriodChange = false;

    // Colores para los diferentes estados de pedido
    private readonly Dictionary<string, string> _coloresEstadoPedido = new Dictionary<string, string>
    {
        { "Pendiente", "#4285f4" },            // Azul
        { "En el punto de Carga", "#f4b400" }, // Amarillo
        { "En camino al destino", "#0f9d58" }, // Verde
        { "Finalizado", "#db4437" }            // Rojo
    };

    public Calendar(int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;
        _pedidos = new ObservableCollection<Pedido>();

        // Inicializar con la fecha actual
        _periodoActual = DateTime.Today;

        // Establecer el modo inicial como semanal
        ModoPicker.SelectedIndex = 0;

        // Inicializar los selectores de mes y año
        ConfigurarSelectoresFecha();
    }

    private void ConfigurarSelectoresFecha()
    {
        // Configurar picker de meses
        var nombresMeses = new List<string>
        {
            "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
        };

        foreach (var mes in nombresMeses)
        {
            MesPicker.Items.Add(mes);
        }

        // Configurar picker de años (desde 5 años atrás hasta 5 años adelante)
        int anioActual = DateTime.Now.Year;
        for (int anio = anioActual - 5; anio <= anioActual + 5; anio++)
        {
            AnioPicker.Items.Add(anio.ToString());
        }

        // Establecer los valores actuales
        _isManualPeriodChange = true;
        MesPicker.SelectedIndex = _periodoActual.Month - 1; // Los meses en C# van de 1-12, pero en el picker de 0-11
        AnioPicker.SelectedIndex = 5; // El año actual está en medio de la lista (posición 5)
        _isManualPeriodChange = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarTodosPedidos();
        ActualizarVistaPeriodo();
    }

    private async void Btn_atras(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // Carga todos los pedidos una sola vez y los almacena en memoria
    private async Task CargarTodosPedidos()
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            // Mostrar indicador de carga
            MostrarIndicadorCarga(true);

            // Obtener los pedidos del usuario actual
            _todosPedidos = await App.Database.ListarPedidosAdminAsync();

            // Procesar los pedidos y organizarlos por fecha
            ActualizarEventosPorDia();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los pedidos: {ex.Message}", "Aceptar");
        }
        finally
        {
            MostrarIndicadorCarga(false);
            _isLoading = false;
        }
    }

    // Método para mostrar/ocultar el indicador de carga personalizado
    private void MostrarIndicadorCarga(bool mostrar)
    {
        Dispatcher.Dispatch(() =>
        {
            LoadingOverlay.IsVisible = mostrar;
            // Mantener la compatibilidad con el binding existente
        });
    }

    // Organiza los pedidos por fecha para acceso rápido
    private void ActualizarEventosPorDia()
    {
        // Limpiar el diccionario existente
        _eventosPorDia.Clear();

        // Reorganizar todos los pedidos por fecha
        foreach (var pedido in _todosPedidos)
        {
            // Determinar qué fecha usar para el calendario
            DateTime fechaEvento;

            // Si la fecha de entrega está establecida, usarla; de lo contrario, usar la fecha de solicitud
            if (pedido.FechaEntrega.HasValue)
            {
                fechaEvento = pedido.FechaEntrega.Value.Date;
            }
            else
            {
                fechaEvento = pedido.FechaSolicitud.Date;
            }

            // Agregar el pedido al diccionario
            if (!_eventosPorDia.ContainsKey(fechaEvento))
            {
                _eventosPorDia[fechaEvento] = new List<Pedido>();
            }
            _eventosPorDia[fechaEvento].Add(pedido);
        }
    }

    private void ActualizarVistaPeriodo()
    {
        if (_esModoMes)
        {
            // Actualizar etiqueta con el mes y año
            PeriodoActualLabel.Text = _periodoActual.ToString("MMMM yyyy");

            // Mostrar vista de mes
            VistaMesCont.IsVisible = true;
            VistaSemanaCont.IsVisible = false;

            // Actualizar la vista del mes de manera optimizada
            ActualizarVistaMesOptimizada();
        }
        else
        {
            // Para la vista semanal
            DateTime inicioSemana = ObtenerInicioDeSemana(_periodoActual);
            DateTime finSemana = inicioSemana.AddDays(6);

            // Actualizar etiqueta con rango de fechas
            PeriodoActualLabel.Text = $"{inicioSemana:dd MMM} - {finSemana:dd MMM yyyy}";

            // Mostrar vista de semana
            VistaMesCont.IsVisible = false;
            VistaSemanaCont.IsVisible = true;

            ActualizarVistaSemana(inicioSemana);
        }

        // Actualizar los selectores de mes/año sin desencadenar eventos
        if (!_isManualPeriodChange)
        {
            _isManualPeriodChange = true;
            MesPicker.SelectedIndex = _periodoActual.Month - 1;

            // Calcular el índice del año en el picker
            int anioActual = DateTime.Now.Year;
            int indiceAnio = _periodoActual.Year - (anioActual - 5);
            if (indiceAnio >= 0 && indiceAnio < AnioPicker.Items.Count)
            {
                AnioPicker.SelectedIndex = indiceAnio;
            }
            _isManualPeriodChange = false;
        }
    }

    private DateTime ObtenerInicioDeSemana(DateTime fecha)
    {
        // Buscar el domingo de la semana (DayOfWeek.Sunday = 0)
        int diff = (7 + (int)fecha.DayOfWeek) % 7;
        return fecha.AddDays(-diff);
    }

    private void ActualizarVistaSemana(DateTime inicioSemana)
    {
        // Usar Dispatcher para operaciones de UI en el hilo principal
        Dispatcher.Dispatch(() => {
            // Limpiar eventos previos
            LimpiarEventosSemana();

            // Contenedores para cada día de la semana
            var contenedoresDias = new[]
            {
                DiaColumn0,
                DiaColumn1,
                DiaColumn2,
                DiaColumn3,
                DiaColumn4,
                DiaColumn5,
                DiaColumn6
            };

            // Para cada día de la semana
            for (int i = 0; i < 7; i++)
            {
                DateTime fechaDia = inicioSemana.AddDays(i);
                var contenedor = contenedoresDias[i];

                // Actualizar la etiqueta con el número del día
                var labelDia = contenedor.Children.OfType<Label>().FirstOrDefault();
                if (labelDia != null)
                {
                    labelDia.Text = fechaDia.Day.ToString();

                    // Resaltar el día actual
                    if (fechaDia.Date == DateTime.Today)
                    {
                        labelDia.FontAttributes = FontAttributes.Bold;
                        labelDia.TextColor = Color.FromArgb("#cb4335");
                    }
                    else
                    {
                        labelDia.FontAttributes = FontAttributes.None;
                        labelDia.TextColor = Colors.Black;
                    }
                }

                // Agregar eventos para este día
                if (_eventosPorDia.ContainsKey(fechaDia.Date))
                {
                    foreach (var pedido in _eventosPorDia[fechaDia.Date])
                    {
                        // Determinar el color según el estado del pedido
                        string colorEvento = "#4285f4"; // Color predeterminado (azul)
                        if (_coloresEstadoPedido.ContainsKey(pedido.EstadoPedido))
                        {
                            colorEvento = _coloresEstadoPedido[pedido.EstadoPedido];
                        }

                        // Crear el frame para el evento
                        var frame = new Frame
                        {
                            Margin = new Thickness(2, 5),
                            Padding = new Thickness(8, 5),
                            BackgroundColor = Color.FromArgb(colorEvento),
                            CornerRadius = 4,
                            HasShadow = false
                        };

                        // Crear el contenido del evento
                        var stackLayout = new VerticalStackLayout { Spacing = 2 };

                        // Primera línea: Servicio
                        stackLayout.Add(new Label
                        {
                            Text = pedido.Servicios,
                            TextColor = Colors.White,
                            FontSize = 12,
                            FontFamily = "Comf-Medium"
                        });

                        // Segunda línea: Origen - Destino
                        stackLayout.Add(new Label
                        {
                            Text = $"{pedido.Origen} → {pedido.Destino}",
                            TextColor = Colors.White,
                            FontSize = 10
                        });

                        frame.Content = stackLayout;

                        // Agregar gesto de tap para ver detalles
                        var tapGesture = new TapGestureRecognizer();
                        int idPedidoCapturado = pedido.IdPedido; // Captura local para evitar problemas con closures
                        tapGesture.Tapped += (s, e) => VerDetallesPedido(_todosPedidos.Find(p => p.IdPedido == idPedidoCapturado));
                        frame.GestureRecognizers.Add(tapGesture);

                        // Agregar el evento al contenedor del día
                        contenedor.Add(frame);
                    }
                }
            }
        });
    }

    private void LimpiarEventosSemana()
    {
        // Limpiar los eventos pero preservar las etiquetas de los días y líneas separadoras
        foreach (var columna in new[] { DiaColumn0, DiaColumn1, DiaColumn2, DiaColumn3, DiaColumn4, DiaColumn5, DiaColumn6 })
        {
            // Mantener solo los primeros dos elementos (título del día y línea separadora)
            while (columna.Children.Count > 2)
            {
                columna.RemoveAt(columna.Children.Count - 1);
            }
        }
    }

    private void ActualizarVistaMesOptimizada()
    {
        try
        {
            // Mostrar el indicador de carga mientras se actualiza el mes
            MostrarIndicadorCarga(true);

            string mesKey = $"{_periodoActual.Year}-{_periodoActual.Month}";

            // Verificar si este mes ya está en caché
            if (_cacheMeses.TryGetValue(mesKey, out var diasDelMes))
            {
                // Usar la vista en caché para mejor rendimiento
                Dispatcher.Dispatch(() => {
                    MesGrid.Children.Clear();
                    foreach (var borde in diasDelMes)
                    {
                        var fila = Grid.GetRow(borde);
                        var columna = Grid.GetColumn(borde);
                        MesGrid.Add(borde, columna, fila);
                    }
                    // Ocultar indicador de carga al terminar
                    MostrarIndicadorCarga(false);
                });
                return;
            }

            // Generar la vista del mes en un hilo separado
            Task.Run(async () => {
                var bordersGenerados = await GenerarBordesMes();

                // Guardar en caché
                _cacheMeses[mesKey] = bordersGenerados;

                // Actualizar UI en el hilo principal
                await Dispatcher.DispatchAsync(() => {
                    MesGrid.Children.Clear();
                    foreach (var borde in bordersGenerados)
                    {
                        var fila = Grid.GetRow(borde);
                        var columna = Grid.GetColumn(borde);
                        MesGrid.Add(borde, columna, fila);
                    }
                    // Ocultar indicador de carga al terminar
                    MostrarIndicadorCarga(false);
                });
            });
        }
        catch (Exception)
        {

            throw;
        }
        
    }

    private Task<List<Border>> GenerarBordesMes()
    {
        // Lista para almacenar todos los bordes generados
        var bordes = new List<Border>();

        // Obtener el primer día del mes
        DateTime primerDiaMes = new DateTime(_periodoActual.Year, _periodoActual.Month, 1);

        // Determinar en qué día de la semana cae el primer día del mes
        int diaSemana = (int)primerDiaMes.DayOfWeek; // 0 = Domingo, 1 = Lunes, etc.

        // Determinar el número de días del mes
        int diasEnMes = DateTime.DaysInMonth(primerDiaMes.Year, primerDiaMes.Month);

        // Variables para seguimiento de posición
        int fila = 0;
        int columna = diaSemana;

        // Para cada día del mes
        for (int dia = 1; dia <= diasEnMes; dia++)
        {
            DateTime fecha = new DateTime(primerDiaMes.Year, primerDiaMes.Month, dia);

            // Crear un borde para este día
            var border = new Border
            {
                Stroke = Color.FromArgb("#e0e0e0"),
                StrokeThickness = 0.5,
                Padding = new Thickness(2),
                BackgroundColor = Colors.Transparent
            };

            // Guardar su posición como propiedades adjuntas
            Grid.SetRow(border, fila);
            Grid.SetColumn(border, columna);

            // Configurar el grid dentro del borde
            var gridDia = new Grid
            {
                RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star)
                }
            };

            // Título del día
            var labelDia = new Label
            {
                Text = dia.ToString(),
                FontSize = 12,
                Margin = new Thickness(5, 2),
                FontAttributes = fecha.Date == DateTime.Today ? FontAttributes.Bold : FontAttributes.None,
                TextColor = fecha.Date == DateTime.Today ? Color.FromArgb("#cb4335") : Colors.Black
            };
            gridDia.Add(labelDia);

            // Contenedor para eventos
            var scrollView = new ScrollView { MaximumHeightRequest = 80 };
            Grid.SetRow(scrollView, 1);
            var eventosStack = new VerticalStackLayout();
            scrollView.Content = eventosStack;

            // Agregar eventos para este día
            if (_eventosPorDia.ContainsKey(fecha.Date))
            {
                foreach (var pedido in _eventosPorDia[fecha.Date])
                {
                    // Determinar color según el estado del pedido
                    string colorEvento = "#4285f4"; // Color predeterminado
                    if (_coloresEstadoPedido.ContainsKey(pedido.EstadoPedido))
                    {
                        colorEvento = _coloresEstadoPedido[pedido.EstadoPedido];
                    }

                    // Crear frame para el evento
                    var frame = new Frame
                    {
                        BackgroundColor = Color.FromArgb(colorEvento),
                        Padding = new Thickness(5, 2),
                        Margin = new Thickness(2),
                        CornerRadius = 2,
                        HasShadow = false
                    };

                    // Texto del evento: Servicio principal (primer servicio de la lista si hay varios)
                    string servicioTexto = pedido.Servicios;
                    if (servicioTexto.Contains(","))
                    {
                        servicioTexto = servicioTexto.Split(',')[0].Trim();
                    }

                    frame.Content = new Label
                    {
                        Text = $"{servicioTexto} ({pedido.Origen}-{pedido.Destino})",
                        TextColor = Colors.White,
                        FontSize = 10,
                        LineBreakMode = LineBreakMode.TailTruncation
                    };

                    // Agregar gesto para ver detalles
                    var tapGesture = new TapGestureRecognizer();
                    int idPedidoCapturado = pedido.IdPedido;
                    tapGesture.Tapped += (s, e) => VerDetallesPedido(_todosPedidos.Find(p => p.IdPedido == idPedidoCapturado));
                    frame.GestureRecognizers.Add(tapGesture);

                    eventosStack.Add(frame);
                }
            }

            // Añadir el scrollview al grid del día
            gridDia.Add(scrollView);

            // Asignar el grid como contenido del borde
            border.Content = gridDia;

            // Añadir el borde a la lista
            bordes.Add(border);

            // Actualizar posición
            columna++;
            if (columna > 6)
            {
                columna = 0;
                fila++;
            }
        }

        return Task.FromResult(bordes);
    }

    private async void VerDetallesPedido(Pedido pedido)
    {
        if (pedido == null) return;

        // Mostrar los detalles del pedido en un diálogo
        string fechaEntrega = pedido.FechaEntrega.HasValue ?
            pedido.FechaEntrega.Value.ToString("dd/MM/yyyy") : "Pendiente";

        await DisplayAlert("Detalles del Pedido",
            $"Pedido #{pedido.IdPedido}\n" +
            $"Servicios: {pedido.Servicios}\n" +
            $"Origen: {pedido.Origen} ({pedido.OrigSector})\n" +
            $"Destino: {pedido.Destino} ({pedido.DestSector})\n" +
            $"Cantidad: {pedido.Cantidad} barriles\n" +
            $"Fecha solicitud: {pedido.FechaSolicitud:dd/MM/yyyy}\n" +
            $"Fecha entrega: {fechaEntrega}\n" +
            $"Estado: {pedido.EstadoPedido}",
            "Cerrar");
    }

    private void PrevPeriod_Clicked(object sender, EventArgs e)
    {
        if (_esModoMes)
        {
            _periodoActual = _periodoActual.AddMonths(-1);
        }
        else
        {
            _periodoActual = _periodoActual.AddDays(-7);
        }

        ActualizarVistaPeriodo();
    }

    private void NextPeriod_Clicked(object sender, EventArgs e)
    {
        if (_esModoMes)
        {
            _periodoActual = _periodoActual.AddMonths(1);
        }
        else
        {
            _periodoActual = _periodoActual.AddDays(7);
        }

        ActualizarVistaPeriodo();
    }

    private void OnModoChanged(object sender, EventArgs e)
    {
        if (ModoPicker.SelectedIndex < 0) return;

        _esModoMes = ModoPicker.SelectedIndex == 1; // 0 = Semana, 1 = Mes
        ActualizarVistaPeriodo();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        // Limpiar la caché al refrescar
        _cacheMeses.Clear();

        await CargarTodosPedidos();
        ActualizarVistaPeriodo();
        refreshView.IsRefreshing = false;
    }

    // Manejar cambio en selector de mes
    private void MesPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_isManualPeriodChange || MesPicker.SelectedIndex < 0)
            return;

        // Construir nueva fecha con el mes seleccionado
        int nuevoMes = MesPicker.SelectedIndex + 1; // Ajustar índice a número de mes (1-12)
        try
        {
            _periodoActual = new DateTime(_periodoActual.Year, nuevoMes, 1);
            ActualizarVistaPeriodo();
        }
        catch (Exception ex)
        {
            // Si hay un error (días inválidos en el mes, etc.), ajustar al último día
            _periodoActual = new DateTime(_periodoActual.Year, nuevoMes, 1).AddMonths(1).AddDays(-1);
            ActualizarVistaPeriodo();
        }
    }

    // Manejar cambio en selector de año
    private void AnioPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_isManualPeriodChange || AnioPicker.SelectedIndex < 0)
            return;

        // Obtener el año seleccionado
        if (int.TryParse(AnioPicker.Items[AnioPicker.SelectedIndex], out int nuevoAnio))
        {
            try
            {
                _periodoActual = new DateTime(nuevoAnio, _periodoActual.Month, 1);
                ActualizarVistaPeriodo();
            }
            catch (Exception ex)
            {
                // Si hay un error, ajustar al último día del mes en ese año
                _periodoActual = new DateTime(nuevoAnio, _periodoActual.Month, 1).AddMonths(1).AddDays(-1);
                ActualizarVistaPeriodo();
            }
        }
    }

    private async void AgregarPedido_Clicked(object sender, EventArgs e)
    {
        // Navegar a la página para crear un nuevo pedido
        await Navigation.PushAsync(new PedidoAuto(_idUsuario, _idTipoUsuario));
    }
}
