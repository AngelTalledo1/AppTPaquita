namespace AppTransporte.Interfaces;

public partial class VEagregarCliente : ContentPage
{
	public VEagregarCliente()
	{
		InitializeComponent();
	}

    private void Btn_atras(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}