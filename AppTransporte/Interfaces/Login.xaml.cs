
using CommunityToolkit.Maui.Views;

namespace AppTransporte.Interfaces;

public partial class Login : ContentPage
{
    public bool IsLoading { get; set; } = false;

    public Login()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void Ingresar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(usuarioEntry.Text) || string.IsNullOrWhiteSpace(contrase�aEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, complete todos los campos.", "OK");
            return;
        }
        usuarioEntry.IsEnabled = false;
        contrase�aEntry.IsEnabled = false;
        MensajeError.IsVisible = false;

        string usuario = usuarioEntry.Text;
        string contrase�a = contrase�aEntry.Text;
        Cargando.IsVisible = true;
        await Task.Delay(2000);
        var resultado = await App.Database.VerificarCredencialesAsync(usuario, contrase�a);
        if (resultado != null)
        {
            abrirInterfaz(await App.Database.obtenerTipoUser(resultado.idTipoUsuario), resultado.idUsuario, resultado.idTipoUsuario);
        }
        else
        {
            Console.WriteLine($"Error");
            MensajeError.Text = "Usuario o contrase�a incorrectos.";
            MensajeError.IsVisible = true;
        }
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Cargando.IsVisible = false;
            usuarioEntry.IsEnabled = true;
            contrase�aEntry.IsEnabled = true;
        });
    }
    private async void abrirInterfaz(string categoria, int idUsuario, int idTipoUsuario)
    {
        Console.WriteLine(categoria);
        var navigator = new MenuNavigator();
        await navigator.NavigateToMenu(categoria, idUsuario, idTipoUsuario);
    }
    private async void Olvide_contra(object sender, EventArgs e)
    {
        await DisplayAlert("Informaci�n", "Contacte con el administrador", "OK");
    }
    public class MenuNavigator
    {
        private readonly Dictionary<string, Type> _menuPages = new Dictionary<string, Type>
    {
        { "Transportista", typeof(MenuTransportista) },
        { "Administrador", typeof(MenuPrincipal) },
        { "Cliente", typeof(MenuCliente) },
        // Agrega m�s men�s seg�n sea necesario
    };
        public async Task NavigateToMenu(string menuName, int idUsuario, int idTipoUsuario)
        {
            if (_menuPages.TryGetValue(menuName, out Type pageType))
            {
                // Crear instancia de la p�gina
                var page = (Page)Activator.CreateInstance(pageType, idUsuario, idTipoUsuario);

                // Asignar par�metros si la p�gina implementa una interfaz espec�fica
                if (page is IMenuPage menuPage)
                {
                    menuPage.setUserData(idUsuario, idTipoUsuario);
                }

                await Application.Current.MainPage.Navigation.PushAsync(page);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "P�gina de men� no encontrada", "OK");
            }
        }
    }
}