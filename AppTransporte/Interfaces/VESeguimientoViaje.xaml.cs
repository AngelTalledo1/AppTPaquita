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
    }

    

    private async void Btn_atrasSeguimientoViaje(object sender, EventArgs e)
    {
        if (_idTipoUsuario == 1)
        {
            if (_pedido != null)
            {
                // Navegar a la página de VEProcesoPedido, pasando los datos del pedido seleccionado
                await Navigation.PushAsync(new VEProcesoPedido(_pedido, _idUsuario, _idTipoUsuario));
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

}