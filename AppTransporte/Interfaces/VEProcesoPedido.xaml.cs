using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEProcesoPedido : ContentPage
{
    private int idUsuario;
    private int idtipousuario;
    private Pedido _pedido;
    private VMSeguimientoViaje _viaje;
    private VMViajes _viewModelViajes; // Agregar referencia al ViewModel

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

        // Configurar labels
        TituloPedido.Text = $"Pedido {pedido.IdPedido}";
        Origen.Text = $"{pedido.Origen}";
        Estado.Text = $"{pedido.EstadoPedido}";
        Destino.Text = $"{pedido.Destino}";
        NombreCliente.Text = $"Cliente: {pedido.Cliente}";
        ServiciosPedido.Text = $"Servicio: {pedido.Servicios}";
        cantidadTotalPedido.Text = $"Cantidad Pedido: {pedido.Cantidad} Barriles";
        creadorAdmin.Text = $"Creado por: {pedido.Usuario}";
        creacionPedido.Text = $"Pedido Creado: {pedido.FechaSolicitud}";
        cantidadViajes.Text = $"Numero de viajes: {pedido.Viajes}";
    }

    // AGREGAR ESTE MÉTODO para recargar cuando regrese de asignar
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Recargar datos de viajes al aparecer la página
        if (_viewModelViajes != null)
        {
            await Task.Delay(500); // Pequeña pausa para asegurar que la BD se actualizó
            _viewModelViajes.InicializarViajes(); // Recargar viajes
        }
    }

    private async void Btn_atrasEstado(object sender, EventArgs e)
    {
        if (idtipousuario == 2)
        {
            await Navigation.PushAsync(new VCMisPedidos(idUsuario, idtipousuario));
        }
        else if (idtipousuario == 1)
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