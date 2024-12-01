
using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEagregarCliente : ContentPage
{
	public VEagregarCliente()
	{
		InitializeComponent();
        GuardarCliente.IsVisible = true;
        TituloLabel.Text = "Informacion General";
	}
    public VEagregarCliente(Cliente clienteSelect)
    {
        InitializeComponent();
        ActualizarCliente.IsVisible = true;
        TituloLabel.Text = "Modificar Cliente";
        NombreEntry.Text = clienteSelect.Persona.Nombre;
        ApellidoEntry.Text = $"{clienteSelect.ApePaterno} {clienteSelect.ApeMaterno} ";
        TipoDocumentoPicker.SelectedIndex = clienteSelect.Persona.IdTipoDoc;
        NumeroDocEntry.Text = clienteSelect.NumDoc;
        TelefonoEntry.Text = clienteSelect.Telefono;
        DireccionEntry.Text = clienteSelect.Direccion;
        EmailEntry.Text = clienteSelect.Email;

        BindingContext = clienteSelect;
    }

    private void Btn_atras(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    private async void OnGuardarClienteClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(NumeroDocEntry.Text) ||
            string.IsNullOrWhiteSpace(TelefonoEntry.Text) ||
            string.IsNullOrWhiteSpace(DireccionEntry.Text) ||
            string.IsNullOrWhiteSpace(TipoDocumentoPicker.SelectedItem?.ToString()))
           
        {
            await DisplayAlert("Error", "Todos los campos obligatorios deben llenarse.", "OK");
            return;
        }
        int idTipoDoc = TipoDocumentoPicker.SelectedIndex + 1;
        try
        {
            string[] apellidos = ApellidoEntry.Text.Split(' ');

            // Llamar al servicio
            int resultado = await App.Database.AgregarClienteAsync(
                NombreEntry.Text,
                apellidos[0],
                apellidos[1],
                idTipoDoc,
                NumeroDocEntry.Text,
                TelefonoEntry.Text,
                DireccionEntry.Text,
                EmailEntry.Text
            );

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Cliente agregado correctamente.", "OK");
                TipoDocumentoPicker.SelectedIndex = -1;
                NombreEntry.Text = "";
                NumeroDocEntry.Text = "";
                TelefonoEntry.Text= "";
                DireccionEntry.Text = "";
                EmailEntry.Text = "";
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar el cliente. Verifica los datos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "OK");
        }
    }

    private void Btn_ActualizarCliente(object sender, EventArgs e)
    {

    }
}