namespace AppTransporte.Interfaces;
using AppTransporte.model;

public partial class VEAgregarUbicacion : ContentPage
{
	public VEAgregarUbicacion()
	{
		InitializeComponent();
		TituloAggUbicacion.Text = "Agregar Origen - Destino";
        GuardarUbicacion.IsVisible = true;
        CancelarUbicacion.IsVisible = true;

    }

    public VEAgregarUbicacion(Ubicacion ubicacion)
    {
        InitializeComponent();
        ActualizarUbicacion.IsVisible = true;
        EliminarUbicacion.IsVisible = true;
        descripcionEntry.Text = ubicacion.Descripcion;
        sectorEntry.Text = ubicacion.Sector;
        coordenadasEntry.Text = ubicacion.CoordenadasMaps;
        referenciasEntry.Text = ubicacion.Referencias;
        TituloAggUbicacion.Text = "Modificar Origen - Destino";
    }

    private void GuardarUbicacion_Clicked(object sender, EventArgs e)
    {

    }

    private void Btn_atrasAggUbicacion(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void ActualizarUbicacion_Clicked(object sender, EventArgs e)
    {

    }

    private void EliminarUbicacion_Clicked(object sender, EventArgs e)
    {

    }
}