using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VECrearPedido : ContentPage
{
	public VECrearPedido(Solicitud solicitud)
	{
		InitializeComponent();
		BindingContext = solicitud;
		IdSolicitud.Text = solicitud.IdSolicitud.ToString();
		fechaEntry.Text = DateTime.Today.ToString();
	}

	private void Btn_atrasCrearPedido(object sender, EventArgs e)
	{
		Navigation.PopAsync();
	}

    private void Btn_crear(object sender, EventArgs e)
    {

    }
    private void Btn_cancelar(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void OnCantidadFluidoChanged(object sender, TextChangedEventArgs e)
    {
        const int capacidadMaxima = 200;
        if (int.TryParse(CantidadFluidoEntry.Text, out int cantidadFluido))
        {
            // Calcula los viajes necesarios y actualiza el Label
            int viajes = (int)Math.Ceiling((double)cantidadFluido / capacidadMaxima);
            ViajesLabel.Text = $"Viajes: {viajes}";
        }
        else
        {
            // Si no es un número válido, resetea el valor
            ViajesLabel.Text = "Viajes: 0";
        }
    }

    }