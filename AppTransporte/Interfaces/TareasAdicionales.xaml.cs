namespace AppTransporte.Interfaces;

public partial class TareasAdicionales : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    public TareasAdicionales(int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();
	}
    private void Btn_Atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuTransportista(_idUsuario, _idTipoUsuario));
    }
}