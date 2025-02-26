using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;
using AppTransporte.model;

public partial class VEclientes : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    public VEclientes(int idUsuario, int idTipoUsuario)
	{
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        InitializeComponent();
        BindingContext = new ClienteViewModel();
    }

    private void btn_agregarCliente(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEagregarCliente(_idUsuario,_idTipoUsuario));
    }

    private void Btn_atrasCliente(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal(_idUsuario,_idTipoUsuario));
    }

    private async void ClienteSlect_modificar(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var clienteSelect = button.CommandParameter as Cliente;

        if (clienteSelect != null)
        {
            await Navigation.PushAsync(new VEagregarCliente(clienteSelect, _idUsuario, _idTipoUsuario));
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