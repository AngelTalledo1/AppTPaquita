using AppTransporte.model;
using System.Diagnostics;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEDetalleSolicitud : ContentPage
{
    private readonly VMPedidos _pedidosViewModel;
    public VEDetalleSolicitud(Solicitud solicitud, VMPedidos pedidosViewModel)
	{
		InitializeComponent();
        _pedidosViewModel = pedidosViewModel;
        BindingContext = solicitud;
        solicitudid.Text = solicitud.IdSolicitud.ToString();
        fechaSolicitud.Text = solicitud.Fecha.ToString();
        clienteSolicitud.Text = solicitud.Cliente;
        descripcionSolicitud.Text = solicitud.Descripcion;
        estadoSolicitud.Text = solicitud.EstadoSolicitud;
    }

    private void Btn_atrasDetSolicitud(object sender, EventArgs e)
    {
		Navigation.PopAsync();
    }

    private async void Btn_CrearPedido(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud;

        if (solicitud != null)
        {

            await Navigation.PushAsync(new VECrearPedido(solicitud));
        }
    }

    private async void IrAPedido_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud;

        if (solicitud != null)
        {
            try
            {
                // Cargar la lista de pedidos desde la base de datos
                var pedidos = await App.Database.ListarPedidosAdminAsync();

                if (pedidos != null && pedidos.Any())
                {
                    // Buscar el pedido correspondiente al IdSolicitud
                    var pedido = pedidos.FirstOrDefault(p => p.IdSolicitud == solicitud.IdSolicitud);

                    if (pedido != null)
                    {
                        // Navegar a la página de Proceso Pedido
                        await Navigation.PushAsync(new VEProcesoPedido(pedido));
                    }
                    else
                    {
                        // Mostrar un mensaje si no se encuentra el pedido
                        await DisplayAlert("Información", "No se encontró un pedido asociado a esta solicitud.", "OK");
                    }
                }
                else
                {
                    // Mostrar un mensaje si la lista de pedidos está vacía
                    await DisplayAlert("Información", "La lista de pedidos está vacía. Verifica si se cargaron los datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejar errores en caso de que algo falle al obtener los pedidos
                await DisplayAlert("Error", $"Ocurrió un error al cargar los pedidos: {ex.Message}", "OK");
            }
        }
    }
}