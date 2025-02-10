namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;


public partial class VEpedidos : ContentPage
{
    private readonly VMPedidos _viewModel;
    private int idUsuario;
    private int idtipousuario;
    public VEpedidos(int idUsuario, int idTipoUsuario)
	{
        this.idUsuario = idUsuario;
        this.idtipousuario = idTipoUsuario;
        InitializeComponent();
        _viewModel = new VMPedidos();
        BindingContext = _viewModel;
    }


    private void Btn_atrasPedidos(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal());
    }

    private async void Btn_DetallePedido(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var pedido = button.CommandParameter as Pedido; // Asegúrate de que "Pedidos" es el tipo de tu modelo de datos

        if (pedido != null)
        {
            // Navegar a la página de VEProcesoPedido, pasando los datos del pedido seleccionado
            await Navigation.PushAsync(new VEProcesoPedido(pedido));
        }
    }
}