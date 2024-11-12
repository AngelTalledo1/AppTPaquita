namespace AppTransporte.Interfaces;

public partial class MenuTransportista : ContentPage
{
    public MenuTransportista()
    {
        InitializeComponent();
    }

    private void Btn_atrasMenuTransportista(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    private async void ReporteTransportista_Clicked(object sender, EventArgs e)
    {

        // await Navigation.PushAsync(new NewPage1());

    }

    private async void pedidosTransportista_Clicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new VTpedidos());
    }

}
