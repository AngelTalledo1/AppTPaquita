namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;
public partial class Servicios : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    public Servicios(int idUsuario, int idTipoUsuario)
	{
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        InitializeComponent();
	}
    private void Btn_Atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal(_idUsuario, _idTipoUsuario));
    }
    private void Btn_aggserv(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AgregarServicio(_idUsuario, _idTipoUsuario));
    }
    private async void Eliminar_servicio(object sender, EventArgs e)
    {
       var button  = (Button)sender;
        var servicio = button.CommandParameter as Servicio;
        bool respuesta = await DisplayAlert("Confirmación",
                                           "¿Deseas eliminar el servicio de " + servicio.Descripcion + "?",
                                           "Sí",
                                           "No");
        if (respuesta)
        {
            var resultado = await App.Database.EliminarServicioAsync(servicio.IdServicio);
            if (resultado > 0)
            {
                await DisplayAlert("Exito", "Servicio eliminado Exitosamente", "OK");
                if (BindingContext is VMServicio viewModel)
                {
                    await viewModel.ActualizarDatos();
                }
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar el servicio. Verifica los datos.", "OK");
            }
        }
    }
    private async void Editar_servicio(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var servicioSelect = button.CommandParameter as Servicio;

        if (servicioSelect != null)
        {
            await Navigation.PushAsync(new AgregarServicio(servicioSelect, _idUsuario, _idTipoUsuario));
        }
    }

}