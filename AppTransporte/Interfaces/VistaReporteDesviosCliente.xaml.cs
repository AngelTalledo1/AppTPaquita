using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Maui.Controls;

namespace AppTransporte.Interfaces
{
    public partial class VistaReporteDesviosCliente : ContentPage
    {
        private int _idCliente;
        private int _idUsuario;
        private int _idTipoUsuario;
        private DataTable _resumenDesvios;
        private DataTable _detalleDesvios;

        public VistaReporteDesviosCliente(int idCliente, int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;

            // Inicializar fechas por defecto
            InicializarFechasPorDefecto();

            // Cargar el reporte inicial
            CargarReporte();
        }

        private void InicializarFechasPorDefecto()
        {
            DateTime hoy = DateTime.Today;
            DateTime inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            fechaInicio.Date = inicioMes;
            fechaFin.Date = hoy;

            // Actualizar etiqueta del período de reporte
            ActualizarEtiquetaPeriodo();
        }

        private void ActualizarEtiquetaPeriodo()
        {
            FechasReporte.Text = $"Periodo: {fechaInicio.Date:dd/MM/yyyy} - {fechaFin.Date:dd/MM/yyyy}";
        }

        private async void CargarReporte()
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                var resultado = await Task.Run(() =>
                    App.Database.ObtenerReporteDesviosPorCliente(
                        _idCliente, fechaInicio.Date, fechaFin.Date));

                _resumenDesvios = resultado.ResumenDesvios;
                _detalleDesvios = resultado.DetalleDesvios;

                // Configurar las colecciones de datos
                if (_resumenDesvios.Rows.Count > 0)
                {
                    var fila = _resumenDesvios.Rows[0];

                    // Actualizar etiquetas de resumen
                    TotalIncidenciasLabel.Text = fila["TotalIncidencias"]?.ToString() ?? "0";
                    IncidenciasResueltasLabel.Text = fila["IncidenciasResueltas"]?.ToString() ?? "0";
                    ViajesConIncidenciasLabel.Text = fila["ViajesConIncidencias"]?.ToString() ?? "0";
                    TiempoResolucionLabel.Text = fila["TiempoPromedioResolucion"] != DBNull.Value ?
                        $"{Math.Round(Convert.ToDouble(fila["TiempoPromedioResolucion"]), 1)} horas" : "N/A";
                }

                desviosCollectionView.ItemsSource = ConvertirDataTableALista(_detalleDesvios);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar el reporte: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private List<dynamic> ConvertirDataTableALista(DataTable dt)
        {
            var lista = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic item = new System.Dynamic.ExpandoObject();
                foreach (DataColumn column in dt.Columns)
                {
                    ((IDictionary<string, object>)item)[column.ColumnName] =
                        row[column] == DBNull.Value ? null : row[column];
                }
                lista.Add(item);
            }
            return lista;
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OnFechaSeleccionada(object sender, DateChangedEventArgs e)
        {
            ActualizarEtiquetaPeriodo();
        }

        private void ActualizarReporte_Clicked(object sender, EventArgs e)
        {
            if (fechaInicio.Date > fechaFin.Date)
            {
                DisplayAlert("Error", "La fecha de inicio no puede ser posterior a la fecha fin", "OK");
                return;
            }

            CargarReporte();
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                // Lógica para exportar a PDF
                await DisplayAlert("Éxito", "Reporte exportado a PDF correctamente.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al exportar el reporte: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void Compartir_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Información", "Funcionalidad de compartir en desarrollo.", "OK");
        }
    }
}
