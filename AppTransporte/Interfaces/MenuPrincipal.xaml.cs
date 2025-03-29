using AppTransporte.viewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using AppTransporte.Interfaces;
using AppTransporte.model;
using System.Collections.ObjectModel;



namespace AppTransporte.Interfaces;

public partial class MenuPrincipal : ContentPage
{
    private List<Frame> opcionesFrames;
    private int _idUsuario;
    private int _idTipoUsuario;

    public MenuPrincipal(int idUsuario, int idTipoUsuario)
    {
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        InitializeComponent();
        CargarFrames();
    }
        private void CargarFrames()
    {
        opcionesFrames = new List<Frame>
            {
                clientesFrame, frameEmpresa, framePedidos, framePedidosAuto,
                frameServicios, frameSolicitudes, frameTrabajadores,
                frameUbicacion, frameUsuarios, frameCalendario, frameVehiculos
            };
    }
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string textoBusqueda = e.NewTextValue.ToLower();

        foreach (var frame in opcionesFrames)
        {
            bool esVisible = frame.Content is Grid grid &&
                             grid.Children.OfType<VerticalStackLayout>()
                                 .FirstOrDefault()?.Children.OfType<Label>()
                                 .FirstOrDefault()?.Text.ToLower().Contains(textoBusqueda) == true;

            frame.IsVisible = esVisible;
        }
    }


    public void setUserData(int idUsuario, int idTipoUsuario)
    {
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;
    }

    private async void pedidos_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEpedidos(_idUsuario, _idTipoUsuario));
    }
    private async void btn_Pedidoauto(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PedidoAuto(_idUsuario, _idTipoUsuario));
    }
    private async void Btn_AgregarServ(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Servicios(_idUsuario, _idTipoUsuario));
    }
    

    private async void cliente_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEclientes(_idUsuario, _idTipoUsuario));
    }
    private async void calendario_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Calendar(_idUsuario, _idTipoUsuario));
    }

    private async void transportista_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEtransportistas(_idUsuario, _idTipoUsuario));
    }

    private async void btn_Vehiculos(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEVehiculos(_idUsuario, _idTipoUsuario));
    }

    private async void btn_Solicitudes(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VESolicitudes(_idUsuario, _idTipoUsuario));
    }
    private async void btn_Empresa(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Empresas(_idUsuario, _idTipoUsuario));
    }

    private async void Btn_DestinosClicket(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEUbicacion(_idUsuario, _idTipoUsuario));
    }

    private async void btn_cerrar(object sender, EventArgs e)
    {
        bool respuesta = await DisplayAlert("Cerrar Sesión", "¿Estás seguro de cerrar sesión?", "Aceptar", "Cancelar");
        if (respuesta)
        {
            Cargandoo.IsVisible = true;
            await Task.Delay(2000);
            Cargandoo.IsVisible = false;
            await Navigation.PushAsync(new Login());
        }
        else
        {

        }
    }
    protected override bool OnBackButtonPressed()
    {

        Device.BeginInvokeOnMainThread(async () =>
        {
            bool confirmar = await DisplayAlert("Cerrar sesión", "¿Deseas cerrar sesión?", "Sí", "No");

            if (confirmar)
            {
                Cargandoo.IsVisible = true;
                await Task.Delay(2000);
                Cargandoo.IsVisible = false;
                await Navigation.PushAsync(new Login()); // Redirigir a la pantalla de Login
            }
        });

        return true; // Bloquea la acción predeterminada del botón atrás
    }

   
    private async void OnUserManagementClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VEUsuarios(_idUsuario, _idTipoUsuario));
    }

    
}
