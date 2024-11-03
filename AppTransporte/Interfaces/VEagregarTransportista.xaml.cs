namespace AppTransporte.Interfaces;

public partial class VEagregarTransportista : ContentPage
{
	public VEagregarTransportista()
	{
		InitializeComponent();
	}
    private void Btn_atrasTrab(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}