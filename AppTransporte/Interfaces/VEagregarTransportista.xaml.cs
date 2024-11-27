namespace AppTransporte.Interfaces;

public partial class VEagregarTransportista : ContentPage
{
	public VEagregarTransportista()
	{
		InitializeComponent();
	}
    private void Btn_atrasTrab(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    private async void OnGuardarTransportistaClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreTrabEntry.Text) ||
            string.IsNullOrWhiteSpace(NumeroDocEntry.Text) ||
            string.IsNullOrWhiteSpace(TelefonoTrabEntry.Text) ||
            string.IsNullOrWhiteSpace(DireccionTrabEntry.Text) ||
            string.IsNullOrWhiteSpace(TipoDocumentoTrabEntry.SelectedItem?.ToString()))

        {
            await DisplayAlert("Error", "Todos los campos obligatorios deben llenarse.", "OK");
            return;
        }
        int idTipoDoc = TipoDocumentoTrabEntry.SelectedIndex + 1;
        try
        {
            string[] apellidos = ApellidoTrabEntry.Text.Split(' ');

            // Llamar al servicio
            int resultado = await App.Database.AgregarTransportistaAsync(
                NombreTrabEntry.Text,
                apellidos[0],
                apellidos[1],
                idTipoDoc,
                NumeroDocEntry.Text,
                TelefonoTrabEntry.Text,
                DireccionTrabEntry.Text,
                EmailTrabEntry.Text
            );

            if (resultado > 0)
            {
                await DisplayAlert("Éxito", "Transportista agregado correctamente.", "OK");
                TipoDocumentoTrabEntry.SelectedIndex = -1;
                NombreTrabEntry.Text = "";
                NumeroDocEntry.Text = "";
                TelefonoTrabEntry.Text = "";
                DireccionTrabEntry.Text = "";
                EmailTrabEntry.Text = "";
            }
            else
            {
                await DisplayAlert("Error", "No se pudo agregar el transportista. Verifica los datos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Ocurrió un error al agregar el transportista. Inténtalo de nuevo.", "OK");
        }
    }
}