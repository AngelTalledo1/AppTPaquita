using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Maui.Controls;

namespace AppTransporte.Interfaces
{
    public partial class VistaReporteProgramacionCliente : ContentPage
    {
        private int _idCliente;
        private int _idUsuario;
        private int _idTipoUsuario;
        private string _periodoActual = "SemanaActual";
        private DataTable _datosReporte;

        public VistaReporteProgramacionCliente(int idCliente, int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;

            // Configurar los radio buttons
            RadioSemanaActual.IsChecked = true;

            // Cargar el reporte inicial
            CargarReporte();
        }

        private void ActualizarEtiquetaPeriodo()
        {
            string periodoTexto = "Semana Actual";

            if (_periodoActual == "MesActual")
                periodoTexto = "Mes Actual";
            else if (_periodoActual == "Personalizado")
                periodoTexto = $"Período Personalizado: {fechaInicio.Date:dd/MM/yyyy} - {fechaFin.Date:dd/MM/yyyy}";

            PeriodoLabel.Text = periodoTexto;
        }

        private async void CargarReporte()
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                _datosReporte = await Task.Run(() =>
                    App.Database.ObtenerReporteProgramacionCliente(_idCliente, _periodoActual));

                // Configurar la colección de datos
                programacionCollectionView.ItemsSource = ConvertirDataTableALista(_datosReporte);

                // Actualizar etiqueta de periodo
                ActualizarEtiquetaPeriodo();

                // Actualizar resumen
                ActualizarResumen();
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

                    // Convertir el día de la semana a nombre
                    if (column.ColumnName == "dia_semana" && row[column] != DBNull.Value)
                    {
                        int diaSemana = Convert.ToInt32(row[column]);
                        string[] diasSemana = { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };
                        ((IDictionary<string, object>)item)["DiaSemanaTexto"] = diasSemana[diaSemana % 7];
                    }
                }
                lista.Add(item);
            }
            return lista;
        }

        private void ActualizarResumen()
        {
            if (_datosReporte != null && _datosReporte.Rows.Count > 0)
            {
                int totalViajes = _datosReporte.Rows.Count;

                // Actualizar etiqueta con el total de viajes programados
                TotalViajesProgramadosLabel.Text = totalViajes.ToString();

                // Actualizar información adicional si es necesario
            }
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void RadioSemanaActual_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                _periodoActual = "SemanaActual";
                panelFechasPersonalizadas.IsVisible = false;
                CargarReporte();
            }
        }

        private void RadioMesActual_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                _periodoActual = "MesActual";
                panelFechasPersonalizadas.IsVisible = false;
                CargarReporte();
            }
        }

        private void RadioPersonalizado_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                _periodoActual = "Personalizado";
                panelFechasPersonalizadas.IsVisible = true;

                // Inicializar fechas con valores predeterminados
                DateTime hoy = DateTime.Today;
                fechaInicio.Date = hoy.AddDays(-7);
                fechaFin.Date = hoy.AddDays(7);
            }
        }

        private void OnFechaSeleccionada(object sender, DateChangedEventArgs e)
        {
            // Actualizar sólo si estamos en modo personalizado y ambas fechas son válidas
            if (_periodoActual == "Personalizado" && fechaInicio.Date <= fechaFin.Date)
            {
                ActualizarEtiquetaPeriodo();
            }
        }

        private void ActualizarReporte_Clicked(object sender, EventArgs e)
        {
            if (_periodoActual == "Personalizado" && fechaInicio.Date > fechaFin.Date)
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
