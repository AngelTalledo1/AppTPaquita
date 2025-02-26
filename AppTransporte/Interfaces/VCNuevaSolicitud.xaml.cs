using AppTransporte.model;
using Microsoft.Maui.Graphics.Text;

namespace AppTransporte.Interfaces;

public partial class VCNuevaSolicitud : ContentPage
{
    public Cliente cliente { get; set; }

    public int idUsuario { get; set; }
    public int idTipoUsuario { get; set; }
    public VCNuevaSolicitud(Cliente cliente,int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this.cliente = cliente;
        this.idUsuario = idUsuario;
        this.idTipoUsuario = idTipoUsuario;
        TituloNuev.Text = "Nueva Solicitud";
        IdClienteLabel.Text = cliente.ApePaterno + " " + cliente.ApeMaterno + ", " + cliente.Nombre;
        Solicitar.IsVisible = true;
        Cancelar.IsVisible = true;
        this.idTipoUsuario = idTipoUsuario;
    }

    public VCNuevaSolicitud(Solicitud solicitud, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this.idUsuario= idUsuario;
        this.idTipoUsuario= idTipoUsuario;
        TituloNuev.Text = $"Modificar Solicitud {solicitud.IdSolicitud}";
        IdClienteLabel.Text = $"{solicitud.IdCliente}";
        descripcionEntry.Text = solicitud.Descripcion;
        Eliminar.IsVisible = true;
        Actualizar.IsVisible = true;
        BindingContext = solicitud;
    }


    private void Btn_atrasSolPedido(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisSolicitudes(idUsuario, idTipoUsuario));
    }
    private void Btn_cancelar(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VCMisSolicitudes(idUsuario, idTipoUsuario));

    }


    private async void Btn_Solicitar(object sender, EventArgs e)
    {
        
        try
        {
            if (string.IsNullOrWhiteSpace(descripcionEntry.Text))
            {
                await DisplayAlert("Error", "La descripción es obligatoria.", "OK");
                return;
            }

            var nuevaSolicitud = new Solicitud
            {
                Descripcion = descripcionEntry.Text,
                IdEstadoSolicitud = 1,
                IdCliente = cliente.IdCliente, 
                FechaSolicitud = DateTime.Now
            };

            await App.Database.AgregarSolicitudAsync(nuevaSolicitud);

            await DisplayAlert("Éxito", "Solicitud creada exitosamente.", "OK");
            Navigation.PushAsync(new VCMisSolicitudes(idUsuario, idTipoUsuario));

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al crear la solicitud: {ex.Message}", "OK");
        }
      
    }



    private void Btn_EliminarSolicitud(object sender, EventArgs e)
    {

    }

    private void Btn_ModificarSolicitud(object sender, EventArgs e)
    {

    }
}