// AppTransporte/Interfaces/VistaReporteTrabajosTrabajadorCliente.xaml.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using AppTransporte.model;
using Microsoft.Maui.Controls;

namespace AppTransporte.Interfaces
{
    public partial class VistaReporteTrabajosTrabajadorCliente : ContentPage
    {
        private readonly int _idCliente;
        private readonly int _idUsuario;
        private readonly int _idTipoUsuario;
        private DataTable _resumenTrabajador;
        private DataTable _detalleViajes;
        private List<dynamic> _trabajadores;
        private List<TrabajadorViajeResumen> _resumenTipado;
        private List<DetalleViajeTrabajador> _detalleTipado;

        public VistaReporteTrabajosTrabajadorCliente(int idCliente, int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;

            // Cargar la lista de trabajadores relacionados con el cliente
            CargarTrabajadores();
        }

        private async void CargarTrabajadores()
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                // Obtener los trabajadores asignados a este cliente
                DataTable trabajadoresCliente = await Task.Run(() =>
                    App.Database.GetTrabajadoresPorCliente(_idCliente));

                _trabajadores = new List<dynamic>
                {
                    new { IdTrabajador = -1, NombreCompleto = "Seleccione un trabajador" }
                };

                foreach (DataRow row in trabajadoresCliente.Rows)
                {
                    string nombreCompleto = $"{row["Nombre"]} {row["apePaterno"]} {row["apeMaterno"]}".Trim();
                    int idTrabajador = Convert.ToInt32(row["id_trabajador"]);
                    _trabajadores.Add(new { IdTrabajador = idTrabajador, NombreCompleto = nombreCompleto });
                }

                // Configurar el selector de trabajadores
                trabajadorPicker.ItemsSource = _trabajadores;
                trabajadorPicker.ItemDisplayBinding = new Binding("NombreCompleto");
                trabajadorPicker.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar la lista de trabajadores: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error en CargarTrabajadores: {ex.Message}");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void CargarReporte(int? idTrabajador = null)
        {
            try
            {
                if (idTrabajador == -1)
                    idTrabajador = null;

                LoadingOverlay.IsVisible = true;

                // Usar el método asíncrono tipado para obtener datos más robustos
                var resultadoTipado = await Task.Run(() =>
                    App.Database.ObtenerReporteTrabajadorViajesCompletoAsync(_idCliente, idTrabajador).Result);

                _resumenTipado = resultadoTipado.Resumen;
                _detalleTipado = resultadoTipado.Detalles;

                // Para compatibilidad, también obtenemos DataTables
                var resultado = await Task.Run(() =>
                    App.Database.ObtenerReporteTrabajosPorTrabajadorCliente(_idCliente, idTrabajador));

                _resumenTrabajador = resultado.ResumenTrabajador;
                _detalleViajes = resultado.DetalleViajes;

                // Actualizar vista de resumen si hay datos
                if (_resumenTipado != null && _resumenTipado.Count > 0)
                {
                    var trabajador = _resumenTipado[0];

                    // Mostrar información del trabajador seleccionado
                    NombreCompletoLabel.Text = trabajador.NombreCompleto;
                    CategoriaLabel.Text = trabajador.Categoria;
                    TotalViajesLabel.Text = trabajador.TotalViajesRealizados.ToString();
                    TotalPedidosLabel.Text = trabajador.TotalPedidosAtendidos.ToString();
                    VolumenTransportadoLabel.Text = $"{trabajador.VolumenTotalTransportado:N2} L";

                    // Mostrar panel de resumen
                    resumenPanel.IsVisible = true;
                }
                else
                {
                    resumenPanel.IsVisible = false;
                }

                // Configurar la colección de datos para los detalles de viajes
                viajesCollectionView.ItemsSource = _detalleTipado;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar el reporte: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error en CargarReporte: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void TrabajadorPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (trabajadorPicker.SelectedIndex >= 0)
            {
                dynamic selectedItem = _trabajadores[trabajadorPicker.SelectedIndex];
                int idTrabajador = selectedItem.IdTrabajador;
                CargarReporte(idTrabajador);
            }
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Función en desarrollo", "La exportación a PDF se encuentra en desarrollo y estará disponible próximamente.", "OK");
            return;
        }

        private async void Compartir_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Función en desarrollo", "La funcionalidad de compartir PDF se encuentra en desarrollo y estará disponible próximamente.", "OK");
            return;
        }
    }
}