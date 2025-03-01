using AppTransporte.model;
using AppTransporte.viewModel;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;

namespace AppTransporte.Interfaces;

public partial class VTMisViajes : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    private Pedido _pedido;

    public VTMisViajes(int idUsuario, int idTipoUsuario)
    {
        var viewModel = new VMViajes(idUsuario : idUsuario);
    
        // Asignar el VM como BindingContext
        this.BindingContext = viewModel;
        InitializeComponent();
        this._idUsuario = idUsuario;
        this._idTipoUsuario = idTipoUsuario;
        
    }
    private void Btn_AtrasMisViajes(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuTransportista(_idUsuario, _idTipoUsuario));
    }

    private async void Btn_SeguimientoViajes(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var viaje = button.CommandParameter as Viaje;
        if (viaje != null)
        {
            // Validar si alg�n campo est� sin asignar
            if (string.IsNullOrWhiteSpace(viaje.TractoAsig) || viaje.TractoAsig == "S/A" ||
                string.IsNullOrWhiteSpace(viaje.CisternaAsig) || viaje.CisternaAsig == "S/A" ||
                viaje.Cantidad <= 0 ||
                string.IsNullOrWhiteSpace(viaje.TrabajadoresAsig) || viaje.TrabajadoresAsig == "S/A")
            {
                // Navegar a la interfaz para asignar el viaje
                await Navigation.PushAsync(new VEAsignarViaje(viaje, _idUsuario, _idTipoUsuario));
            }
            else
            {
                // Navegar a la interfaz de seguimiento del viaje
                await Navigation.PushAsync(new VESeguimientoViaje(viaje,_pedido, _idUsuario, _idTipoUsuario));
            }
        }

    }
}