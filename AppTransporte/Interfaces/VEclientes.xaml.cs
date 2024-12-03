using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;
using AppTransporte.model;

public partial class VEclientes : ContentPage
{
	public VEclientes()
	{
        BindingContext = new ClienteViewModel();
        InitializeComponent();
	}

    private void btn_agregarCliente(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEagregarCliente());
    }

    private void Btn_atrasCliente(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void ClienteSlect_modificar(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var clienteSelect = button.CommandParameter as Cliente;

        if (clienteSelect != null)
        {
            await Navigation.PushAsync(new VEagregarCliente(clienteSelect));
        }
    }

    private void Btn_EliminarCliente(object sender, EventArgs e)
    {

    }
}