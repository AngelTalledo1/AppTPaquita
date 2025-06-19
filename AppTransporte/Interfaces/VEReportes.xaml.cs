using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AppTransporte.Interfaces;

public partial class VEReportes : ContentPage
{
    private int _idUsuario;
    private int _idTipoUsuario;
    private DateTime fechaInicio = DateTime.Now.AddMonths(-1); // Por defecto, último mes
    private DateTime fechaFin = DateTime.Now;
    private string tipoReporteSeleccionado = "Diario"; // Valor por defecto
    private int? idTrabajadorSeleccionado = null;

    public VEReportes(int idUsuario, int idTipoUsuario)
    {
        InitializeComponent();
        _idUsuario = idUsuario;
        _idTipoUsuario = idTipoUsuario;

        // Inicializar controles de fecha con valores predeterminados
        fechaInicioPicker.Date = fechaInicio;
        fechaFinPicker.Date = fechaFin;

        // Establecer periodo por defecto
        RadioDiario.IsChecked = true;

        // Cargar datos de trabajadores para el picker
        CargarTrabajadores();
    }

    private async void CargarTrabajadores()
    {
        try
        {
            LoadingOverlay.IsVisible = true;

            // Obtener lista de trabajadores
            var trabajadores = await App.Database.ObtenerTrabajadoresAsync();

            // Agregar opción "Todos los trabajadores" al inicio
            var listaTrabajadores = new List<object>
            {
                new { IdTrabajador = (int?)null, NombreCompleto = "Todos los trabajadores" }
            };

            // Agregar el resto de trabajadores
            foreach (var trab in trabajadores)
            {
                listaTrabajadores.Add(new
                {
                    IdTrabajador = trab.IdTrabajador,
                    NombreCompleto = $"{trab.Nombre} {trab.apePaterno} {trab.apeMaterno}".Trim()
                });
            }

            // Configurar el picker
            trabajadorPicker.ItemsSource = listaTrabajadores;
            trabajadorPicker.ItemDisplayBinding = new Binding("NombreCompleto");
            trabajadorPicker.SelectedIndex = 0; // Seleccionar "Todos" por defecto

            LoadingOverlay.IsVisible = false;
        }
        catch (Exception ex)
        {
            LoadingOverlay.IsVisible = false;
            await DisplayAlert("Error", $"No se pudieron cargar los trabajadores: {ex.Message}", "OK");
        }
    }

    private void Btn_atras(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPrincipal(_idUsuario, _idTipoUsuario));
    }

    private void fechaInicioPicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        fechaInicio = e.NewDate;
    }

    private void fechaFinPicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        fechaFin = e.NewDate;
    }

    private void RadioDiario_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            tipoReporteSeleccionado = "Diario";
        }
    }

    private void RadioSemanal_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            tipoReporteSeleccionado = "Semanal";
        }
    }

    private void RadioMensual_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            tipoReporteSeleccionado = "Mensual";
        }
    }

    private void trabajadorPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (trabajadorPicker.SelectedItem != null)
        {
            var selectedItem = trabajadorPicker.SelectedItem as dynamic;
            idTrabajadorSeleccionado = selectedItem.IdTrabajador;
        }
    }

    private async void ReporteTrabajador_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Validar fechas
            if (fechaInicio > fechaFin)
            {
                await DisplayAlert("Error", "La fecha de inicio no puede ser posterior a la fecha de fin", "OK");
                return;
            }

            LoadingOverlay.IsVisible = true;

            // Obtener los datos del reporte
            var reporteData = await App.Database.ObtenerReporteTrabajadorAsync(
                idTrabajadorSeleccionado,
                fechaInicio,
                fechaFin,
                tipoReporteSeleccionado
            );

            LoadingOverlay.IsVisible = false;

            if (reporteData == null || reporteData.Count == 0)
            {
                await DisplayAlert("Sin datos", "No hay datos disponibles para el período seleccionado.", "OK");
                return;
            }

            // Navegar a la página de visualización del reporte
            await Navigation.PushAsync(new VistaPreviaReporte(reporteData, fechaInicio, fechaFin, tipoReporteSeleccionado));
        }
        catch (Exception ex)
        {
            LoadingOverlay.IsVisible = false;
            await DisplayAlert("Error", $"No se pudo generar el reporte: {ex.Message}", "OK");
        }
    }

    // Otros métodos para los demás tipos de reportes
    private async void ReporteServicios_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VistaReporteServicios());
    }

    private async void ReporteDesvios_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("En desarrollo", "Esta funcionalidad estará disponible próximamente.", "OK");
    }

    private async void ReportePedidos_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VistaReportePedidos());
    }

    private async void ReporteSolicitudes_Clicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new VistaReporteAtencionSolicitudes());
    }
}
