namespace AppTransporte.Interfaces;

using AppTransporte.model;
using AppTransporte.viewModel;

public partial class VEpedidos : ContentPage
{
	public VEpedidos()
	{
		InitializeComponent();
        BindingContext = new VMPedidos();
    }

    private void Btn_atrasPedidos(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void PedidosCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var pedido = e.CurrentSelection.FirstOrDefault() as Pedidos;

        if (pedido != null)
        {
            // Cambiar el color del Frame cuando se selecciona
            var frame = (sender as CollectionView)?.SelectedItem as Frame;
            if (frame != null)
            {
                frame.BackgroundColor = Colors.LightBlue; // Cambia a un color seleccionado
            }

            // Muestra una ventana emergente con información del pedido seleccionado
            await Application.Current.MainPage.DisplayAlert("Pedido Seleccionado",
                                $"Número de Pedido: {pedido.Numero}\n" +
                                $"Peso: {pedido.cantidad}\n" +
                                $"Estado: {pedido.estado}",
                                "OK");
        }

    // Deseleccionar el ítem después de mostrar la ventana emergente (opcional)
    ((CollectionView)sender).SelectedItem = null;
    }


}