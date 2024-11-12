
namespace AppTransporte.Interfaces;

public partial class MenuCliente : ContentPage
{
	public MenuCliente()
	{
		InitializeComponent();
	}

    private void Btn_atrasMenuCliente(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    
        private void NuevoPedido_Clicked(object sender, EventArgs e)
    {
        //Navigation.PushAsync(new VCnuevoPedidos());
    }
    private void pedidosCliente_Clicked(object sender, EventArgs e)
    {
        //Navigation.PushAsync(new VCpedidos());
    }

}