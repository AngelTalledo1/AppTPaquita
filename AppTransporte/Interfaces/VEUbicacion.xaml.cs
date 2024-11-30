namespace AppTransporte.Interfaces;

public partial class VEUbicacion : ContentPage
{
	public VEUbicacion()
	{
		InitializeComponent();
	}

    private void Btn_AtrasUbicacion(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    private void Btn_AggUbicacion(object sender, EventArgs e)
    {

    }

    private void Btn_ModifUbicacion(object sender, EventArgs e)
    {

    }
}