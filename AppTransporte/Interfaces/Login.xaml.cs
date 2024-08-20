namespace AppTransporte.Interfaces;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

    private void Ingresar_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal());
    }
}