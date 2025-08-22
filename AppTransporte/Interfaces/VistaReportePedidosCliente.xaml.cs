using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class VistaReportePedidosCliente : ContentPage
    {
        private int _idCliente;
        private int _idUsuario;
        private int _idTipoUsuario;
        private DataTable _resumenPedidos;
        private DataTable _detallePedidos;
        private DateTime? _fechaDesde;
        private DateTime? _fechaHasta;
        private string _clienteNombre;

        public VistaReportePedidosCliente(int idCliente, int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;

            InicializarControles();
            _ = ObtenerNombreCliente();
            _ = CargarReporte();
        }

        private async Task ObtenerNombreCliente()
        {
            try
            {
                var clientes = await App.Database.ObtenerClientesAsync();
                var cliente = clientes.FirstOrDefault(c => c.IdCliente == _idCliente);
                _clienteNombre = cliente != null ?
                    $"{cliente.Nombre} {cliente.ApePaterno} {cliente.ApeMaterno}".Trim() :
                    "Cliente no encontrado";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener nombre del cliente: {ex.Message}");
                _clienteNombre = "Cliente";
            }
        }

        private void InicializarControles()
        {
            // Configurar fechas por defecto (últimos 30 días)
            fechaHastaPicker.Date = DateTime.Today;
            fechaDesdePicker.Date = DateTime.Today.AddDays(-30);

            // Establecer fechas iniciales
            _fechaDesde = fechaDesdePicker.Date;
            _fechaHasta = fechaHastaPicker.Date;

            ActualizarLabelPeriodo();
        }

        private async Task CargarReporte()
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                var resultado = await Task.Run(() => App.Database.ObtenerReportePedidosPorClienteAsync(
                    _idCliente,
                    null, // Ya no usamos tipo de pedido
                    _fechaDesde,
                    _fechaHasta));

                _resumenPedidos = resultado.ResumenPedidos;
                _detallePedidos = resultado.DetallePedidos;

                // Depuración: imprimir las columnas y filas
                Console.WriteLine($"Filas en resumen: {_resumenPedidos?.Rows.Count ?? 0}");
                Console.WriteLine($"Filas en detalle: {_detallePedidos?.Rows.Count ?? 0}");

                // Crear objetos anónimos con la propiedad Row para alternar colores
                var resumenItems = new List<object>();
                if (_resumenPedidos != null)
                {
                    resumenItems = _resumenPedidos.AsEnumerable()
                        .Select((row, index) => new {
                            EstadoPedido = row["EstadoPedido"]?.ToString() ?? "",
                            CantidadPedidos = Convert.ToInt32(row["CantidadPedidos"]),
                            VolumenTotal = Convert.ToInt32(row["VolumenTotal"]),
                            PromedioViajes = _resumenPedidos.Columns.Contains("PromedioViajes")
                                ? Convert.ToDouble(row["PromedioViajes"])
                                : 1.0,
                            Row = index % 2 == 0
                        }).Cast<object>().ToList();
                }

                // Procesar directamente la tabla de detalle
                var detalleItems = new List<object>();
                if (_detallePedidos != null && _detallePedidos.Rows.Count > 0)
                {
                    foreach (DataRow row in _detallePedidos.Rows)
                    {
                        detalleItems.Add(new
                        {
                            id_pedido = row["id_pedido"]?.ToString() ?? "",
                            origen = row["origen"]?.ToString() ?? "",
                            destino = row["destino"]?.ToString() ?? "",
                            origen_destino = $"{row["origen"]?.ToString() ?? ""} → {row["destino"]?.ToString() ?? ""}",
                            estado = row["estado"]?.ToString() ?? "",
                            cantidad = row["cantidad"] == DBNull.Value ? 0 : Convert.ToInt32(row["cantidad"]),
                            viajes = row.Table.Columns.Contains("viajes") && row["viajes"] != DBNull.Value ?
                                     Convert.ToInt32(row["viajes"]) : 1,
                            completado = row["completado"]?.ToString() ?? "No",
                            Row = detalleItems.Count % 2 == 0
                        });
                    }
                }

                // Configurar las colecciones de datos en el hilo principal
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    resumenCollectionView.ItemsSource = resumenItems;
                    detalleCollectionView.ItemsSource = detalleItems;

                    // Actualizar resumen
                    ActualizarResumen();
                    ActualizarLabelPeriodo();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CargarReporte: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                await DisplayAlert("Error", $"Error al cargar el reporte: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private void ActualizarLabelPeriodo()
        {
            if (_fechaDesde.HasValue && _fechaHasta.HasValue)
            {
                if (_fechaDesde.Value.Date == _fechaHasta.Value.Date)
                {
                    PeriodoLabel.Text = _fechaDesde.Value.ToString("dd/MM/yyyy");
                }
                else
                {
                    PeriodoLabel.Text = $"{_fechaDesde.Value:dd/MM/yyyy} - {_fechaHasta.Value:dd/MM/yyyy}";
                }
            }
            else
            {
                PeriodoLabel.Text = "Todos";
            }
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
            else
            {
                TotalPedidosLabel.Text = "0";
                VolumenTotalLabel.Text = "0";
            }
        }

        // Eventos de controles
        private async void Btn_atras(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void FechaDesde_DateSelected(object sender, DateChangedEventArgs e)
        {
            _fechaDesde = e.NewDate;

            // Validar que fecha desde no sea mayor que fecha hasta
            if (_fechaHasta.HasValue && _fechaDesde > _fechaHasta)
            {
                _fechaHasta = _fechaDesde;
                fechaHastaPicker.Date = _fechaDesde.Value;
            }

            _ = CargarReporte();
        }

        private void FechaHasta_DateSelected(object sender, DateChangedEventArgs e)
        {
            _fechaHasta = e.NewDate;

            // Validar que fecha hasta no sea menor que fecha desde
            if (_fechaDesde.HasValue && _fechaHasta < _fechaDesde)
            {
                _fechaDesde = _fechaHasta;
                fechaDesdePicker.Date = _fechaHasta.Value;
            }

            _ = CargarReporte();
        }

        private void LimpiarFiltros_Clicked(object sender, EventArgs e)
        {
            // Limpiar filtros de fecha
            _fechaDesde = null;
            _fechaHasta = null;

            // Resetear controles
            fechaDesdePicker.Date = DateTime.Today.AddDays(-30);
            fechaHastaPicker.Date = DateTime.Today;

            // Establecer nuevas fechas por defecto
            _fechaDesde = fechaDesdePicker.Date;
            _fechaHasta = fechaHastaPicker.Date;

            // Recargar con filtros limpiados
            _ = CargarReporte();
        }

        private async void ActualizarReporte_Clicked(object sender, EventArgs e)
        {
            await CargarReporte();
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                if (_resumenPedidos == null || _detallePedidos == null)
                {
                    await DisplayAlert("Error", "No hay datos para exportar", "OK");
                    return;
                }

                // Generar PDF usando el método del SqlServerService
                byte[] pdfBytes = await App.Database.GenerarReportePedidosClientePDF(
                    _resumenPedidos,
                    _detallePedidos,
                    _clienteNombre,
                    _fechaDesde,
                    _fechaHasta);

                // Guardar en cache para poder abrirlo
                string fileName = $"Reporte_Pedidos_Cliente_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                File.WriteAllBytes(filePath, pdfBytes);

                // Abrir el PDF usando el sistema de archivos del dispositivo
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath),
                    Title = "Abrir reporte de pedidos"
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al exportar PDF: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }
        private static string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }

        private async void Compartir_Clicked(object sender, EventArgs e)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                if (_resumenPedidos == null || _detallePedidos == null)
                {
                    await DisplayAlert("Error", "No hay datos para compartir", "OK");
                    return;
                }

                // Generar PDF
                byte[] pdfBytes = await App.Database.GenerarReportePedidosClientePDF(
                    _resumenPedidos,
                    _detallePedidos,
                    _clienteNombre,
                    _fechaDesde,
                    _fechaHasta);

                // Crear archivo temporal para compartir
                string fileName = $"Reporte_Pedidos_Cliente_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                File.WriteAllBytes(filePath, pdfBytes);

                // Compartir archivo usando Share
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Pedidos",
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al compartir: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }
    }
}