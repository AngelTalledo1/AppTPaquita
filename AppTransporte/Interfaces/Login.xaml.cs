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
        string contraseña = contraseñaEntry.Text;

        if (await conexion.VerificarCredenciales(usuario, contraseña))
        {
            // Redirigir a la siguiente página
            await Navigation.PushAsync(new MenuPrincipal());
        }
        else
        {
            // Mostrar mensaje de error
            MensajeError.Text = "Usuario o contraseña incorrectos.";
            MensajeError.IsVisible = true;
        }
    }
}