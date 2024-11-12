namespace AppTransporte.Interfaces;

public partial class VEclientes : ContentPage
{
	public VEclientes()
	{
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
}