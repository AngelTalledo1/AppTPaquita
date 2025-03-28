namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;
public partial class AgregarServicio : ContentPage
{
    public int id_servicio { get; set; }
    private int _idUsuario;
    private int _idTipoUsuario;
    public AgregarServicio(int idUsuario, int idTipoUsuario)
	{
       
        InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        GuardarServicio.IsVisible = true;
        TituloLabel.Text = "Información General";
    }
    public AgregarServicio(Servicio servicioSelect, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        id_servicio = servicioSelect.IdServicio;
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        ActualizarServicio.IsVisible = true;
        tituloInterfaz.Text = "Modificar Servicio";
        TituloLabel.Text = "Información General";
        descripcion_entry.Text = servicioSelect.Descripcion;
        BindingContext = servicioSelect;
    }


    private void Btn_atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Servicios(_idUsuario, _idTipoUsuario));
    }
    private async void btn_actualizar(object sender, EventArgs e)
    {
        try
        {
            int resultado = await App.Database.ActualizarServicioAsync(id_servicio, descripcion_entry.Text);
            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Servicio modificado correctamente.", "OK");
                await Navigation.PushAsync(new Servicios(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo modificar el servicio. Verifica los datos.", "OK");
                await Navigation.PushAsync(new Servicios(_idUsuario, _idTipoUsuario));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "OK");
        }
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