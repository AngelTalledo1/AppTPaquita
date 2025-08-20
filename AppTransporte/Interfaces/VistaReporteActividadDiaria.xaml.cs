using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class VistaReporteActividadDiaria : ContentPage
    {
        private int _idTrabajador;
        private int _idUsuario;
        private int _idTipoUsuario;
        private DateTime _fechaSeleccionada;
        private ReporteDiarioCompleto _reporteActual;

        public DateTime FechaSeleccionada
        {
            get => _fechaSeleccionada;
            set => _fechaSeleccionada = value;
        }

        public VistaReporteActividadDiaria(int idTrabajador, int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idTrabajador = idTrabajador;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;
            _fechaSeleccionada = DateTime.Today;
            BindingContext = this;
        }

        private void fechaSelector_DateSelected(object sender, DateChangedEventArgs e)
        {
            _fechaSeleccionada = e.NewDate;
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void GenerarReporte_Clicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                // Obtener el reporte diario para la fecha seleccionada
                _reporteActual = await App.Database.ObtenerReporteDiarioCompletoAsync(_idTrabajador, _fechaSeleccionada);

                // Actualizar la interfaz con los datos
                MostrarDatosReporte();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo generarwwqqwwqqwwq el reporte: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private void MostrarDatosReporte()
        {
            if (_reporteActual == null)
            {
                // No hay datos para mostrar
                OcultarPanelesReporte();
                return;
            }

            // Actualizar información del trabajador
            lblNombreTrabajador.Text = _reporteActual.NombreTrabajador;
            lblCategoriaTrabajador.Text = _reporteActual.Categoria;
            lblFechaReporte.Text = _reporteActual.FechaFormateada;

            // Actualizar resumen
            lblTotalViajes.Text = _reporteActual.TotalViajes.ToString();
            lblTotalTareas.Text = _reporteActual.TotalTareas.ToString();

            // Actualizar colecciones
            ListaViajes.ItemsSource = _reporteActual.Actividades;
            ListaTareas.ItemsSource = _reporteActual.TareasAdicionales;

            // Mostrar todos los paneles y botón de exportar
            FrameInfoTrabajador.IsVisible = true;
            FrameResumen.IsVisible = true;
            FrameViajes.IsVisible = true;
            FrameTareas.IsVisible = true;
            BtnExportarPDF.IsVisible = true;
        }

        private void OcultarPanelesReporte()
        {
            FrameInfoTrabajador.IsVisible = false;
            FrameResumen.IsVisible = false;
            FrameViajes.IsVisible = false;
            FrameTareas.IsVisible = false;
            BtnExportarPDF.IsVisible = false;
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            if (_reporteActual == null)
            {
                await DisplayAlert("Error", "No hay datos para exportar", "OK");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                // Generar el documento PDF
               byte[] pdfBytes = await App.Database.GenerarReporteActividadDiariaPDF(_reporteActual);

                // Guardar y compartir el PDF
                string fileName = $"Reporte_Diario_{_fechaSeleccionada:yyyy-MM-dd}.pdf";
                string tempPath = Path.Combine(FileSystem.CacheDirectory, fileName);

                File.WriteAllBytes(tempPath, pdfBytes);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir reporte de actividad diaria",
                    File = new ShareFile(tempPath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo exportar el PDF: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }
    }

    // Converters para la UI
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isCompleted && isCompleted)
            {
                return Color.FromArgb("#107C10"); // Verde para Completado
            }
            return Color.FromArgb("#F25022"); // Rojo para Pendiente
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringNotNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
