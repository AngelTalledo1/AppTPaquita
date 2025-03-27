namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;
public partial class AgregarServicio : ContentPage
{

    private int _idUsuario;
    private int _idTipoUsuario;
    public AgregarServicio(int idUsuario, int idTipoUsuario)
	{
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        InitializeComponent();
	}
   
    private void Btn_atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Servicios(_idUsuario, _idTipoUsuario));
    }
    private void btn_cancelar(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Servicios(_idUsuario, _idTipoUsuario));
    }

    private async void btn_guardar(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(descripcion_entry.Text))
        {
            int resultado = await App.Database.InsertarServicioAsync(descripcion_entry.Text);
            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Servicio agregado correctamente.", "OK");
                await Navigation.PushAsync(new Servicios(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar el servicio.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "El campo de descripción es obligatorio.", "OK");
        }
    }
}