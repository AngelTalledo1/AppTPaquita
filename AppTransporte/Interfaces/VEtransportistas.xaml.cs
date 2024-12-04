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

    private void EliminarTrabajador(object sender, EventArgs e)
    {

    }
}