using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEAsignarViaje : ContentPage
{
    private int idUsuario;
    private int idtipousuario;
    private Pedido _pedido;
    public VEAsignarViaje(Viaje viaje,Pedido pedido, int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();
		
        this.idUsuario = idUsuario;
        this.idtipousuario = idTipoUsuario;
        this._pedido = pedido;
        Id_Pedido.Text = $"ID Pedido: { viaje.IdPedido.ToString()}";
        Id_viaje.Text = $"ID Viaje: {viaje.IdViaje.ToString()}";
    }

    private async void Btn_atrasAsignarViaje(object sender, EventArgs e)
    {
        
        var button = (Button)sender;
        var pedido = button.CommandParameter as Pedido;
        

        if (pedido != null)
        {
            await Navigation.PushAsync(new VEProcesoPedido(_pedido, idUsuario, idtipousuario,null));
        }
    }
}