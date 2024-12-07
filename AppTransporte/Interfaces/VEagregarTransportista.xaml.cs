using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces;

public partial class VEagregarTransportista : ContentPage
{
    private Trabajador trabajador;
    public VEagregarTransportista()
    {
        InitializeComponent();
        tituloLabel.Text = "Agregar  Trabajador";
        GuardarTrabajador.IsVisible = true;
    }
    public VEagregarTransportista(Trabajador trabajador)
    {
        InitializeComponent();
        this.trabajador = trabajador;
        BindingContext = trabajador;
        tituloLabel.Text = "Modificar Trabajador";
        ActualizarTrabajador.IsVisible = true;
        trabajadorPicker.SelectedIndex = trabajador.idcategoria - 1;
        NombreTrabEntry.Text = trabajador.Nombre;
        ApellidoTrabEntry.Text = trabajador.apePaterno + " " + trabajador.apeMaterno;
        TipoDocumentoTrabPicker.SelectedIndex = trabajador.idtipoDoc;
        NumeroDocEntry.Text = trabajador.numDoc;
        TelefonoTrabEntry.Text = trabajador.Telefono;
        LicenciaTrabEntry.Text = trabajador.licencia;
        DireccionTrabEntry.Text = trabajador.direccion;
        EmailTrabEntry.Text = trabajador.email;

    }
    private void Btn_atrasTrab(object sender, EventArgs e)
    {
        Navigation.PopAsync();
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

            // Llamar al servicio
            int resultado = await App.Database.AgregarTrabajadorAsync(
                NombreTrabEntry.Text,
                apellidos[0],
                apellidos[1],
                idTipoDoc,
                NumeroDocEntry.Text,
                TelefonoTrabEntry.Text,
                DireccionTrabEntry.Text,
                EmailTrabEntry.Text,
                idCat,
                LicenciaTrabEntry.Text
            );

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Trabajador agregado correctamente.", "OK");
                TipoDocumentoTrabPicker.SelectedIndex = -1;
                NombreTrabEntry.Text = "";
                ApellidoTrabEntry.Text = "";
                NumeroDocEntry.Text = "";
                TelefonoTrabEntry.Text = "";
                DireccionTrabEntry.Text = "";
                EmailTrabEntry.Text = "";
                trabajadorPicker.SelectedIndex = -1;
                LicenciaTrabEntry.Text = "";
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar el trabajador. Verifica los datos.", "OK");
            }
        }
        catch (Exception ex)
        {
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

            // Llamar al servicio
            int resultado = await App.Database.ModificarTrabajadorAsync(
                trabajador.IdTrabajador,
                NombreTrabEntry.Text,
                apellidos[0],
                apellidos[1],
                idTipoDoc,
                NumeroDocEntry.Text,
                TelefonoTrabEntry.Text,
                DireccionTrabEntry.Text,
                EmailTrabEntry.Text,
                idCat,
                LicenciaTrabEntry.Text
            );

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Trabajador actualizado correctamente.", "OK");
                Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar el trabajador. Verifica los datos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Ocurrió un error al agregar el trabajador. Inténtalo de nuevo.", "OK");
        }
    }

}