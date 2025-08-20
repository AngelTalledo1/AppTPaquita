using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class VistaReporteTareasAdicionales : ContentPage
    {
        private int _idTrabajador;
        private int _idUsuario;
        private int _idTipoUsuario;
        private DateTime _fechaInicio;
        private DateTime _fechaFin;
        private List<TareaAdicionalTrabajador> _tareasReporte;
        private Trabajador _trabajador;

        public DateTime FechaInicio
        {
            get => _fechaInicio;
            set => _fechaInicio = value;
        }

        public DateTime FechaFin
        {
            get => _fechaFin;
            set => _fechaFin = value;
        }

        public VistaReporteTareasAdicionales(int idTrabajador, int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idTrabajador = idTrabajador;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;

            // Establecer fechas por defecto (último mes)
            _fechaInicio = DateTime.Today.AddDays(-30);
            _fechaFin = DateTime.Today;

            BindingContext = this;
            CargarDatosTrabajador();
        }

        private async void CargarDatosTrabajador()
        {
            try
            {
                var trabajadores = await App.Database.ObtenerTrabajadoresAsync();
                _trabajador = trabajadores.FirstOrDefault(t => t.IdTrabajador == _idTrabajador);

                if (_trabajador == null)
                {
                    await DisplayAlert("Error", "No se pudo encontrar la información del trabajador", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar datos del trabajador: {ex.Message}", "OK");
            }
        }

        private void fechaInicioPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            _fechaInicio = e.NewDate;
        }

        private void fechaFinPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            _fechaFin = e.NewDate;
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void GenerarReporte_Clicked(object sender, EventArgs e)
        {
            if (_fechaInicio > _fechaFin)
            {
                await DisplayAlert("Error", "La fecha de inicio no puede ser posterior a la fecha de fin", "OK");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                // Obtener las tareas adicionales para el rango de fechas
                _tareasReporte = await App.Database.ObtenerTareasAdicionalesTrabajadorAsync(_idUsuario, _fechaInicio, _fechaFin);

                // Actualizar la interfaz con los datos
                MostrarDatosReporte();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo generar el reporte: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private void MostrarDatosReporte()
        {
            if (_tareasReporte == null || _tareasReporte.Count == 0 || _trabajador == null)
            {
                // No hay datos para mostrar
                OcultarPanelesReporte();

                if (_tareasReporte != null && _tareasReporte.Count == 0 && _trabajador != null)
                {
                    DisplayAlert("Información", "No hay tareas adicionales registradas en el período seleccionado", "OK");
                }

                return;
            }

            // Actualizar información del trabajador
            string nombreCompleto = $"{_trabajador.Nombre} {_trabajador.apePaterno} {_trabajador.apeMaterno}".Trim();
            lblNombreTrabajador.Text = nombreCompleto;
            lblCategoriaTrabajador.Text = _trabajador.categoria;
            lblPeriodoReporte.Text = $"{_fechaInicio:dd/MM/yyyy} - {_fechaFin:dd/MM/yyyy}";

            // Actualizar resumen
            int totalTareas = _tareasReporte.Count;
            int tareasCompletadas = _tareasReporte.Count(t => t.Estado);
            int tareasPendientes = totalTareas - tareasCompletadas;

            lblTotalTareas.Text = totalTareas.ToString();
            lblTareasCompletadas.Text = tareasCompletadas.ToString();
            lblTareasPendientes.Text = tareasPendientes.ToString();

            // Actualizar lista de tareas
            ListaTareas.ItemsSource = _tareasReporte.OrderBy(t => t.FechaTarea).ThenBy(t => t.HoraInicio).ToList();

            // Mostrar todos los paneles y botón de exportar
            FrameInfoTrabajador.IsVisible = true;
            FrameResumen.IsVisible = true;
            FrameTareas.IsVisible = true;
            BtnExportarPDF.IsVisible = true;
        }

        private void OcultarPanelesReporte()
        {
            FrameInfoTrabajador.IsVisible = false;
            FrameResumen.IsVisible = false;
            FrameTareas.IsVisible = false;
            BtnExportarPDF.IsVisible = false;
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            if (_tareasReporte == null || _tareasReporte.Count == 0 || _trabajador == null)
            {
                await DisplayAlert("Error", "No hay datos para exportar", "OK");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                string nombreCompleto = $"{_trabajador.Nombre} {_trabajador.apePaterno} {_trabajador.apeMaterno}".Trim();

                // Generar el documento PDF
                byte[] pdfBytes = await App.Database.GenerarReporteTareasAdicionalesPDF(
                    _tareasReporte,
                    nombreCompleto,
                    _fechaInicio,
                    _fechaFin);

                // Guardar y compartir el PDF
                string fileName = $"Reporte_Tareas_{_fechaInicio:yyyy-MM-dd}_a_{_fechaFin:yyyy-MM-dd}.pdf";
                string tempPath = Path.Combine(FileSystem.CacheDirectory, fileName);

               File.WriteAllBytes(tempPath, pdfBytes);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir reporte de tareas adicionales",
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
}
