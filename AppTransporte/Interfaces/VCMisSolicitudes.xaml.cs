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
    private int idtipousuario;

    public VCMisSolicitudes(int idUsuario, int idtipoUsuario )
    {
        InitializeComponent();
        this.idUsuario = idUsuario;
        this.idtipousuario = idtipoUsuario;
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
        Navigation.PushAsync(new MenuCliente(idUsuario,idtipousuario));
    }

    private async void btn_NewSolicitud(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VCNuevaSolicitud(Cliente, idUsuario, idtipousuario));
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
            await Navigation.PushAsync(new VCNuevaSolicitud(solicitud, idUsuario, idtipousuario));
        }
    }

private async void verMiPedido_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud;

        if (solicitud != null)
        {
            try
            {
                // Cargar la lista de pedidos desde la base de datos
                var pedidos = await App.Database.ListarPedidosPorUsuario(idUsuario);

                if (pedidos != null && pedidos.Any())
                {
                    // Buscar el pedido correspondiente al IdSolicitud
                    var pedido = pedidos.FirstOrDefault(p => p.IdSolicitud == solicitud.IdSolicitud);

                    if (pedido != null)
                    {
                        // Navegar a la página de Proceso Pedido
                        await Navigation.PushAsync(new VEProcesoPedido(pedido, idUsuario, idtipousuario));
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