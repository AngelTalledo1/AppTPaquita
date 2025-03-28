using AppTransporte.model;
using System.Collections.ObjectModel;

namespace AppTransporte.Interfaces;

public partial class Calendar : ContentPage
{
	

    private int _idUsuario;
    private int _idTipoUsuario;
    private DateTime _periodoActual;
    private bool _esModoMes = false;
    private ObservableCollection<Pedido1> _pedidosProgramados;
    private bool _isLoading = false;

    // Diccionario para almacenar los eventos por d�a
    private Dictionary<DateTime, List<Pedido1>> _eventosPorDia = new Dictionary<DateTime, List<Pedido1>>();

    // Colores para los diferentes tipos de servicio
    private readonly Dictionary<string, string> _coloresTipoServicio = new Dictionary<string, string>
        {
            { "Regado", "#4285f4" },            // Azul de Google
            { "Transporte de Fluido", "#0f9d58" }, // Verde de Google
            { "Suministro de Agua", "#f4b400" },   // Amarillo de Google
            { "Transporte de Agua", "#db4437" }    // Rojo de Google
        };

    public Calendar(int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;
        _pedidosProgramados = new ObservableCollection<Pedido1>();

        // Inicializar con la fecha actual
        _periodoActual = DateTime.Today;

        // Establecer el modo inicial como semanal
        ModoPicker.SelectedIndex = 0;

        // Actualizar la vista del calendario
        ActualizarVistaPeriodo();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarPedidosProgramados();
    }

