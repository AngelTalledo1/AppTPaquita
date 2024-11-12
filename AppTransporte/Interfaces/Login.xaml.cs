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

        // Obtener el valor seleccionado del Picker como categoría
        string categoria = tipoUsuarios.SelectedItem.ToString();
        string usuario = usuarioEntry.Text;
        string contraseña = contraseñaEntry.Text;

        // Llamar al método VerificarCredenciales con los tres parámetros: categoria, usuario, y contraseña
        if (await conexion.VerificarCredenciales(categoria, usuario, contraseña))
        {
            // Navegar según el tipo de usuario
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
            MensajeError.Text = "Usuario o contraseña incorrectos.";
            MensajeError.IsVisible = true;
        }
    }

    private async void Olvide_contra(object sender, EventArgs e)
    {
        await DisplayAlert("Información", "Contacte con el administrador", "OK");
    }
}