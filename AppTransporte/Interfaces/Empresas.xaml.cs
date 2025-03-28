namespace AppTransporte.Interfaces;

public partial class Empresas : ContentPage
{
    public int id_servicio { get; set; }
    private int _idUsuario;
    private int _idTipoUsuario;
    public Empresas(int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();
	}
    private void Btn_atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal(_idUsuario, _idTipoUsuario));
    }
    private void Btn_aggempre(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AgregarEmpresa(_idUsuario, _idTipoUsuario));
    }
    
}