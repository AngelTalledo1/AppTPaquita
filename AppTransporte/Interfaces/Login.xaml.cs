using AppTransporte.model;
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
        if (tipoUsuarios.SelectedIndex == -1)
        {
            await DisplayAlert("Tipo de usuario", "Seleccione el tipo de usuario", "OK");
            return;
        }
        string categoria = tipoUsuarios.SelectedItem.ToString();
        string usuario = usuarioEntry.Text;
        string contraseña = contraseñaEntry.Text;
        var url = "http://emmanuel8a-001-site1.ktempurl.com/auth/login";
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };
        var cliente = new HttpClient(handler);
        var bytearray = Encoding.ASCII.GetBytes("11201737:60-dayfreetrial");
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytearray));
        var request = new HttpRequestMessage(HttpMethod.Post, "http://emmanuel8a-001-site1.ktempurl.com/auth/login?username="+usuario+"&password="+contraseña);
        var response = await cliente.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            var usuarioResponse = JsonSerializer.Deserialize<Usuario>(data);
            if (usuarioResponse != null)
            {
                var tipo_usuario = await cliente.SendAsync(new HttpRequestMessage(HttpMethod.Get,"http://emmanuel8a-001-site1.ktempurl.com/tipo_usuario/" + usuarioResponse.idTipoUsuario));
                var data_user =  tipo_usuario.Content.ReadAsStringAsync();
                var tipoUsuario = JsonSerializer.Deserialize<TipoUsuario>(await data_user);
                abrirInterfaz(tipoUsuario.descripcion);
            }
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
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