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
        BindingContext = ubicacion;
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