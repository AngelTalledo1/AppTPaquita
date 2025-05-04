using AppTransporte.model;

namespace AppTransporte.Interfaces;

public partial class VEagregarCliente : ContentPage
{
    public int id_cliente { get; set; }
    private int _idUsuario;
    private int _idTipoUsuario;
    private const int RUC_LENGTH = 11;
    private const int DNI_LENGTH = 8;

    public VEagregarCliente(int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        GuardarCliente.IsVisible = true;
        TituloLabel.Text = "Información General";

        // Establecer DNI como valor predeterminado
        TipoDocumentoPicker.SelectedIndex = 0;

        // Configurar el evento para el cambio en el picker de tipo de documento
        TipoDocumentoPicker.SelectedIndexChanged += TipoDocumentoPicker_SelectedIndexChanged;
    }

    public VEagregarCliente(Cliente clienteSelect, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        id_cliente = clienteSelect.IdCliente;
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        ActualizarCliente.IsVisible = true;
        tituloInterfaz.Text = "Modificar Cliente";
        TituloLabel.Text = "Información General";
        NombreEntry.Text = clienteSelect.Nombre;
        ApellidoEntry.Text = $"{clienteSelect.ApePaterno} {clienteSelect.ApeMaterno} ";
        TipoDocumentoPicker.SelectedIndex = clienteSelect.Persona.IdTipoDoc;
        NumeroDocEntry.Text = clienteSelect.NumDoc;
        TelefonoEntry.Text = clienteSelect.Telefono;
        DireccionEntry.Text = clienteSelect.Direccion;
        EmailEntry.Text = clienteSelect.Email;

        // Configurar el evento para el cambio en el picker de tipo de documento
        TipoDocumentoPicker.SelectedIndexChanged += TipoDocumentoPicker_SelectedIndexChanged;

        // Aplicar la lógica de visibilidad según el tipo de documento seleccionado
        AjustarCamposPorTipoDocumento();

        BindingContext = clienteSelect;
    }

