namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;
public partial class AgregarEmpresa : ContentPage
{
    public int id_servicio { get; set; }
    private int _idUsuario;
    private int _idTipoUsuario;
    public AgregarEmpresa(int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        GuardarEmpresa.IsVisible = true;
        TituloLabel.Text = "Informacion General";
    }
    public AgregarEmpresa(Empresa empresaSelect, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        id_servicio = empresaSelect.IdEmpresa;
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        ActualizarEmpresa.IsVisible = true;
        tituloInterfaz.Text = "Modificar Servicio";
        TituloLabel.Text = "Información General";
        empresaEntry.Text = empresaSelect.NombreEmpresa;
        razonEntry.Text = empresaSelect.Razon;
        rucEntry.Text = empresaSelect.Ruc;
        direccionEntry.Text = empresaSelect.Direccion;
        BindingContext = empresaSelect;

    }
    private void Btn_Atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Empresas(_idUsuario, _idTipoUsuario));
    }
    private void btn_actualizar(object sender, EventArgs e)
    {
        
    }
    private void btn_guardar(object sender, EventArgs e)
    {
        
    }

}