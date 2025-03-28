using AppTransporte.model;
using AppTransporte.viewModel;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
    private void Editar_empresa(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var empresaSelect = button.CommandParameter as Empresa;
        Navigation.PushAsync(new AgregarEmpresa(empresaSelect,_idUsuario, _idTipoUsuario));
    }

    private async void Eliminar_empresa(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var empresa = button.CommandParameter as Empresa;
        bool respuesta = await DisplayAlert("Confirmación",
                                           "¿Deseas eliminar la empresa " + empresa.razonSocial + "?",
                                           "Sí",
                                           "No");
        if (respuesta)
        {
            var resultado = await App.Database.EliminarEmpresaAsync(empresa.id_empresa);
            if (resultado > 0)
            {
                await DisplayAlert("Exito", "Empresa eliminada Exitosamente", "OK");
                if (BindingContext is VMEmpresas viewModel)
                {
                    await viewModel.ActualizarDatos();
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar la empresa. Verifica los datos.", "OK");
            }
        }
    }
}