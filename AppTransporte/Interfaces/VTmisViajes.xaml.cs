namespace AppTransporte.Interfaces;

public partial class VTMisViajes : ContentPage
{
	public VTMisViajes()
	{
		InitializeComponent();
	}

    private void Btn_AtrasMisViajes(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }
}