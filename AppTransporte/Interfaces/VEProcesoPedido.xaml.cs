using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEProcesoPedido : ContentPage
{

    
    public VEProcesoPedido()//string numero, double peso, string estado)
    {
        InitializeComponent();
        //var Barra = new VMEstado
        /*{
            Titulo = $"Pedido {numero}",
            EstadoPedido = estado
        };

        BindingContext = Barra;

        Barra.OnOrderCompleted += async () =>
        {
            await DisplayAlert("Éxito", "Pedido completado con éxito", "OK");
        };*/
    }

    private void Btn_atrasEstado(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void expandir_Clicked(object sender, EventArgs e)
    {
        ExpandirDetalle.IsExpanded = !ExpandirDetalle.IsExpanded;
    }

    private void ExpandirLinea_Clicked(object sender, EventArgs e)
    {
        ExpandirLinea.IsExpanded = !ExpandirLinea.IsExpanded;
    }
}