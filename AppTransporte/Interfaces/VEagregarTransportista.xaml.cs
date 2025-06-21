using AppTransporte.model;
using AppTransporte.viewModel;
using System.ComponentModel;

namespace AppTransporte.Interfaces;

public partial class VEagregarTransportista : ContentPage, INotifyPropertyChanged
{
    private Trabajador trabajador;
    private int _idUsuario;
    private int _idTipoUsuario;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public VEagregarTransportista(int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        tituloLabel.Text = "Agregar Trabajador";
        GuardarTrabajador.IsVisible = true;

        // Configurar el switch
        estado_switch.Toggled += (s, e) => {
            estado_label.Text = estado_switch.IsToggled ? "Activo" : "Inactivo";
        };
    }

    public VEagregarTransportista(Trabajador trabajador, int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        this._idTipoUsuario = idUsuario;
        this._idUsuario = idTipoUsuario;
        this.trabajador = trabajador;
        BindingContext = trabajador;
        tituloLabel.Text = "Modificar Trabajador";
        ActualizarTrabajador.IsVisible = true;

        // Cargar datos del trabajador
        trabajadorPicker.SelectedIndex = trabajador.idcategoria - 1;
        NombreTrabEntry.Text = trabajador.Nombre;
        ApellidoTrabEntry.Text = trabajador.apePaterno + " " + trabajador.apeMaterno;
        TipoDocumentoTrabPicker.SelectedIndex = trabajador.idtipoDoc - 1;
        NumeroDocEntry.Text = trabajador.numDoc;
        TelefonoTrabEntry.Text = trabajador.Telefono;
        LicenciaTrabEntry.Text = trabajador.licencia;
        DireccionTrabEntry.Text = trabajador.direccion;
        EmailTrabEntry.Text = trabajador.email;

        // NUEVO: Configurar estado
        estado_switch.IsToggled = trabajador.estado;
        estado_label.Text = trabajador.estado ? "Activo" : "Inactivo";

        // Configurar el switch
        estado_switch.Toggled += (s, e) => {
            estado_label.Text = estado_switch.IsToggled ? "Activo" : "Inactivo";
        };
    }

    private void Btn_atrasTrab(object sender, EventArgs e)
    {
        Navigation.PushAsync(new VEtransportistas(_idUsuario, _idTipoUsuario));
    }

    private void categoriaPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedOption = trabajadorPicker.SelectedItem?.ToString();

        if (selectedOption == "Transportista")
        {
            licenciaField.IsVisible = true;
        }
        else
        {
            licenciaField.IsVisible = false;
        }
    }

    private async void Btn_AgregarTrabajador(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreTrabEntry.Text) ||
            string.IsNullOrWhiteSpace(NumeroDocEntry.Text) ||
            string.IsNullOrWhiteSpace(TelefonoTrabEntry.Text) ||
            string.IsNullOrWhiteSpace(DireccionTrabEntry.Text) ||
            string.IsNullOrWhiteSpace(TipoDocumentoTrabPicker.SelectedItem?.ToString()))
        {
            await DisplayAlert("Error", "Todos los campos obligatorios deben llenarse.", "OK");
            return;
        }

        int idTipoDoc = TipoDocumentoTrabPicker.SelectedIndex + 1;
        int idCat = trabajadorPicker.SelectedIndex + 1;

        try
        {
            string[] apellidos = ApellidoTrabEntry.Text.Split(' ');

            // Llamar al servicio CON ESTADO
            int resultado = await App.Database.AgregarTrabajadorConEstadoAsync(
                NombreTrabEntry.Text,
                apellidos.Length > 0 ? apellidos[0] : "",
                apellidos.Length > 1 ? apellidos[1] : "",
                idTipoDoc,
                NumeroDocEntry.Text,
                TelefonoTrabEntry.Text,
                DireccionTrabEntry.Text,
                EmailTrabEntry.Text,
                idCat,
                LicenciaTrabEntry.Text,
                estado_switch.IsToggled // NUEVO PARÁMETRO
            );

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Trabajador agregado correctamente.", "OK");
                await Navigation.PushAsync(new VEtransportistas(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar el trabajador. Verifica los datos.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await DisplayAlert("Error", "Ocurrió un error al agregar el trabajador. Inténtalo de nuevo.", "OK");
        }
    }

    private async void ActualizarTrabajador_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreTrabEntry.Text) ||
            string.IsNullOrWhiteSpace(NumeroDocEntry.Text) ||
            string.IsNullOrWhiteSpace(TelefonoTrabEntry.Text) ||
            string.IsNullOrWhiteSpace(DireccionTrabEntry.Text) ||
            string.IsNullOrWhiteSpace(TipoDocumentoTrabPicker.SelectedItem?.ToString()))
        {
            await DisplayAlert("Error", "Todos los campos obligatorios deben llenarse.", "OK");
            return;
        }

        int idTipoDoc = TipoDocumentoTrabPicker.SelectedIndex + 1;
        int idCat = trabajadorPicker.SelectedIndex + 1;

        try
        {
            string[] apellidos = ApellidoTrabEntry.Text.Split(' ');

            // Llamar al servicio CON ESTADO
            int resultado = await App.Database.ModificarTrabajadorConEstadoAsync(
                trabajador.IdTrabajador,
                NombreTrabEntry.Text,
                apellidos.Length > 0 ? apellidos[0] : "",
                apellidos.Length > 1 ? apellidos[1] : "",
                idTipoDoc,
                NumeroDocEntry.Text,
                TelefonoTrabEntry.Text,
                DireccionTrabEntry.Text,
                EmailTrabEntry.Text,
                idCat,
                LicenciaTrabEntry.Text,
                estado_switch.IsToggled // NUEVO PARÁMETRO
            );

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Trabajador actualizado correctamente.", "OK");
                await Navigation.PushAsync(new VEtransportistas(_idUsuario, _idTipoUsuario));
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar el trabajador. Verifica los datos.", "OK");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Ocurrió un error al actualizar el trabajador. Inténtalo de nuevo.", "OK");
        }
    }
}