    private async void Btn_atras(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async Task CargarPedidosProgramados()
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            // Mostrar indicador de carga
            IsBusy = true;

            // En una implementaci�n real, cargar�as los pedidos programados desde tu base de datos
            // Aqu� usar�a App.Database para obtener los pedidos, algo como:
            // var pedidos = await App.Database.ObtenerPedidosProgramadosAsync(_idUsuario);

            // Por ahora, simularemos algunos datos
            await Task.Delay(500); // Simular tiempo de carga

            // Limpiamos el diccionario de eventos
            _eventosPorDia.Clear();

            // En una implementaci�n real, llenar�as el diccionario con los datos obtenidos de la BD
            // Por ahora, a�adimos algunos datos de ejemplo
            AgregarPedidosEjemplo();

            // Actualizar la vista del calendario con los eventos cargados
            ActualizarVistaPeriodo();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los pedidos programados: {ex.Message}", "Aceptar");
        }
        finally
        {
            IsBusy = false;
            _isLoading = false;
        }
    }

    // M�todo para a�adir pedidos de ejemplo (en implementaci�n real, esto vendr�a de la base de datos)
    private void AgregarPedidosEjemplo()
    {
        // Ejemplo: Agregar algunos pedidos programados para este mes
        var hoy = DateTime.Today;
        var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);
        var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

        // Ejemplo 1: Pedido de regado para hoy
        var pedido1 = new Pedido1
        {
            IdPedido = 1,
            EstadoPedido = "Pendiente",
            FechaProgramada = hoy,
            HoraProgramada = new TimeSpan(9, 0, 0),
            Descripcion = "Regado",
            Detalle = "9:00 AM"
        };

        // Ejemplo 2: Pedido de transporte para ma�ana
        var pedido2 = new Pedido1
        {
            IdPedido = 2,
            EstadoPedido = "Pendiente",
            FechaProgramada = hoy.AddDays(1),
            HoraProgramada = new TimeSpan(14, 30, 0),
            Descripcion = "Transporte de Fluido",
            Detalle = "2:30 PM"
        };

        // Ejemplo 3: Pedido para el d�a 15 del mes
        var pedido3 = new Pedido1
        {
            IdPedido = 3,
            EstadoPedido = "Pendiente",
            FechaProgramada = new DateTime(hoy.Year, hoy.Month, 15),
            HoraProgramada = new TimeSpan(11, 0, 0),
            Descripcion = "Suministro de Agua",
            Detalle = "11:00 AM"
        };

        // A�adir los pedidos al diccionario agrupados por fecha
        AgregarEventoAlDiccionario(pedido1);
        AgregarEventoAlDiccionario(pedido2);
        AgregarEventoAlDiccionario(pedido3);
    }

    private void AgregarEventoAlDiccionario(Pedido1 pedido)
    {
        var fecha = pedido.FechaProgramada.Date;

        if (!_eventosPorDia.ContainsKey(fecha))
        {
            _eventosPorDia[fecha] = new List<Pedido1>();
        }

        _eventosPorDia[fecha].Add(pedido);
    }

    private void ActualizarVistaPeriodo()
    {
        if (_esModoMes)
        {
            // Actualizar etiqueta con el mes y a�o
            PeriodoActualLabel.Text = _periodoActual.ToString("MMMM yyyy");

            // Mostrar vista de mes
            VistaMesCont.IsVisible = true;
            VistaSemanaCont.IsVisible = false;

            ActualizarVistaMes();
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
    }

    private DateTime ObtenerInicioDeSemana(DateTime fecha)
    {
        // Buscar el domingo de la semana (DayOfWeek.Sunday = 0)
        int diff = (7 + (int)fecha.DayOfWeek) % 7;
        return fecha.AddDays(-diff);
    }

    private void ActualizarVistaSemana(DateTime inicioSemana)
    {
        // Limpiar eventos previos
        LimpiarEventosSemana();

        // Contenedores para cada d�a de la semana
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

        // Para cada d�a de la semana
        for (int i = 0; i < 7; i++)
        {
            DateTime fechaDia = inicioSemana.AddDays(i);
            var contenedor = contenedoresDias[i];

            // Actualizar la etiqueta con el n�mero del d�a
            var labelDia = contenedor.Children.OfType<Label>().FirstOrDefault();
            if (labelDia != null)
            {
                labelDia.Text = fechaDia.Day.ToString();

                // Resaltar el d�a actual
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

            // Agregar eventos para este d�a
            if (_eventosPorDia.ContainsKey(fechaDia.Date))
            {
                foreach (var pedido in _eventosPorDia[fechaDia.Date])
                {
                    // Determinar el color seg�n el tipo de servicio
                    string colorEvento = "#4285f4"; // Color predeterminado
                    if (_coloresTipoServicio.ContainsKey(pedido.Descripcion))
                    {
                        colorEvento = _coloresTipoServicio[pedido.Descripcion];
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
                    stackLayout.Add(new Label
                    {
                        Text = pedido.Descripcion,
                        TextColor = Colors.White,
                        FontSize = 12,
                        FontFamily = "Comf-Medium"
                    });
                    stackLayout.Add(new Label
                    {
                        Text = pedido.Detalle,
                        TextColor = Colors.White,
                        FontSize = 10
                    });

                    frame.Content = stackLayout;

                    // Agregar gesto de tap para ver detalles
                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += (s, e) => VerDetallesPedido(pedido);
                    frame.GestureRecognizers.Add(tapGesture);

                    // Agregar el evento al contenedor del d�a
                    contenedor.Add(frame);
                }
            }
        }
    }

    private void LimpiarEventosSemana()
    {
        // Limpiar los eventos pero preservar las etiquetas de los d�as y l�neas separadoras
        foreach (var columna in new[] { DiaColumn0, DiaColumn1, DiaColumn2, DiaColumn3, DiaColumn4, DiaColumn5, DiaColumn6 })
        {
            // Mantener solo los primeros dos elementos (t�tulo del d�a y l�nea separadora)
            while (columna.Children.Count > 2)
            {
                columna.RemoveAt(columna.Children.Count - 1);
            }
        }
    }

    private void ActualizarVistaMes()
    {
        // Limpiar la cuadr�cula del mes
        MesGrid.Children.Clear();

        // Obtener el primer d�a del mes
        DateTime primerDiaMes = new DateTime(_periodoActual.Year, _periodoActual.Month, 1);

        // Determinar en qu� d�a de la semana cae el primer d�a del mes
        int diaSemana = (int)primerDiaMes.DayOfWeek; // 0 = Domingo, 1 = Lunes, etc.

        // Determinar el n�mero de d�as del mes
        int diasEnMes = DateTime.DaysInMonth(primerDiaMes.Year, primerDiaMes.Month);

        // Variables para seguimiento de posici�n
        int fila = 0;
        int columna = diaSemana;

        // Para cada d�a del mes
        for (int dia = 1; dia <= diasEnMes; dia++)
        {
            DateTime fecha = new DateTime(primerDiaMes.Year, primerDiaMes.Month, dia);

            // Crear un borde para este d�a
            var border = new Border
            {
                Stroke = Color.FromArgb("#e0e0e0"),
                StrokeThickness = 0.5,
                Padding = new Thickness(2),
                BackgroundColor = Colors.Transparent
            };

            // Configurar el grid dentro del borde
            var gridDia = new Grid { RowDefinitions = new RowDefinitionCollection { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) } };

            // T�tulo del d�a
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
            var scrollView = new ScrollView { MaximumHeightRequest = 80};
            var eventosStack = new VerticalStackLayout();
            scrollView.Content = eventosStack;

            // Agregar eventos para este d�a
            if (_eventosPorDia.ContainsKey(fecha.Date))
            {
                foreach (var pedido in _eventosPorDia[fecha.Date])
                {
                    // Determinar color
                    string colorEvento = "#4285f4"; // Color predeterminado
                    if (_coloresTipoServicio.ContainsKey(pedido.Descripcion))
                    {
                        colorEvento = _coloresTipoServicio[pedido.Descripcion];
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

                    // Texto del evento
                    var horaFormatted = pedido.HoraProgramada.Hours >= 12
                        ? $"{(pedido.HoraProgramada.Hours == 12 ? 12 : pedido.HoraProgramada.Hours - 12)}:{pedido.HoraProgramada.Minutes:D2} PM"
                        : $"{pedido.HoraProgramada.Hours}:{pedido.HoraProgramada.Minutes:D2} AM";

                    frame.Content = new Label
                    {
                        Text = $"{pedido.Descripcion} {horaFormatted}",
                        TextColor = Colors.White,
                        FontSize = 10
                    };

                    // Agregar gesto para ver detalles
                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += (s, e) => VerDetallesPedido(pedido);
                    frame.GestureRecognizers.Add(tapGesture);

                    eventosStack.Add(frame);
                }
            }

            // A�adir el scrollview al grid del d�a
            gridDia.Add(scrollView);

            // Asignar el grid como contenido del borde
            border.Content = gridDia;

            // A�adir el borde a la cuadr�cula del mes en la posici�n correspondiente
            MesGrid.Add(border, columna, fila);

            // Actualizar posici�n
            columna++;
            if (columna > 6)
            {
                columna = 0;
                fila++;
            }
        }
    }

    private async void VerDetallesPedido(Pedido1 pedido)
    {
        // Aqu� implementar�as la navegaci�n a la p�gina de detalles del pedido
        await DisplayAlert("Detalles del Pedido",
            $"Pedido #{pedido.IdPedido}\n" +
            $"Tipo: {pedido.Descripcion}\n" +
            $"Fecha: {pedido.FechaProgramada:dd/MM/yyyy}\n" +
            $"Hora: {pedido.HoraProgramada:hh\\:mm tt}\n" +
            $"Estado: {pedido.EstadoPedido}",
            "Cerrar");

        // En una implementaci�n real, navegar�as a una p�gina de detalles:
        // await Navigation.PushAsync(new DetallePedidoProgramadoPage(pedido));
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
        await CargarPedidosProgramados();
        refreshView.IsRefreshing = false;
    }

    private async void AgregarPedido_Clicked(object sender, EventArgs e)
    {
        // Navegar a la p�gina para crear un nuevo pedido programado
        await Navigation.PushAsync(new PedidoAuto(_idUsuario, _idTipoUsuario));
    }
}
	
