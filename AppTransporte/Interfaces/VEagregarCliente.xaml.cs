
using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEagregarCliente : ContentPage
{
    public int id_cliente { get; set; }
    private int _idUsuario;
    private int _idTipoUsuario;
    public VEagregarCliente(int idUsuario, int idTipoUsuario)
	{
		InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        GuardarCliente.IsVisible = true;
        TituloLabel.Text = "Informacion General";
	}
    public VEagregarCliente(Cliente clienteSelect, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        id_cliente = clienteSelect.IdCliente;
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        ActualizarCliente.IsVisible = true;
        tituloInterfaz.Text = "Modificar Cliente";
        TituloLabel.Text = "Informacion General";
        NombreEntry.Text = clienteSelect.Nombre;
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
        Navigation.PushAsync(new VEclientes(_idUsuario,_idTipoUsuario));
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
                await Navigation.PushAsync(new VEclientes(_idUsuario,_idTipoUsuario));

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

    private async void  Btn_ActualizarCliente(object sender, EventArgs e)
    {
        int idTipoDoc = TipoDocumentoPicker.SelectedIndex + 1;
        try
        {
            string[] apellidos = ApellidoEntry.Text.Split(' ');

            // Llamar al servicio
            int resultado = await App.Database.ModificarClienteAsync(
                id_cliente,
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
                await DisplayAlert("Éxito", "Cliente modificado correctamente.", "OK");
                await Navigation.PushAsync(new VEclientes(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo modificar el cliente. Verifica los datos.", "OK");
                await Navigation.PushAsync(new VEclientes(_idUsuario, _idTipoUsuario));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "OK");
        }
    }
}