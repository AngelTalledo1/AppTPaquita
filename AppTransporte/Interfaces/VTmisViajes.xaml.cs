namespace AppTransporte.Interfaces;

public partial class VTmisViajes : ContentPage
{
	public VTmisViajes()
	{
		InitializeComponent();
	}

    private void Btn_atrasMisViajes(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

}