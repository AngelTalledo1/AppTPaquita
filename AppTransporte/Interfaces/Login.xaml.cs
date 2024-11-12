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
        // Validar que se ha seleccionado un tipo de usuario
        if (tipoUsuarios.SelectedIndex == -1)
        {
            await DisplayAlert("Tipo de usuario", "Seleccione el tipo de usuario", "OK");
            return;
        }

        // Obtener el valor seleccionado del Picker como categor�a
        string categoria = tipoUsuarios.SelectedItem.ToString();
        string usuario = usuarioEntry.Text;
        string contrase�a = contrase�aEntry.Text;

        // Llamar al m�todo VerificarCredenciales con los tres par�metros: categoria, usuario, y contrase�a
        if (await conexion.VerificarCredenciales(categoria, usuario, contrase�a))
        {
            // Navegar seg�n el tipo de usuario
            switch (tipoUsuarios.SelectedIndex)
            {
                case 0: // Transportista
                    await Navigation.PushAsync(new MenuTransportista());
                    break;
                case 1: // Cliente
                    await Navigation.PushAsync(new MenuCliente());
                    break;
                case 2: // Admin u otro tipo de usuario
                    await Navigation.PushAsync(new MenuPrincipal());
                    break;
                default:
                    break;
            }
        }
        else
        {
            // Mostrar mensaje de error si las credenciales son incorrectas
            MensajeError.Text = "Usuario o contrase�a incorrectos.";
            MensajeError.IsVisible = true;
        }
    }

    private async void Olvide_contra(object sender, EventArgs e)
    {
        await DisplayAlert("Informaci�n", "Contacte con el administrador", "OK");
    }
}