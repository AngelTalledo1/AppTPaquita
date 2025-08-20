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
            LoadingOverlay.IsVisible = true;

            try
            {
                // Verificar que tengamos datos para exportar
                if (_resumenPedidos == null || _detallePedidos == null ||
                    _resumenPedidos.Rows.Count == 0)
                {
                    await DisplayAlert("Sin datos",
                        "No hay datos disponibles para exportar. Actualice el reporte primero.", "OK");
                    return;
                }

                // Mostrar opciones al usuario
                string action = await DisplayActionSheet(
                    "¿Qué desea hacer con el PDF?",
                    "Cancelar",
                    null,
                    "Guardar y abrir",
                    "Solo guardar",
                    "Compartir");

                if (action == "Cancelar")
                    return;

                // Obtener información del cliente
                string clienteNombre = await ObtenerNombreClienteAsync(_idCliente);

                // Obtener filtros aplicados
                string tipoPedidoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
                if (tipoPedidoSeleccionado == "Todos los tipos")
                    tipoPedidoSeleccionado = null;

                // Generar el PDF
                byte[] pdfBytes = await Task.Run(async () =>
                    await App.Database.GenerarReportePedidosClientePDF(
                        _resumenPedidos,
                        _detallePedidos,
                        clienteNombre,
                        tipoPedidoSeleccionado,
                        _fechaDesde,
                        _fechaHasta));

                // Crear el nombre del archivo
                string fileName = $"Reporte_Pedidos_Cliente_{_idCliente}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                switch (action)
                {
                    case "Guardar y abrir":
                        await GuardarYAbrirPDFAsync(pdfBytes, fileName);
                        await DisplayAlert("Éxito", "El reporte PDF se ha generado y abierto correctamente.", "OK");
                        break;

                    case "Solo guardar":
                        await GuardarEnCarpetaDescargas(pdfBytes, fileName);
                        await DisplayAlert("Éxito", "El reporte PDF se ha guardado en la carpeta de descargas.", "OK");
                        break;

                    case "Compartir":
                        await CompartirPDFAsync(pdfBytes, fileName);
                        break;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al procesar el PDF: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error completo: {ex}");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void Compartir_Clicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                // Verificar que tengamos datos para compartir
                if (_resumenPedidos == null || _detallePedidos == null ||
                    _resumenPedidos.Rows.Count == 0)
                {
                    await DisplayAlert("Sin datos",
                        "No hay datos disponibles para compartir. Actualice el reporte primero.", "OK");
                    return;
                }

                // Obtener información del cliente
                string clienteNombre = await ObtenerNombreClienteAsync(_idCliente);

                // Obtener filtros aplicados
                string tipoPedidoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
                if (tipoPedidoSeleccionado == "Todos los tipos")
                    tipoPedidoSeleccionado = null;

                // Generar el PDF
                byte[] pdfBytes = await Task.Run(async () =>
                    await App.Database.GenerarReportePedidosClientePDF(
                        _resumenPedidos,
                        _detallePedidos,
                        clienteNombre,
                        tipoPedidoSeleccionado,
                        _fechaDesde,
                        _fechaHasta));

                // Crear el nombre del archivo
                string fileName = $"Reporte_Pedidos_Cliente_{_idCliente}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                // Compartir el archivo
                await CompartirPDFAsync(pdfBytes, fileName);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al compartir el PDF: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al compartir PDF: {ex.Message}");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        // AGREGAR estos métodos auxiliares a la clase VistaReportePedidosCliente:

        private async Task<string> ObtenerNombreClienteAsync(int idCliente)
        {
            try
            {
                // Intentar obtener el nombre del cliente desde la base de datos
                var clientes = await App.Database.ObtenerClientesAsync();
                var cliente = clientes.FirstOrDefault(c => c.IdCliente == idCliente);

                if (cliente != null)
                {
                    return $"{cliente.Nombre} {cliente.ApePaterno} {cliente.ApeMaterno}".Trim();
                }

                return $"Cliente ID: {idCliente}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener nombre del cliente: {ex.Message}");
                return $"Cliente ID: {idCliente}";
            }
        }

        private async Task GuardarYAbrirPDFAsync(byte[] pdfBytes, string fileName)
        {
            try
            {
                // Crear un archivo temporal
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                // Escribir los bytes al archivo
                await File.WriteAllBytesAsync(filePath, pdfBytes);

                // Abrir el archivo con la aplicación predeterminada
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath),
                    Title = "Abrir Reporte PDF"
                });
            }
            catch (Exception ex)
            {
                // Si no se puede abrir, al menos guardarlo en Downloads
                await GuardarEnCarpetaDescargas(pdfBytes, fileName);
                throw new Exception($"PDF guardado pero no se pudo abrir automáticamente: {ex.Message}");
            }
        }

        private async Task GuardarEnCarpetaDescargas(byte[] pdfBytes, string fileName)
        {
            try
            {
#if ANDROID
                // Android: Guardar en la carpeta Downloads
                var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryDownloads)?.AbsolutePath;

                if (!string.IsNullOrEmpty(downloadsPath))
                {
                    string filePath = Path.Combine(downloadsPath, fileName);
                    await File.WriteAllBytesAsync(filePath, pdfBytes);

                    // Notificar al sistema que se agregó un archivo
                    var mediaScanIntent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
                    mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(filePath)));
                    Platform.CurrentActivity?.SendBroadcast(mediaScanIntent);
                }
#elif IOS
                // iOS: Guardar en Documents
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(documentsPath, fileName);
                await File.WriteAllBytesAsync(filePath, pdfBytes);
#else
        // Otras plataformas: usar carpeta de documentos
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string filePath = Path.Combine(documentsPath, fileName);
        await File.WriteAllBytesAsync(filePath, pdfBytes);
#endif
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar en carpeta de descargas: {ex.Message}");
                throw;
            }
        }

        private async Task CompartirPDFAsync(byte[] pdfBytes, string fileName)
        {
            try
            {
                // Crear un archivo temporal
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                // Escribir los bytes al archivo
                await File.WriteAllBytesAsync(filePath, pdfBytes);

                // Compartir el archivo
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Pedidos por Cliente",
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al compartir el archivo: {ex.Message}");
            }
        }
    }
}