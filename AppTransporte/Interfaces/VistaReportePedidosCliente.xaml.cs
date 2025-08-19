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
        private List<string> _tiposPedido;
        private DateTime? _fechaDesde;
        private DateTime? _fechaHasta;
        private string _empresaRUC = "";
        private string _empresaTelefono = "";
        private string _empresaNombre = "";
        private byte[] _empresaLogo = null;

        public VistaReportePedidosCliente(int idCliente, int idUsuario, int idTipoUsuario)
        {
            // Registrar licencia QuestPDF - COMENTADO TEMPORALMENTE
            //QuestPDF.Settings.License = LicenseType.Community;

            InitializeComponent();
            _idCliente = idCliente;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;
            CargarLogoEmpresa();
            InicializarControles();
            CargarTiposPedido();
            CargarDatosEmpresa();
            _ = CargarReporte();
        }

        private void CargarDatosEmpresa()
        {
            try
            {
                _empresaRUC = "20102423985";
                _empresaTelefono = "981 229 253";
                _empresaNombre = "TRANSPORTES PAQUITA S.R.L.";
            }
            catch (Exception ex)
            {
                // Usar valores por defecto
                _empresaRUC = "20102423985";
                _empresaTelefono = "981 229 253";
                _empresaNombre = "TRANSPORTES PAQUITA S.R.L.";
            }
        }

        private async void CargarLogoEmpresa()
        {
            try
            {
                var stream = await FileSystem.OpenAppPackageFileAsync("paquitaaa.png");
                if (stream != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        _empresaLogo = memoryStream.ToArray();
                    }
                }
            }
            catch
            {
                _empresaLogo = null;
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

        private async Task CargarReporte(string tipoPedido = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                string tipoSeleccionado = tipoPedido;
                if (tipoPedido == "Todos los tipos" || string.IsNullOrEmpty(tipoPedido))
                {
                    tipoSeleccionado = null;
                }

                // Usar las fechas proporcionadas o las del control
                DateTime? fechaDesdeParam = fechaDesde ?? _fechaDesde;
                DateTime? fechaHastaParam = fechaHasta ?? _fechaHasta;

                var resultado = await Task.Run(() => App.Database.ObtenerReportePedidosPorClienteAsync(
                    _idCliente,
                    tipoSeleccionado,
                    fechaDesdeParam,
                    fechaHastaParam));

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
                                : 1.0, // Valor predeterminado si no existe la columna
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

        private void TipoPedido_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tipoPedidoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
            _ = CargarReporte(tipoPedidoSeleccionado, _fechaDesde, _fechaHasta);
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

            string tipoPedidoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
            _ = CargarReporte(tipoPedidoSeleccionado, _fechaDesde, _fechaHasta);
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

            string tipoPedidoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
            _ = CargarReporte(tipoPedidoSeleccionado, _fechaDesde, _fechaHasta);
        }

        private void LimpiarFiltros_Clicked(object sender, EventArgs e)
        {
            // Limpiar filtros de fecha
            _fechaDesde = null;
            _fechaHasta = null;

            // Resetear controles
            fechaDesdePicker.Date = DateTime.Today.AddDays(-30);
            fechaHastaPicker.Date = DateTime.Today;
            tipoPedidoPicker.SelectedIndex = 0;

            // Establecer nuevas fechas por defecto
            _fechaDesde = fechaDesdePicker.Date;
            _fechaHasta = fechaHastaPicker.Date;

            // Recargar con filtros limpiados
            _ = CargarReporte("Todos los tipos", _fechaDesde, _fechaHasta);
        }

        private async void ActualizarReporte_Clicked(object sender, EventArgs e)
        {
            string tipoPedidoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
            await CargarReporte(tipoPedidoSeleccionado, _fechaDesde, _fechaHasta);
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            // Funcionalidad temporalmente suspendida
            await DisplayAlert("Función en desarrollo", "La exportación a PDF se encuentra en desarrollo y estará disponible próximamente.", "OK");
            return;
        }

        private async void Compartir_Clicked(object sender, EventArgs e)
        {
            // Funcionalidad temporalmente suspendida
            await DisplayAlert("Función en desarrollo", "La funcionalidad de compartir PDF se encuentra en desarrollo y estará disponible próximamente.", "OK");
            return;
        }
    }
}