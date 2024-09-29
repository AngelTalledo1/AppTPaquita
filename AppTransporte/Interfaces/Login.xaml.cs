namespace AppTransporte.Interfaces;

public partial class Login : ContentPage
{
    Conexion conexion = new Conexion();
    public Login()
	{
		InitializeComponent();
	}

    private async void Ingresar_Clicked(object sender, EventArgs e)
    {
        string usuario = usuarioEntry.Text;
        string contrase�a = contrase�aEntry.Text;

        if (await conexion.VerificarCredenciales(usuario, contrase�a))
        {
            // Redirigir a la siguiente p�gina
            await Navigation.PushAsync(new MenuPrincipal());
        }
        else
        {
            // Mostrar mensaje de error
            MensajeError.Text = "Usuario o contrase�a incorrectos.";
            MensajeError.IsVisible = true;
        }
    }
}