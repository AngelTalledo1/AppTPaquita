using System;
using Microsoft.Maui.Controls;

namespace AppTransporte.Interfaces
{
    public partial class VEReportes : ContentPage
    {
        private int _idUsuario;
        private int _idTipoUsuario;

        public VEReportes(int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MenuPrincipal(_idUsuario, _idTipoUsuario));
        }

        private async void ReporteTrabajador_Clicked(object sender, EventArgs e)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                // Determinar qué filtro está seleccionado
                string periodoSeleccionado = "Diario"; // Por defecto

                // Lógica para obtener el periodo seleccionado
                // (Puedes implementar esto de manera más elegante)

                await Task.Delay(2000); // Simulación de generación de reporte

                LoadingOverlay.IsVisible = false;
                await DisplayAlert("Reporte Generado",
                    $"Se ha generado el reporte de trabajadores con periodo {periodoSeleccionado}.",
                    "OK");
            }
            catch (Exception ex)
            {
                LoadingOverlay.IsVisible = false;
                await DisplayAlert("Error", $"No se pudo generar el reporte: {ex.Message}", "OK");
            }
        }

        private async void ReporteServicios_Clicked(object sender, EventArgs e)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                await Task.Delay(2000); // Simulación de generación de reporte

                LoadingOverlay.IsVisible = false;
                await DisplayAlert("Reporte Generado",
                    "Se ha generado el reporte de volumen transportado por servicio.",
                    "OK");
            }
            catch (Exception ex)
            {
                LoadingOverlay.IsVisible = false;
                await DisplayAlert("Error", $"No se pudo generar el reporte: {ex.Message}", "OK");
            }
        }

        private async void ReporteDesvios_Clicked(object sender, EventArgs e)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                await Task.Delay(2000); // Simulación de generación de reporte

                LoadingOverlay.IsVisible = false;
                await DisplayAlert("Reporte Generado",
                    "Se ha generado el reporte de desvíos.",
                    "OK");
            }
            catch (Exception ex)
            {
                LoadingOverlay.IsVisible = false;
                await DisplayAlert("Error", $"No se pudo generar el reporte: {ex.Message}", "OK");
            }
        }

        private async void ReportePedidos_Clicked(object sender, EventArgs e)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                await Task.Delay(2000); // Simulación de generación de reporte

                LoadingOverlay.IsVisible = false;
                await DisplayAlert("Reporte Generado",
                    "Se ha generado el reporte de pedidos agrupados por estado.",
                    "OK");
            }
            catch (Exception ex)
            {
                LoadingOverlay.IsVisible = false;
                await DisplayAlert("Error", $"No se pudo generar el reporte: {ex.Message}", "OK");
            }
        }

        private async void ReporteSolicitudes_Clicked(object sender, EventArgs e)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                await Task.Delay(2000); // Simulación de generación de reporte

                LoadingOverlay.IsVisible = false;
                await DisplayAlert("Reporte Generado",
                    "Se ha generado el reporte de atención de solicitudes (TP).",
                    "OK");
            }
            catch (Exception ex)
            {
                LoadingOverlay.IsVisible = false;
                await DisplayAlert("Error", $"No se pudo generar el reporte: {ex.Message}", "OK");
            }
        }
    }
}
