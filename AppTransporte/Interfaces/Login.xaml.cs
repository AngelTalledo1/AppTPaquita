using AppTransporte.Servicios;
using CommunityToolkit.Maui.Views;

namespace AppTransporte.Interfaces;

public partial class Login : ContentPage
{
    private readonly AuthService _authService;
    public bool IsLoading { get; set; } = false;

    public Login()
	{
		InitializeComponent();
        _authService = new AuthService();
        BindingContext = this;
    }

    private async void Ingresar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(usuarioEntry.Text) || string.IsNullOrWhiteSpace(contraseñaEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, complete todos los campos.", "OK");
            return;
        }

        //var popup = new ValidatingPopup();
        //await this.ShowPopupAsync(popup);
        //loadingIndicator.IsRunning = true;
        //loadingIndicator.IsVisible = true;

        //ValidandoDatosLabel.IsVisible = true;
        usuarioEntry.IsEnabled = false;
        contraseñaEntry.IsEnabled = false;
        MensajeError.IsVisible = false;
        
        string usuario = usuarioEntry.Text;
        string contraseña = contraseñaEntry.Text;
        
        var resultado = await _authService.VerificarCredencialesAsync(usuario, contraseña);
        if (resultado != null)
        {
            
            abrirInterfaz(await _authService.ObtenerTipoUsuarioAsync(resultado.idTipoUsuario), resultado.idUsuario, resultado.idTipoUsuario);
            //popup.Close();
        }
        else
        {
            Console.WriteLine($"Error");
            MensajeError.Text = "Usuario o contraseña incorrectos.";
            MensajeError.IsVisible = true;
        }

        MainThread.BeginInvokeOnMainThread(() =>
        {
            //boxValidar.IsVisible = false;
            
            //ValidandoDatosLabel.IsVisible = false;
            //loadingIndicator.IsRunning = false;
            //loadingIndicator.IsVisible = false;
            usuarioEntry.IsEnabled = true;
            contraseñaEntry.IsEnabled = true;

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

        public async Task NavigateToMenu(string menuName, int idUsuario, int idTipoUsuario)
        {
            if (_menuPages.TryGetValue(menuName, out Type pageType))
            {
                // Crear instancia de la página
                var page = (Page)Activator.CreateInstance(pageType);

                // Asignar parámetros si la página implementa una interfaz específica
                if (page is IMenuPage menuPage)
                {
                    menuPage.setUserData(idUsuario, idTipoUsuario);
                }

                await Application.Current.MainPage.Navigation.PushAsync(page);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Página de menú no encontrada", "OK");
            }
        }
    }
}