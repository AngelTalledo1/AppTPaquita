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
    //private void OnNumeroEntryTextChanged(object sender, TextChangedEventArgs e)
    //{
    //    if (!string.IsNullOrWhiteSpace(e.NewTextValue))
    //    {
    //        bool isValid = Regex.IsMatch(e.NewTextValue, @"^\d+$");
    //        if (!isValid)
    //        {
    //            // Si el texto no es v�lido (contiene algo que no sea un n�mero),
    //            // revertimos al texto anterior
    //            ((Entry)sender).Text = e.OldTextValue;
    //        }
    //    }
    //}
}