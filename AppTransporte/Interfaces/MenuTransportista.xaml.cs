namespace AppTransporte.Interfaces;

public partial class MenuTransportista : ContentPage,IMenuPage
{
    private int _idUsuario;
    private int _idTipoUsuario; 
    public MenuTransportista()
    {
        InitializeComponent();
    }

    public void setUserData(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;
    }

    private async void btn_misViajes(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VTMisViajes());
    }
    private async void btn_Cerrar(object sender, EventArgs e)
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

}
