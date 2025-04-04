using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VESolicitudes : ContentPage
{
    private int idUsuario;
    private int idtipousuario;
    public VESolicitudes(int idUsuario, int idTipoUsuario)
	{
        this.idUsuario = idUsuario;
        this.idtipousuario = idTipoUsuario;
        BindingContext = new VMmisSolicitudes();
        InitializeComponent();
    }

    private void Btn_atrasSolicitudes(object sender, EventArgs e)
    {
		Navigation.PushAsync(new MenuPrincipal(idUsuario, idtipousuario));
    }


    private async void SolicitudSelected_detalle(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud; 

        if (solicitud != null)
        {
            var pedidosViewModel = this.BindingContext as VMPedidos;
            await Navigation.PushAsync(new VEDetalleSolicitud(solicitud, pedidosViewModel, idUsuario, idtipousuario));
        }
    }

    
}