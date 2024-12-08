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
        Trabajadores.Text = $"{viaje.TrabajadoresAsig}";
        tracto.Text = $"{viaje.TractoAsig}";
        Cisterna.Text = $"{viaje.CisternaAsig}";
        cantidad.Text = $"{viaje.Cantidad}";
        UltEstado.Text = viaje.ultEstado;

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