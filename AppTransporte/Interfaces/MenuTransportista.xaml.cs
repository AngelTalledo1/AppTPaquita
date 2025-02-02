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

}
