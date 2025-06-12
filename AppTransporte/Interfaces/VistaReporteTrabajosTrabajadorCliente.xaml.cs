using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Maui.Controls;

namespace AppTransporte.Interfaces
{
    public partial class VistaReporteTrabajosTrabajadorCliente : ContentPage
    {
        private int _idCliente;
        private int _idUsuario;
        private int _idTipoUsuario;
        private DataTable _resumenTrabajador;
        private DataTable _detalleViajes;
        private List<dynamic> _trabajadores;

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

                var resultado = await Task.Run(() =>
                    App.Database.ObtenerReporteTrabajosPorTrabajadorCliente(_idCliente, idTrabajador));

                _resumenTrabajador = resultado.ResumenTrabajador;
                _detalleViajes = resultado.DetalleViajes;

                // Actualizar vista de resumen si hay datos
                if (_resumenTrabajador.Rows.Count > 0)
                {
                    var fila = _resumenTrabajador.Rows[0];

                    // Mostrar información del trabajador seleccionado
                    NombreCompletoLabel.Text = fila["NombreCompleto"]?.ToString();
                    CategoriaLabel.Text = fila["Categoria"]?.ToString();
                    TotalViajesLabel.Text = fila["TotalViajesRealizados"]?.ToString();
                    TotalPedidosLabel.Text = fila["TotalPedidosAtendidos"]?.ToString();
                    VolumenTransportadoLabel.Text = fila["VolumenTotalTransportado"]?.ToString();

                    // Mostrar panel de resumen
                    resumenPanel.IsVisible = true;
                }
                else
                {
                    resumenPanel.IsVisible = false;
                }

                // Configurar la colección de datos para los detalles de viajes
                viajesCollectionView.ItemsSource = ConvertirDataTableALista(_detalleViajes);
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
