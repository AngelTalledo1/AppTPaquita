using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEAsignarViaje : ContentPage
{
	public VEAsignarViaje(Viaje viaje)
	{
		InitializeComponent();
		BindingContext = viaje;
		Id_Pedido.Text = $"ID Pedido: { viaje.IdPedido.ToString()}";
        Id_viaje.Text = $"ID Viaje: {viaje.IdViaje.ToString()}";
    }

    private void Btn_atrasAsignarViaje(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }
}