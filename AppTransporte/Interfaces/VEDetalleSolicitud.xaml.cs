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
        Console.WriteLine(solicitud.Fecha);
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
                        // Navegar a la p�gina de Proceso Pedido
                        await Navigation.PushAsync(new VEProcesoPedido(pedido));
                    }
                    else
                    {
                        // Mostrar un mensaje si no se encuentra el pedido
                        await DisplayAlert("Informaci�n", "No se encontr� un pedido asociado a esta solicitud.", "OK");
                    }
                }
                else
                {
                    // Mostrar un mensaje si la lista de pedidos est� vac�a
                    await DisplayAlert("Informaci�n", "La lista de pedidos est� vac�a. Verifica si se cargaron los datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejar errores en caso de que algo falle al obtener los pedidos
                await DisplayAlert("Error", $"Ocurri� un error al cargar los pedidos: {ex.Message}", "OK");
            }
        }
    }
}