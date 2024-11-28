namespace AppTransporte.Interfaces;

public partial class VCNuevaSolicitud : ContentPage
{
    public VCNuevaSolicitud()
    {
        InitializeComponent();
        CargarIdCliente();
    }

    private void Btn_Solicitar(object sender, EventArgs e)
    {
        // Simula la recolecci�n de datos del formulario
        var solicitud = new
        {
            IdCliente = IdClienteLabel.Text,
            Direccion = descripcionEntry.Text
        };

        // Enviar el mensaje al "MenuPrincipal"
        MessagingCenter.Send(this, "Nueva Solicitud", solicitud);

        // Opcional: Muestra un mensaje de confirmaci�n al usuario
        DisplayAlert("Solicitud Enviada", "La solicitud se ha enviado al administrador.", "OK");

        // Regresa al men� principal
        Navigation.PushAsync(new VCMisSolicitudes());
    }
    private void Btn_atrasSolPedido(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    private void Btn_cancelar(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    private void CargarIdCliente()
    {
        // Supongamos que obtienes el idCliente desde un servicio o base de datos
        string idCliente = ObtenerIdCliente();
        IdClienteLabel.Text = idCliente;
    }

    private static string ObtenerIdCliente()
    {
        // Aqu� puedes implementar la l�gica para obtener el idCliente, 
        // por ejemplo, desde una API, base de datos, etc.
        // Esto es solo un ejemplo.
        return "12345";


    }
}