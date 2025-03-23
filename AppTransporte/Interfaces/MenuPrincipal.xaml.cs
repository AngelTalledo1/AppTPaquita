using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class MenuPrincipal : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    public MenuPrincipal(int idUsuario, int idTipoUsuario)
    {
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        InitializeComponent();
    }
    public void setUserData(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;
    }

    private async void pedidos_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEpedidos(_idUsuario, _idTipoUsuario));
    }
    private async void btn_Pedidoauto(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PedidoAuto(_idUsuario, _idTipoUsuario));
    }

    private async void cliente_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEclientes(_idUsuario, _idTipoUsuario));
    }


    private async void transportista_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEtransportistas(_idUsuario, _idTipoUsuario));
    }

    private async void btn_Vehiculos(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEVehiculos(_idUsuario, _idTipoUsuario));
    }

    private async void btn_Solicitudes(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VESolicitudes(_idUsuario, _idTipoUsuario));
    }

    private async void Btn_DestinosClicket(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEUbicacion(_idUsuario, _idTipoUsuario));
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
