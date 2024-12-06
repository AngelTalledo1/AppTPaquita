namespace AppTransporte.Interfaces;
using AppTransporte.model;
using AppTransporte.viewModel;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;

public partial class VESeguimientoViaje : ContentPage
{
	public VESeguimientoViaje(Viaje viaje)
	{
		InitializeComponent();
         
        this.BindingContext = new VMSeguimientoViaje(viaje.IdViaje);
        EstadoDetalle.Text = viaje.ultEstado;
        Cliente.Text = $"Trabajadores asignados: {viaje.TrabajadoresAsig}";
        tracto.Text = $"Placa Tracto asignado: {viaje.TractoAsig}";
        Cisterna.Text = $"Placa Tracto asignado: {viaje.CisternaAsig}";
        cantidad.Text = $"Cantidad de viaje: {viaje.Cantidad}";

    }

    public VESeguimientoViaje()
    {
        InitializeComponent();
        
    }

    private void Btn_atrasSeguimientoViaje(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    

}