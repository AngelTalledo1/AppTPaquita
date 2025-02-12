using AppTransporte.viewModel;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VCMisSolicitudes : ContentPage
{
    private readonly VMmisSolicitudes _viewModel;
    private Cliente Cliente { get; set; }
    private int idUsuario;
    public VCMisSolicitudes(int idUsuario)
    {
        InitializeComponent();
        this.idUsuario = idUsuario;
        InitializeAsync();

    }

    private void InitializeAsync()
    {
        InitializeAsync(_viewModel);
    }

    private async void InitializeAsync(VMmisSolicitudes _viewModel)
    {
        await asignarCliente(idUsuario);
        _viewModel = new VMmisSolicitudes(Cliente.IdCliente);
        BindingContext = _viewModel;
    }

    private void Btn_atrasMisSolic(object sender, EventArgs e)
    {
		 Navigation.PopAsync();
    }

    private async void btn_NewSolicitud(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VCNuevaSolicitud(Cliente,idUsuario));
    }

    private async Task asignarCliente(int idUsuario)
    {
        Cliente = await App.Database.ObtenerClientePorUsuarioAsync(idUsuario);

        // Validación adicional por si no se encuentra el cliente
        if (Cliente == null)
        {
            await DisplayAlert("Error", "Cliente no encontrado", "OK");
            await Navigation.PopAsync();
        }
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