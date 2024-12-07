using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;
using AppTransporte.model;

public partial class VEclientes : ContentPage
{
	public VEclientes()
	{
        
        InitializeComponent();
        BindingContext = new ClienteViewModel();
    }

    private void btn_agregarCliente(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEagregarCliente());
    }

    private void Btn_atrasCliente(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal());
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

    private async void Btn_EliminarCliente(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var cliente = button.CommandParameter as Cliente;
        bool respuesta = await DisplayAlert("Confirmación",
                                       "¿Deseas eliminar a " + cliente.Nombre + "?",
                                       "Sí",
                                       "No");
        if (respuesta)
        {
            var resultado = await App.Database.eliminarClienteAsync(cliente.IdCliente);

            if (resultado > 0)
            {
                await DisplayAlert("Exito", "Cliente eliminado Exitosamente", "OK");
                if (BindingContext is ClienteViewModel viewModel)
                {
                    await viewModel.ActualizarDatos();
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar el trabajador. Verifica los datos.", "OK");
            }
        }
    }
}