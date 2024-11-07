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
        bool respuesta = await DisplayAlert("Confirmación", "¿Cerrar sesión?", "Sí", "No");
        if (respuesta)
        {
            // Aquí puedes agregar el código para cerrar sesión
            await DisplayAlert("Sesión cerrada", "Has cerrado sesión correctamente.", "OK");

            // Opcional: Navegar a la página de inicio de sesión o realizar otra acción
            // await Navigation.PushAsync(new LoginPage());
        }
        else
        {
            // Opcional: Mensaje o acción si el usuario decide no cerrar sesión
            await DisplayAlert("Operación cancelada", "No se cerró la sesión.", "OK");
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