    private void TipoDocumentoPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        AjustarCamposPorTipoDocumento();
    }

    private void AjustarCamposPorTipoDocumento()
    {
        // El picker tiene "DNI" en posición 0 y "RUC" en posición 1
        string tipoDocumento = TipoDocumentoPicker.SelectedItem?.ToString();

        // Obtener la Grid que contiene el campo de apellidos (segunda Grid en VerticalStackLayout)
        Grid apellidoGrid = (Grid)((VerticalStackLayout)((ScrollView)((Grid)TituloLabel.Parent.Parent).Children[0]).Content).Children[2];

        if (tipoDocumento == "RUC")
        {
            // Ocultar campo de apellidos para RUC
            apellidoGrid.IsVisible = false;

            // Limpiar campo de apellidos
            ApellidoEntry.Text = "";

            // Ajustar MaxLength del campo NumeroDocEntry para RUC
            NumeroDocEntry.MaxLength = RUC_LENGTH;
        }
        else if (tipoDocumento == "DNI")
        {
            // Mostrar campo de apellidos para DNI
            apellidoGrid.IsVisible = true;

            // Ajustar MaxLength del campo NumeroDocEntry para DNI
            NumeroDocEntry.MaxLength = DNI_LENGTH;
        }
    }

    private void Btn_atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEclientes(_idUsuario, _idTipoUsuario));
    }

    private async void OnGuardarClienteClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(NumeroDocEntry.Text) ||
            string.IsNullOrWhiteSpace(TelefonoEntry.Text) ||
            string.IsNullOrWhiteSpace(DireccionEntry.Text) ||
            TipoDocumentoPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Error", "Todos los campos obligatorios deben llenarse.", "OK");
            return;
        }

        // Validación específica según tipo de documento
        string tipoDocumento = TipoDocumentoPicker.SelectedItem?.ToString();
        if (tipoDocumento == "RUC")
        {
            // Validar longitud de RUC
            if (NumeroDocEntry.Text.Length != RUC_LENGTH)
            {
                await DisplayAlert("Error", "Verifique su número de RUC. Debe tener 11 dígitos.", "OK");
                return;
            }
        }
        else if (tipoDocumento == "DNI")
        {
            // Validar longitud de DNI
            if (NumeroDocEntry.Text.Length != DNI_LENGTH)
            {
                await DisplayAlert("Error", "Verifique su número de DNI. Debe tener 8 dígitos.", "OK");
                return;
            }

            // Validar si el campo de apellidos está vacío
            if (string.IsNullOrWhiteSpace(ApellidoEntry.Text))
            {
                await DisplayAlert("Error", "El campo de apellidos es obligatorio para DNI.", "OK");
                return;
            }

            // Validar que ambos apellidos estén presentes
            string[] apellidos = ApellidoEntry.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (apellidos.Length < 2)
            {
                await DisplayAlert("Error", "Ambos apellidos son obligatorios.", "OK");
                return;
            }
        }

        int idTipoDoc = TipoDocumentoPicker.SelectedIndex + 1;
        try
        {
            string apePaterno = "";
            string apeMaterno = "";

            // Solo procesar apellidos si no es RUC
            if (tipoDocumento != "RUC")
            {
                string[] apellidos = ApellidoEntry.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (apellidos.Length >= 2)
                {
                    apePaterno = apellidos[0];
                    apeMaterno = apellidos[1];
                }
            }

            // Llamar al servicio
            int resultado = await App.Database.AgregarClienteAsync(
                NombreEntry.Text,
                apePaterno,
                apeMaterno,
                idTipoDoc,
                NumeroDocEntry.Text,
                TelefonoEntry.Text,
                DireccionEntry.Text,
                EmailEntry.Text
            );

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Cliente agregado correctamente.", "OK");
                await Navigation.PushAsync(new VEclientes(_idUsuario, _idTipoUsuario));
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

    private async void Btn_ActualizarCliente(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(NumeroDocEntry.Text) ||
            string.IsNullOrWhiteSpace(TelefonoEntry.Text) ||
            string.IsNullOrWhiteSpace(DireccionEntry.Text) ||
            TipoDocumentoPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Error", "Todos los campos obligatorios deben llenarse.", "OK");
            return;
        }

        // Validación específica según tipo de documento
        string tipoDocumento = TipoDocumentoPicker.SelectedItem?.ToString();
        if (tipoDocumento == "RUC")
        {
            // Validar longitud de RUC
            if (NumeroDocEntry.Text.Length != RUC_LENGTH)
            {
                await DisplayAlert("Error", "Verifique su número de RUC. Debe tener 11 dígitos.", "OK");
                return;
            }
        }
        else if (tipoDocumento == "DNI")
        {
            // Validar longitud de DNI
            if (NumeroDocEntry.Text.Length != DNI_LENGTH)
            {
                await DisplayAlert("Error", "Verifique su número de DNI. Debe tener 8 dígitos.", "OK");
                return;
            }

            // Validar si el campo de apellidos está vacío
            if (string.IsNullOrWhiteSpace(ApellidoEntry.Text))
            {
                await DisplayAlert("Error", "El campo de apellidos es obligatorio para DNI.", "OK");
                return;
            }

            // Validar que ambos apellidos estén presentes
            string[] apellidos = ApellidoEntry.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (apellidos.Length < 2)
            {
                await DisplayAlert("Error", "Ambos apellidos son obligatorios.", "OK");
                return;
            }
        }

        int idTipoDoc = TipoDocumentoPicker.SelectedIndex + 1;
        try
        {
            string apePaterno = "";
            string apeMaterno = "";

            // Solo procesar apellidos si no es RUC
            if (tipoDocumento != "RUC")
            {
                string[] apellidos = ApellidoEntry.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (apellidos.Length >= 2)
                {
                    apePaterno = apellidos[0];
                    apeMaterno = apellidos[1];
                }
            }

            // Llamar al servicio
            int resultado = await App.Database.ModificarClienteAsync(
                id_cliente,
                NombreEntry.Text,
                apePaterno,
                apeMaterno,
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