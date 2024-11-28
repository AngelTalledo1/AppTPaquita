using AppTransporte.viewModel;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

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

    

}