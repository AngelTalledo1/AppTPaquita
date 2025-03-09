using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class MenuPrincipal : ContentPage
{
    private int _idUsuario;
    private int _idtipousuario;
    public MenuPrincipal()
    {
        InitializeComponent();
    }
    public void setUserData(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idtipousuario = idTipoUsuario;
    }

    private async void pedidos_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEpedidos(_idUsuario, _idtipousuario));
    }
    private async void btn_Pedidoauto(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PedidoAuto());
    }

    private async void cliente_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEclientes());
    }


    private async void transportista_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEtransportistas());
    }

    private async void btn_Vehiculos(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEVehiculos());
    }

    private async void btn_Solicitudes(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VESolicitudes());
    }

    private async void Btn_DestinosClicket(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEUbicacion());
    }

    private async void btn_cerrar(object sender, EventArgs e)
    {
        bool respuesta = await DisplayAlert("Cerrar Sesión", "¿Estás seguro de cerrar sesión?", "Aceptar", "Cancelar");
        if (respuesta)
        {
            Cargandoo.IsVisible = true;
            await Task.Delay(2000);
            Cargandoo.IsVisible = false;
            await Navigation.PushAsync(new Login());
        }
        else
        {

        }
    }
    protected override bool OnBackButtonPressed()
    {

        Device.BeginInvokeOnMainThread(async () =>
        {
            bool confirmar = await DisplayAlert("Cerrar sesión", "¿Deseas cerrar sesión?", "Sí", "No");

            if (confirmar)
            {
                Cargandoo.IsVisible = true;
                await Task.Delay(2000);
                Cargandoo.IsVisible = false;
                await Navigation.PushAsync(new Login()); // Redirigir a la pantalla de Login
            }
        });

        return true; // Bloquea la acción predeterminada del botón atrás
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }
}
