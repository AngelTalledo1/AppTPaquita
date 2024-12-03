using AppTransporte.model;
using AppTransporte.viewModel;
using Java.Time;

namespace AppTransporte.Interfaces;

public partial class VEProcesoPedido : ContentPage
{

    
    public VEProcesoPedido() //Pedido pedido
    {
        InitializeComponent();
        //BindingContext = pedido;
        //TituloPedido.Text = $"Pedido {pedido.IdPedido}";
        //Origen.Text = $"{pedido.Origen.Descripcion}";
        //Cantidad.Text = $"CalcularCantidadFinalizada / {pedido.Cantidad}";
        //OrigenSector.Text = $"{pedido.Origen.Sector}";
        //Estado.Text = $"{pedido.EstadoPedido.Descripcion}";
        //Destino.Text = $"{pedido.Destino.Descripcion}";
        //DestinoSector.Text = $"{pedido.Destino.Sector}";
        //NombreCliente.Text = $"Cliente: {pedido.Solicitud.Cliente.Persona.NombreCompleto}";
        //ServicioPedido.Text = $"";
        //cantidadTotalPedido.Text = $"Cantidad Pedido: {pedido.Cantidad} Barriles";
        //creadorAdmin.Text = $"Creado por: {pedido.Usuario.Persona.NombreCompleto}";
        //FechaSolicitud.Text = $"Fecha de Solicitud: {pedido.Solicitud.Fecha}";
        //creacionPedido.Text = $"Pedido Creado: {pedido.FechaCreacion}";
        //cantidadViajes.Text = $"Numero de viajes: {NumeroViajes}";


    }

    private void Btn_atrasEstado(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void expandir_Clicked(object sender, EventArgs e)
    {
        // ExpandirDetalle.IsExpanded = !ExpandirDetalle.IsExpanded;
    }
    //public int NumeroViajes(Pedido pedido)
    //{
    //    int viajes = pedido.Cantidad / 200;

    //    return viajes;
    //}

    private void ExpandirLinea_Clicked(object sender, EventArgs e)
    {
        
        //ExpandirLinea.IsExpanded = !ExpandirLinea.IsExpanded;
    }

    //public decimal CalcularCantidadFinalizada(Pedido pedido)
    //{
    //    decimal cantidadFinalizada = 0;

    //    foreach (var viaje in pedido.Viaje)
    //    {
    //         Obtener el último seguimiento (el más reciente) del viaje
    //        var ultimoSeguimiento = viaje.Seguimientos
    //            .OrderByDescending(s => s.FechaHora) // Ordenamos por FechaHora descendente
    //            .FirstOrDefault();

    //         Verificamos si el último estado es "Finalizado"
    //        if (ultimoSeguimiento != null && ultimoSeguimiento.Estado == "Finalizado")
    //        {
    //            cantidadFinalizada += viaje.Cantidad;  // Sumamos la cantidad de barriles del viaje
    //        }
    //    }

    //    return cantidadFinalizada;
    //}
}