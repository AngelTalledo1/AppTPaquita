using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class MenuPrincipal : ContentPage
{
	public MenuPrincipal()
	{
		InitializeComponent();
	}
    private void pedidos_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEpedidos());
    }

    private void cliente_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEclientes());
    }

    private async void Btn_cerrarSesion(object sender, EventArgs e)
    {
        bool respuesta = await DisplayAlert("Confirmaci�n", "�Cerrar sesi�n?", "S�", "No");
        if (respuesta)
        {
            // Aqu� puedes agregar el c�digo para cerrar sesi�n
            await DisplayAlert("Sesi�n cerrada", "Has cerrado sesi�n correctamente.", "OK");

            // Opcional: Navegar a la p�gina de inicio de sesi�n o realizar otra acci�n
            // await Navigation.PushAsync(new LoginPage());
        }
        else
        {
            // Opcional: Mensaje o acci�n si el usuario decide no cerrar sesi�n
            await DisplayAlert("Operaci�n cancelada", "No se cerr� la sesi�n.", "OK");
        }
    }

    private void transportista_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEtransportistas());
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}