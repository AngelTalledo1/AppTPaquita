
namespace AppTransporte.Interfaces;

public partial class MenuCliente : ContentPage, IMenuPage
{
    public int _idUsuario;
    public int _idTipoUsuario;
    public MenuCliente(int idUsuario, int idTipoUsuario)
	{
        InitializeComponent();
        this._idUsuario = idUsuario;
        this._idTipoUsuario = idTipoUsuario;

    }

    public void setUserData(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;
        
    }
    private void NuevoPedido_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisSolicitudes(_idUsuario, _idTipoUsuario));
    }
    private void pedidosCliente_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisPedidos(_idUsuario, _idTipoUsuario));
    }
    private async void btn_Cerrar(object sender, EventArgs e)
    {
        bool respuesta = await DisplayAlert("Cerrar Sesi�n", "�Est�s seguro de cerrar sesi�n?", "Aceptar", "Cancelar");
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
            bool confirmar = await DisplayAlert("Cerrar sesi�n", "�Deseas cerrar sesi�n?", "S�", "No");

            if (confirmar)
            {
                Cargandoo.IsVisible = true;
                await Task.Delay(2000);
                Cargandoo.IsVisible = false;
                await Navigation.PushAsync(new Login()); // Redirigir a la pantalla de Login
            }
        });

        return true; // Bloquea la acci�n predeterminada del bot�n atr�s
    }

}


