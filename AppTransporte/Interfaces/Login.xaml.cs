using AppTransporte.model;
using Azure;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AppTransporte.Interfaces;

public partial class Login : ContentPage
{
  
    public Login()
	{
		InitializeComponent();
	}

    private async void Ingresar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(usuarioEntry.Text) || string.IsNullOrWhiteSpace(contraseñaEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, complete todos los campos.", "OK");
            return;
        }

        string usuario = usuarioEntry.Text;
        string contraseña = contraseñaEntry.Text;

        var resultado = await App.Database.VerificarCredencialesAsync(usuario, contraseña);

        if (resultado.IdUsuario.HasValue)
        {
            abrirInterfaz(await App.Database.obtenerTipoUser(resultado.IdTipoUsuario.GetValueOrDefault()));
        }
        else
        {
            Console.WriteLine($"Error");
            MensajeError.Text = "Usuario o contraseña incorrectos.";
            MensajeError.IsVisible = true;
        }
    }
    private async void abrirInterfaz(string categoria)
    {
        var navigator = new MenuNavigator();
        await navigator.NavigateToMenu(categoria);
}
        private async void Olvide_contra(object sender, EventArgs e)
    {
        await DisplayAlert("Información", "Contacte con el administrador", "OK");
    }
    public class MenuNavigator
    {
        private readonly Dictionary<string, Type> _menuPages = new Dictionary<string, Type>
    {
        { "Transportista", typeof(MenuTransportista) },
        { "Administrador", typeof(MenuPrincipal) },
        { "Cliente", typeof(MenuCliente) },
        // Agrega más menús según sea necesario
    };

        public async Task NavigateToMenu(string menuName)
        {
            if (_menuPages.TryGetValue(menuName, out Type pageType))
            {
                var page = (Page)Activator.CreateInstance(pageType);
                await Application.Current.MainPage.Navigation.PushAsync(page);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Página de menú no encontrada", "OK");
            }
        }
    }
}