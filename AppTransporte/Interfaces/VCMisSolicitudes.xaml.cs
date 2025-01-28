using AppTransporte.viewModel;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VCMisSolicitudes : ContentPage
{
    private Cliente Cliente { get; set; }
    private int idUsuario;
    public VCMisSolicitudes(int idUsuario)
    {
        InitializeComponent();
        this.idUsuario = idUsuario;
        asignarCliente(idUsuario);

    }

    private  void Btn_atrasMisSolic(object sender, EventArgs e)
    {
		 Navigation.PopAsync();
    }

    private async void btn_NewSolicitud(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VCNuevaSolicitud(Cliente,idUsuario));
    }

    private async void asignarCliente(int idUsuario)
    {
        Cliente = await App.Database.ObtenerClientePorUsuarioAsync(idUsuario);
    }

    private async void btnModificar_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud; 

        if (solicitud != null)
        {
            await Navigation.PushAsync(new VCNuevaSolicitud(solicitud));
        }
    }
}