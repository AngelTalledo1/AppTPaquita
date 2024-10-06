namespace AppTransporte.Interfaces;

public partial class VEtransportistas : ContentPage
{
	public VEtransportistas()
	{
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
}