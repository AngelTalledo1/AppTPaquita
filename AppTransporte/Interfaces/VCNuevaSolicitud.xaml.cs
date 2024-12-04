using AppTransporte.model;
using Microsoft.Maui.Graphics.Text;

namespace AppTransporte.Interfaces;

public partial class VCNuevaSolicitud : ContentPage
{
    public VCNuevaSolicitud()
    {
        InitializeComponent();
        TituloNuev.Text = "Nueva Solicitud";
        //IdClienteLabel.Text =
        Solicitar.IsVisible = true;
        Cancelar.IsVisible = true;
        //CargarIdCliente();
    }

    public VCNuevaSolicitud(Solicitud solicitud)
    {
        InitializeComponent();
        TituloNuev.Text = $"Modificar Solicitud {solicitud.IdSolicitud}";
        IdClienteLabel.Text = $"{solicitud.IdCliente}";
        descripcionEntry.Text = solicitud.Descripcion;
        Eliminar.IsVisible = true;
        Actualizar.IsVisible = true;
        BindingContext = solicitud;
    }


    private void Btn_atrasSolPedido(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    private void Btn_cancelar(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }


    private void Btn_Solicitar(object sender, EventArgs e)
    {
        // Simula la recolección de datos del formulario
        var solicitud = new
        {
            IdCliente = IdClienteLabel.Text,
            Direccion = descripcionEntry.Text
        };

        // Enviar el mensaje al "MenuPrincipal"
        // MessagingCenter.Send(this, "Nueva Solicitud", solicitud);

        // Opcional: Muestra un mensaje de confirmación al usuario
        DisplayAlert("Solicitud Enviada", "La solicitud se ha enviado al administrador.", "OK");

        // Regresa al menú principal
        Navigation.PushAsync(new VCMisSolicitudes());
    }

    private static string ObtenerIdCliente()
    {
        // Aquí puedes implementar la lógica para obtener el idCliente, 
        // por ejemplo, desde una API, base de datos, etc.
        // Esto es solo un ejemplo.
        return "12345";


    }

    private void Btn_EliminarSolicitud(object sender, EventArgs e)
    {

    }

    private void Btn_ModificarSolicitud(object sender, EventArgs e)
    {

    }
}