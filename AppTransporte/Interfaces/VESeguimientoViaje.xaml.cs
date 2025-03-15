namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;

public partial class VESeguimientoViaje : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    private Pedido _pedido;
    private Viaje _viaje;


    public VESeguimientoViaje(Viaje viaje,Pedido pedido, int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();

        this._idTipoUsuario = idTipoUsuario;
        this._idUsuario = idUsuario;
        this._pedido = pedido;
        this._viaje = viaje;
        this.BindingContext = new VMSeguimientoViaje(viaje.IdViaje);
        Trabajadores.Text = $"{viaje.TrabajadoresAsig}";
        tracto.Text = $"{viaje.TractoAsig}";
        Cisterna.Text = $"{viaje.CisternaAsig}";
        cantidad.Text = $"{viaje.Cantidad}";
        UltEstado.Text = viaje.ultEstado;
        Btn_Actualizar.IsVisible = mostrarActualizar(idTipoUsuario);
        btn_pedido.IsVisible = mostrarActualizar(idTipoUsuario);
    }

    

    private async void Btn_atrasSeguimientoViaje(object sender, EventArgs e)
    {
        if (_idTipoUsuario == 1)
        {
            if (_pedido != null)
            {
                // Navegar a la página de VEProcesoPedido, pasando los datos del pedido seleccionado
                await Navigation.PushAsync(new VEProcesoPedido(_pedido, _idUsuario, _idTipoUsuario,null));
            }
        }
        else if (_idTipoUsuario == 3)
        {
            await Navigation.PushAsync(new VTMisViajes(_idUsuario, _idTipoUsuario));

        }
    }

    private async void actualizarEstado(object sender, EventArgs e)
    {
        
        if (_viaje != null)
        {
            // Navegar a la página de VEProcesoPedido, pasando los datos del pedido seleccionado
            await Navigation.PushAsync(new actualizarEstado(_viaje,_idUsuario, _idTipoUsuario));
        }

    }

    private bool mostrarActualizar(int _idTipoUsuario) { 
    
        if (_idTipoUsuario == 3)
        {
            return true;
        } 
        return false;
    }

    private async void btn_verPedidoViaje(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var viaje = button.CommandParameter as VMSeguimientoViaje;

        if (viaje != null)
        {
            try
            {
                // Cargar la lista de pedidos desde la base de datos
                var pedidos = await App.Database.ListarPedidosAdminAsync();

                if (pedidos != null && pedidos.Any())
                {
                    var pedido = pedidos.FirstOrDefault(p => p.IdPedido == viaje.IdViajeSeleccionado);

                    if (pedido != null)
                    {
                        // Navegar a la página de Proceso Pedido
                        await Navigation.PushAsync(new VEProcesoPedido(pedido, _idUsuario, _idTipoUsuario,viaje));
                    }
                    else
                    {
                        // Mostrar un mensaje si no se encuentra el pedido
                        await DisplayAlert("Información", "No se encontró un pedido asociado a esta solicitud.", "OK");
                    }
                }
                else
                {
                    // Mostrar un mensaje si la lista de pedidos está vacía
                    await DisplayAlert("Información", "La lista de pedidos está vacía. Verifica si se cargaron los datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejar errores en caso de que algo falle al obtener los pedidos
                await DisplayAlert("Error", $"Ocurrió un error al cargar los pedidos: {ex.Message}", "OK");
            }
        }
    }
}