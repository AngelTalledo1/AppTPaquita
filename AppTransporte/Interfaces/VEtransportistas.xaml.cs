namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;

public partial class VEtransportistas : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    public VEtransportistas(int idUsuario, int idTipoUsuario)
	{
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        BindingContext = new VMTrabajadores();
        InitializeComponent();
	}

    private void btn_agregarTransportista(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEagregarTransportista(_idUsuario, _idTipoUsuario));
    }

    private void Btn_atrasTransportista(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal(_idUsuario, _idTipoUsuario));
    }

    private async void Btn_ModificarTrabajador(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var trabajador = button.CommandParameter as Trabajador;

        if (trabajador != null)
        {
            await Navigation.PushAsync(new VEagregarTransportista(trabajador, _idUsuario, _idTipoUsuario));
        }
    }

    private async void EliminarTrabajador(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var trabajador = button.CommandParameter as Trabajador;
        bool respuesta = await DisplayAlert("Confirmación",
                                       "¿Deseas eliminar a "+ trabajador.NombreTrabajador+"?",
                                       "Sí",
                                       "No");
        if (respuesta)
        {
            var resultado = await App.Database.eliminarTrabajadorAsync(trabajador.IdTrabajador);

            if (resultado > 0)
            {
                await DisplayAlert("Exito", "Trabajador eliminado Exitosamente", "OK");
                if (BindingContext is VMTrabajadores viewModel)
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
    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        if (BindingContext is VMTrabajadores viewModel)
        {
            await viewModel.ActualizarDatos();
        }
    }
}