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

            switch (tipoUsuarios.SelectedIndex)
            {
                case -1:
                    await DisplayAlert("Tipo de usuario", "Seleccione el tipo de usuario", "OK");
                    break;
                case 0:
                    await Navigation.PushAsync(new MenuTransportista());
                    break;
                case 1:
                    await Navigation.PushAsync(new MenuCliente());
                    break;
                case 2:
                    await Navigation.PushAsync(new MenuPrincipal());
                    break;
                default:
                    break;
            }

        }
        else
        {
            // Mostrar mensaje de error
            MensajeError.Text = "Usuario o contrase�a incorrectos.";
            MensajeError.IsVisible = true;
        }
    }

    private async void Olvide_contra(object sender, EventArgs e)
    {
        await DisplayAlert("Informaci�n", "Contacte con el administrador", "OK");
    }
}