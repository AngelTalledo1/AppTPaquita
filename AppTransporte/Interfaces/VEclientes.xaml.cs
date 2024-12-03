using AppTransporte.viewModel;


namespace AppTransporte.Interfaces;

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

    private void editar_cliente(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEagregarCliente());
    }

    private void eliminar_cliente(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEagregarCliente());

    }
}