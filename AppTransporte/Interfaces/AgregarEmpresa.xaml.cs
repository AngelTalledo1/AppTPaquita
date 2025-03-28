namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;
using System.Threading.Tasks;

public partial class AgregarEmpresa : ContentPage
{
    public int id_empresa { get; set; }
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
        id_empresa = empresaSelect.id_empresa;
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        ActualizarEmpresa.IsVisible = true;
        tituloInterfaz.Text = "Modificar Servicio";
        TituloLabel.Text = "Información General";
        razonEntry.Text = empresaSelect.razonSocial;
        rucEntry.Text = empresaSelect.RUC;
        direccionEntry.Text = empresaSelect.Direccion;
        BindingContext = empresaSelect;

    }
    private void Btn_Atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Empresas(_idUsuario, _idTipoUsuario));
    }
    private async void btn_actualizar(object sender, EventArgs e)
    {
        try
        {
            int resultado = await App.Database.ActualizarEmpresaAsync(id_empresa,razonEntry.Text,rucEntry.Text,direccionEntry.Text);
            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Empresa modificada correctamente.", "OK");
                await Navigation.PushAsync(new Empresas(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo modificar la empresa. Verifica los datos.", "OK");
                await Navigation.PushAsync(new Empresas(_idUsuario, _idTipoUsuario));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "OK");
        }
    }
    private async void btn_guardar(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(razonEntry.Text) && !string.IsNullOrWhiteSpace(rucEntry.Text))
        {
            int resultado = await App.Database.AgregarEmpresaAsync(razonEntry.Text, rucEntry.Text, direccionEntry.Text);
            if (resultado > 0)
            {
                await DisplayAlert("Exito", "Empresa guardada Exitosamente", "OK");
                await Navigation.PushAsync(new Empresas(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo guardar la empresa. Verifica los datos.", "OK");
            }
        }
        else
        {
                await DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
        
        }
    }

}