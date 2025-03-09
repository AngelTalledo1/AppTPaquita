namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;

public partial class VEAgregarUbicacion : ContentPage
{
    public Ubicacion ubicacion { get; set; }
    private int _idUsuario;
    private int _idTipoUsuario;
    public VEAgregarUbicacion(int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        TituloAggUbicacion.Text = "Agregar Origen - Destino";
        GuardarUbicacion.IsVisible = true;
        CancelarUbicacion.IsVisible = true;

    }

    public VEAgregarUbicacion(Ubicacion ubicacion, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this.ubicacion = ubicacion;
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        ActualizarUbicacion.IsVisible = true;
        EliminarUbicacion.IsVisible = true;
        descripcionEntry.Text = ubicacion.Descripcion;
        sectorEntry.Text = ubicacion.Sector;
        coordenadasEntry.Text = ubicacion.CoordenadasMaps;
        referenciasEntry.Text = ubicacion.Referencias;
        TituloAggUbicacion.Text = "Modificar Origen - Destino";
    }

    private async void GuardarUbicacion_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(descripcionEntry.Text) &&
            !string.IsNullOrWhiteSpace(sectorEntry.Text) &&
            !string.IsNullOrWhiteSpace(coordenadasEntry.Text) &&
            !string.IsNullOrWhiteSpace(referenciasEntry.Text))
        {
            int resultado = await App.Database.AgregarUbicacionAsync(
                 descripcionEntry.Text,
                 sectorEntry.Text,
                 referenciasEntry.Text,
                 coordenadasEntry.Text
                 );
            if (resultado > 0)
            {

                await DisplayAlert("Exito", "Ubicacion agregada correctamente.", "OK");
                await Navigation.PushAsync(new VEUbicacion(_idUsuario, _idTipoUsuario));
            }
            else
                await DisplayAlert("Error", "No se pudo agregar la ubicacion.", "OK");
        }
        else
            await DisplayAlert("Error", "Todos los campos obligatorios deben llenarse.", "OK");

    }

    private void Btn_atrasAggUbicacion(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEUbicacion(_idUsuario, _idTipoUsuario));
    }

    private async void ActualizarUbicacion_Clicked(object sender, EventArgs e)
    {
        int resultado = await App.Database.ModificarUbicacionAsync(
            ubicacion.IdUbicacion,
            descripcionEntry.Text,
            sectorEntry.Text,
            referenciasEntry.Text,
            coordenadasEntry.Text
            );
        if (resultado > 0)
        {
            await DisplayAlert("Exito", "Ubicacion actualizada correctamente.", "OK");
            await Navigation.PushAsync(new VEUbicacion(_idUsuario, _idTipoUsuario));
        }
        else
        {
            await DisplayAlert("Error", "No se pudo actualizar la ubicacion.", "OK");
        }
    }

    private async void EliminarUbicacion_Clicked(object sender, EventArgs e)
    {
        bool respuesta = await DisplayAlert("Confirmaci�n",
                                     "�Deseas eliminar a " + ubicacion.IdUbicacion + "?",
                                     "S�",
                                     "No");
        if (respuesta)
        {
            var resultado = await App.Database.eliminarUbicacionAsync(ubicacion.IdUbicacion);

            if (resultado > 0)
            {
                await DisplayAlert("Exito", "Ubicaci�n eliminada Exitosamente", "OK");
                await Navigation.PushAsync(new VEUbicacion(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar el trabajador. Verifica los datos.", "OK");
            }
        }
    }
}