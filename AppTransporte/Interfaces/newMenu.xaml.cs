namespace AppTransporte.Interfaces;

public partial class newMenu : ContentPage
{
    private int _idUsuario;
    private int _idtipousuario;
    public newMenu()
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

    //private async void btn_cerrar(object sender, EventArgs e)
    //{
    //    bool respuesta = await DisplayAlert("Cerrar Sesi�n", "�Est�s seguro de cerrar sesi�n?", "Aceptar", "Cancelar");
    //    if (respuesta)
    //    {
    //        Cargandoo.IsVisible = true;
    //        await Task.Delay(2000);
    //        Cargandoo.IsVisible = false;
    //        await Navigation.PushAsync(new Login());
    //    }
    //    else
    //    {

    //    }


    //}
}