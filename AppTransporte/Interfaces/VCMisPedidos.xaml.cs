namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;


public partial class VCMisPedidos : ContentPage
{
    private readonly VMPedidos _viewModel;
    private int idUsuario;
    private int idtipousuario;
    public VCMisPedidos(int idUsuario, int idTipoUsuario)
	{
        InitializeComponent();
        this.idUsuario = idUsuario;
        this.idtipousuario = idTipoUsuario;
        _viewModel = new VMPedidos(idUsuario);
        BindingContext = _viewModel;
    }
    
    private void Btn_atrasMisPedidos(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    private async void Btn_DetalleMisPedido(object sender, EventArgs e)
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