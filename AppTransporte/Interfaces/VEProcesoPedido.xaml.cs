using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEProcesoPedido : ContentPage
{
    private int idUsuario;
    private int idtipousuario;
    private Pedido _pedido;
    private VMSeguimientoViaje _viaje;

    public VEProcesoPedido(Pedido pedido, int idUsuario, int idTipoUsuario,VMSeguimientoViaje? viaje)
    {
        InitializeComponent();
        this.idUsuario = idUsuario;
        this.idtipousuario = idTipoUsuario;
        this._pedido = pedido;
        this._viaje = viaje;

        if(idTipoUsuario == 1 || idTipoUsuario == 2)
        {
            this.BindingContext = new VMViajes(pedido.IdPedido);
        }else if (idTipoUsuario == 3)
        {
            this.BindingContext = new VMViajes(idUsuario: idUsuario);

        }
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

    private async void Btn_atrasEstado(object sender, EventArgs e)
        {
        if (idtipousuario == 2) {
            await Navigation.PushAsync(new VCMisPedidos(idUsuario, idtipousuario));
        }
        else if (idtipousuario == 1)
        {
            await Navigation.PushAsync(new VEpedidos(idUsuario, idtipousuario));

        }
        else if (idtipousuario == 3)
        {
            await Navigation.PushAsync(new VTMisViajes(idUsuario,idtipousuario));

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
            // Validar si el viaje NO está asignado
            bool noAsignado =
                string.IsNullOrWhiteSpace(viaje.TractoAsig) || viaje.TractoAsig == "S/A" ||
                string.IsNullOrWhiteSpace(viaje.CisternaAsig) || viaje.CisternaAsig == "S/A" ||
                viaje.Cantidad <= 0 ||
                string.IsNullOrWhiteSpace(viaje.TrabajadoresAsig) || viaje.TrabajadoresAsig == "S/A";

            // Revisar el tipo de usuario
            if (noAsignado)
            {
                // Si NO está asignado:
                if (idtipousuario == 1)
                {
                    // Admin -> Navega a la interfaz para ASIGNAR
                    await Navigation.PushAsync(new VEAsignarViaje(viaje,_pedido, idUsuario, idtipousuario));
                }
                else if (idtipousuario == 2)
                {
                    // Cliente -> Que NO navegue a ningún lado (no hace nada)
                    return;
                }
                else if (idtipousuario == 3)
                {
                    // Transportista -> Teóricamente ni siquiera debería ver viajes no asignados a él
                    // pero si por lógica extra apareciera, tampoco lo dejes asignar:
                    return;
                }
            }
            else
            {
                // El viaje SÍ está asignado, navega al seguimiento
                await Navigation.PushAsync(new VESeguimientoViaje(viaje, _pedido, idUsuario, idtipousuario));
            }
        }
    }

}