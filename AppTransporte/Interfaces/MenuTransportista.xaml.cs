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

    private async void btn_misViajes(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VTmisViajes());
    }

}
