using AppTransporte.model;
using System.Diagnostics;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEDetalleSolicitud : ContentPage
{
    private int idUsuario;
    private int idtipousuario;
    private readonly VMPedidos _pedidosViewModel;
    public VEDetalleSolicitud(Solicitud solicitud, VMPedidos pedidosViewModel, int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();
        _pedidosViewModel = pedidosViewModel;
        this.idUsuario = idUsuario;
        this.idtipousuario = idTipoUsuario;
        BindingContext = solicitud;
        solicitudid.Text = solicitud.IdSolicitud.ToString();
        fechaSolicitud.Text = solicitud.FechaSolicitud.ToString();
        clienteSolicitud.Text = solicitud.Cliente;
        descripcionSolicitud.Text = solicitud.Descripcion;
        estadoSolicitud.Text = solicitud.EstadoSolicitud;
    }

    private void Btn_atrasDetSolicitud(object sender, EventArgs e)
    {
		Navigation.PushAsync(new VESolicitudes(idUsuario, idtipousuario));
    }

    private async void Btn_CrearPedido(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var solicitud = button.CommandParameter as Solicitud;

        if (solicitud != null)
        {

            await Navigation.PushAsync(new VECrearPedido(solicitud, idUsuario, idtipousuario));
        }
    }
    private async void Btn_rechazarPedido(object sender, EventArgs e)
    {
        // Confirmar la acción
        bool confirmar = await DisplayAlert("Confirmar",
            "¿Está seguro que desea rechazar esta solicitud?",
            "Sí", "No");

        if (confirmar)
        {
            try
            {
                // Obtener la solicitud actual
                var solicitud = BindingContext as Solicitud;

                if (solicitud != null)
                {
                    // Obtener el comentario del usuario
                    string comentario = ComentarioEntry.Text?.Trim();

                    System.Diagnostics.Debug.WriteLine($"RECHAZANDO SOLICITUD");
                    System.Diagnostics.Debug.WriteLine($"ID Solicitud: {solicitud.IdSolicitud}");
                    System.Diagnostics.Debug.WriteLine($"Comentario: {comentario ?? "Sin comentario"}");

                    // Llamar al método específico para rechazar
                    var resultado = await App.Database.RechazarSolicitudAsync(
                        solicitud.IdSolicitud,
                        comentario);

                    System.Diagnostics.Debug.WriteLine($"Resultado - Exitoso: {resultado.Exitoso}");
                    System.Diagnostics.Debug.WriteLine($"Resultado - Mensaje: {resultado.Mensaje}");
                    System.Diagnostics.Debug.WriteLine($"Resultado - Filas: {resultado.FilasAfectadas}");

                    if (resultado.Exitoso && resultado.FilasAfectadas > 0)
                    {
                        // Actualizar los datos locales
                        solicitud.EstadoSolicitud = "Cancelada";
                        solicitud.IdEstadoSolicitud = 3;

                        if (!string.IsNullOrEmpty(comentario))
                        {
                            solicitud.Comentario = comentario;
                        }

                        // Actualizar la interfaz
                        estadoSolicitud.Text = "Cancelada";
                        ComentarioEntry.Text = string.Empty;

                        // Refrescar la lista en el ViewModel
                        if (_pedidosViewModel != null)
                        {
                            try
                            {
                                _pedidosViewModel.CargarPedidos();
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error al refrescar ViewModel: {ex.Message}");
                            }
                        }

                        // Mostrar mensaje de éxito
                        await DisplayAlert("Éxito",
                            "La solicitud ha sido cancelada correctamente.",
                            "OK");

                        // Regresar a la página anterior
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        // Mostrar el mensaje de error del procedimiento
                        await DisplayAlert("Error",
                            $"No se pudo cancelar la solicitud.\n\n{resultado.Mensaje}",
                            "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error",
                        "No se pudieron obtener los datos de la solicitud.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en Btn_rechazarPedido: {ex}");

                await DisplayAlert("Error",
                    $"Error inesperado: {ex.Message}",
                    "OK");
            }
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
                        await Navigation.PushAsync(new VEProcesoPedido(pedido, idUsuario, idtipousuario, null));
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