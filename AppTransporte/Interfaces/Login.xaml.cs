using System.Text.RegularExpressions;

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

    private async void Olvide_contra(object sender, EventArgs e)
    {
        await DisplayAlert("Información", "Contacte con el administrador", "OK");
    }
    private void OnNumeroEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            bool isValid = Regex.IsMatch(e.NewTextValue, @"^\d+$");
            if (!isValid)
            {
                // Si el texto no es válido (contiene algo que no sea un número),
                // revertimos al texto anterior
                ((Entry)sender).Text = e.OldTextValue;
            }
        }
    }
}