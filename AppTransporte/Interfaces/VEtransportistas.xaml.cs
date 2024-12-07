namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;

public partial class VEtransportistas : ContentPage
{
	public VEtransportistas()
	{
        BindingContext = new VMTrabajadores();
        InitializeComponent();
	}

    private void btn_agregarTransportista(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEagregarTransportista());
    }

    private void Btn_atrasTransportista(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void Btn_ModificarTrabajador(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var trabajador = button.CommandParameter as Trabajador;

        if (trabajador != null)
        {
            await Navigation.PushAsync(new VEagregarTransportista(trabajador));
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