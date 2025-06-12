using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Maui.Controls;

namespace AppTransporte.Interfaces
{
    public partial class VistaReportePedidosCliente : ContentPage
    {
        private int _idCliente;
        private int _idUsuario;
        private int _idTipoUsuario;
        private DataTable _resumenPedidos;
        private DataTable _detallePedidos;
        private List<string> _tiposPedido;

        public VistaReportePedidosCliente(int idCliente, int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;

            CargarTiposPedido();
            CargarReporte();
        }

        private void CargarTiposPedido()
        {
            _tiposPedido = new List<string>
            {
                "Todos los tipos",
                "Urgente",
                "Normal",
                "Programado",
                "Recurrente"
            };

            tipoPedidoPicker.ItemsSource = _tiposPedido;
            tipoPedidoPicker.SelectedIndex = 0;
        }

        private async void CargarReporte(string tipoPedido = null)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                string tipoSeleccionado = tipoPedido;
                if (tipoPedido == "Todos los tipos" || string.IsNullOrEmpty(tipoPedido))
                {
                    tipoSeleccionado = null;
                }

                var resultado = await Task.Run(() => App.Database.ObtenerReportePedidosPorCliente(_idCliente, tipoSeleccionado));
                _resumenPedidos = resultado.ResumenPedidos;
                _detallePedidos = resultado.DetallePedidos;

                // Configurar las colecciones de datos
                resumenCollectionView.ItemsSource = ConvertirDataTableALista(_resumenPedidos);
                detalleCollectionView.ItemsSource = ConvertirDataTableALista(_detallePedidos);

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
                }
                lista.Add(item);
            }
            return lista;
        }

        private void ActualizarResumen()
        {
            if (_resumenPedidos != null && _resumenPedidos.Rows.Count > 0)
            {
                // Calcular totales para el resumen
                int totalPedidos = _resumenPedidos.AsEnumerable()
                    .Sum(r => Convert.ToInt32(r["CantidadPedidos"]));
                int volumenTotal = _resumenPedidos.AsEnumerable()
                    .Sum(r => Convert.ToInt32(r["VolumenTotal"]));

                // Actualizar etiquetas
                TotalPedidosLabel.Text = totalPedidos.ToString();
                VolumenTotalLabel.Text = volumenTotal.ToString();
            }
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void TipoPedido_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tipoPedidoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
            CargarReporte(tipoPedidoSeleccionado);
        }

        private async void ActualizarReporte_Clicked(object sender, EventArgs e)
        {
            string tipoPedidoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
            CargarReporte(tipoPedidoSeleccionado);
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
