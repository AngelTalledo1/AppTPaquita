using AppTransporte.model;
using AppTransporte.viewModel;
using System.Collections.ObjectModel;

namespace AppTransporte.Interfaces;

public partial class VECrearPedido : ContentPage
{
    private Solicitud Solicitud { get; set; }
    private int idUsuario;
    private int idtipousuario;
    public int Viajes { get; set; }
    public VECrearPedido(Solicitud solicitud, int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();
		BindingContext = solicitud;
        this.idUsuario = idUsuario;
        this.idtipousuario = idTipoUsuario;
        Solicitud = solicitud;
        IdSolicitud.Text = solicitud.IdSolicitud.ToString();
        Descripcionlbl.Text = solicitud.Descripcion.ToString();

    }
    public ObservableCollection<ServicioSeleccionable> Servicios { get; set; } = new ObservableCollection<ServicioSeleccionable>();

  
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var origenes = await App.Database.ObtenerUbicacionesAsync();
        OrigenPicker.ItemsSource = origenes;
        OrigenPicker.ItemDisplayBinding = new Binding("Descripcion");

        var destinos = await App.Database.ObtenerUbicacionesAsync();
        DestinoPicker.ItemsSource = destinos;
        DestinoPicker.ItemDisplayBinding = new Binding("Descripcion");
        var servicios = await App.Database.ObtenerServiciosAsync();
        Servicios.Clear();

        foreach (var servicio in servicios)
        {
            Servicios.Add(new ServicioSeleccionable
            {
                IdServicio = servicio.IdServicio,
                Descripcion = servicio.Descripcion,
                Estado = servicio.Estado
            });
        }

        ServiciosCollectionView.ItemsSource = Servicios;
    }

    private async void Btn_atrasCrearPedido(object sender, EventArgs e)
	{
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud;
        if (solicitud != null)
        {
            var pedidosViewModel = this.BindingContext as VMPedidos;
            await Navigation.PushAsync(new VEDetalleSolicitud(solicitud, pedidosViewModel, idUsuario, idtipousuario));
        }
    }

    private async void Btn_crear(object sender, EventArgs e)
    {
        try
        {
            // Validar datos ingresados
            if (string.IsNullOrWhiteSpace(CantidadFluidoEntry.Text) || !int.TryParse(CantidadFluidoEntry.Text, out int cantidad) || cantidad <= 0)
            {
                await DisplayAlert("Error", "Debe ingresar una cantidad válida de barriles.", "OK");
                return;
            }

            if (OrigenPicker.SelectedItem == null || DestinoPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Debe seleccionar un origen y un destino.", "OK");
                return;
            }

            // Obtener los servicios seleccionados
            var serviciosSeleccionados = Servicios.Where(s => s.IsSelected).ToList();
            if (serviciosSeleccionados.Count == 0)
            {
                await DisplayAlert("Error", "Debe seleccionar al menos un servicio.", "OK");
                return;
            }
            var listaServicios = string.Join(",", serviciosSeleccionados.Select(s => s.Descripcion));
            int idSolicitud = Solicitud.IdSolicitud;
            int viajes = Viajes;
            int idOrigen = ((Ubicacion)OrigenPicker.SelectedItem).IdUbicacion;
            int idDestino = ((Ubicacion)DestinoPicker.SelectedItem).IdUbicacion;
            int idEstadoPedido = 1;

            // Llamar al servicio para crear el pedido
            await App.Database.CrearPedidoAsync(
                idSolicitud,
                cantidad,
                viajes,
                idOrigen,
                idDestino,
                idEstadoPedido,
                listaServicios);

            await DisplayAlert("Éxito", "Pedido creado exitosamente.", "OK");

            // Limpiar los campos después de crear el pedido
            await Navigation.PushAsync(new VESolicitudes(idUsuario, idtipousuario));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al crear el pedido: {ex.Message}", "OK");
        }
    }
    private async void Btn_cancelar(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud;
        if (solicitud != null)
        {
            var pedidosViewModel = this.BindingContext as VMPedidos;
            await Navigation.PushAsync(new VEDetalleSolicitud(solicitud, pedidosViewModel, idUsuario, idtipousuario));
        }
    }

    private void OnCantidadFluidoChanged(object sender, TextChangedEventArgs e)
    {
        const int capacidadMaxima = 200;
        if (int.TryParse(CantidadFluidoEntry.Text, out int cantidadFluido))
        {
            // Calcula los viajes necesarios y actualiza el Label
            int viajes = (int)Math.Ceiling((double)cantidadFluido / capacidadMaxima);
            ViajesLabel.Text = $"Viajes: {viajes}";
            Viajes = viajes;
        }
        else
        {
            // Si no es un número válido, resetea el valor
            ViajesLabel.Text = "Viajes: 0";
        }
    }

    }