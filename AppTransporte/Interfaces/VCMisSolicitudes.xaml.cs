using AppTransporte.viewModel;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VCMisSolicitudes : ContentPage
{
	public VCMisSolicitudes()
	{
		InitializeComponent();
        
    }

    private  void Btn_atrasMisSolic(object sender, EventArgs e)
    {
		 Navigation.PopAsync();
    }

    private async void btn_NewSolicitud(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VCNuevaSolicitud());
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