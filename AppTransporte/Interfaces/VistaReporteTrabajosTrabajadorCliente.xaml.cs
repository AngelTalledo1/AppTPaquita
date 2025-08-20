// AppTransporte/Interfaces/VistaReporteTrabajosTrabajadorCliente.xaml.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using AppTransporte.model;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Syncfusion.Pdf.Graphics;

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
            LoadingOverlay.IsVisible = true;

            try
            {
                // Verificar que tengamos datos para exportar
                if (_resumenTipado == null || _resumenTipado.Count == 0 || 
                    _detalleTipado == null || _detalleTipado.Count == 0)
                {
                    await DisplayAlert("Sin datos", 
                        "No hay datos disponibles para exportar. Seleccione un trabajador primero.", "OK");
                    return;
                }

                // Generar el PDF usando Syncfusion
                byte[] pdfBytes = await GenerarReporteTrabajosTrabajadorClientePDF();

                // Guardar el PDF en el directorio de caché de la aplicación
                string nombreArchivo = $"ReporteTrabajos_Cliente{_idCliente}_Trabajador{_resumenTipado[0].IdTrabajador}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);

                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Mostrar mensaje de éxito con la ubicación del archivo
                await DisplayAlert("Éxito", $"Reporte exportado a PDF correctamente.\nGuardado en: {rutaArchivo}", "OK");

                // Abrir el archivo automáticamente
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(rutaArchivo)
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al exportar PDF: {ex}");
                await DisplayAlert("Error", $"Error al exportar el reporte: {ex.Message}", "OK");
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
                if (_resumenTipado == null || _resumenTipado.Count == 0 || 
                    _detalleTipado == null || _detalleTipado.Count == 0)
                {
                    await DisplayAlert("Sin datos", 
                        "No hay datos disponibles para compartir. Seleccione un trabajador primero.", "OK");
                    return;
                }

                // Generar el PDF usando Syncfusion
                byte[] pdfBytes = await GenerarReporteTrabajosTrabajadorClientePDF();

                // Guardar el PDF temporalmente
                string nombreArchivo = $"ReporteTrabajos_Cliente{_idCliente}_Trabajador{_resumenTipado[0].IdTrabajador}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);
                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Compartir el archivo usando el selector nativo de compartir
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Trabajos por Trabajador",
                    File = new ShareFile(rutaArchivo)
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al compartir PDF: {ex}");
                await DisplayAlert("Error", $"Error al compartir el reporte: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async Task<byte[]> GenerarReporteTrabajosTrabajadorClientePDF()
        {
            try
            {
                using (var document = new Syncfusion.Pdf.PdfDocument())
                {
                    // Crear página
                    var page = document.Pages.Add();
                    var graphics = page.Graphics;

                    // Configurar fuentes
                    var titleFont = new Syncfusion.Pdf.Graphics.PdfStandardFont(
                        Syncfusion.Pdf.Graphics.PdfFontFamily.Helvetica, 18, 
                        Syncfusion.Pdf.Graphics.PdfFontStyle.Bold);
                    var companyFont = new Syncfusion.Pdf.Graphics.PdfStandardFont(
                        Syncfusion.Pdf.Graphics.PdfFontFamily.Helvetica, 16, 
                        Syncfusion.Pdf.Graphics.PdfFontStyle.Bold);
                    var headerFont = new Syncfusion.Pdf.Graphics.PdfStandardFont(
                        Syncfusion.Pdf.Graphics.PdfFontFamily.Helvetica, 14, 
                        Syncfusion.Pdf.Graphics.PdfFontStyle.Bold);
                    var normalFont = new Syncfusion.Pdf.Graphics.PdfStandardFont(
                        Syncfusion.Pdf.Graphics.PdfFontFamily.Helvetica, 10);
                    var boldFont = new Syncfusion.Pdf.Graphics.PdfStandardFont(
                        Syncfusion.Pdf.Graphics.PdfFontFamily.Helvetica, 10, 
                        Syncfusion.Pdf.Graphics.PdfFontStyle.Bold);
                    var infoFont = new Syncfusion.Pdf.Graphics.PdfStandardFont(
                        Syncfusion.Pdf.Graphics.PdfFontFamily.Helvetica, 12);

                    // Colores corporativos
                    var redBrush = new Syncfusion.Pdf.Graphics.PdfSolidBrush(
                        new Syncfusion.Pdf.Graphics.PdfColor(203, 67, 53)); // #cb4335
                    var blackBrush = new Syncfusion.Pdf.Graphics.PdfSolidBrush(
                        new Syncfusion.Pdf.Graphics.PdfColor(0, 0, 0));
                    var grayBrush = new Syncfusion.Pdf.Graphics.PdfSolidBrush(
                        new Syncfusion.Pdf.Graphics.PdfColor(85, 85, 85));
                    var whiteBrush = new Syncfusion.Pdf.Graphics.PdfSolidBrush(
                        new Syncfusion.Pdf.Graphics.PdfColor(255, 255, 255));
                    var lightGrayBrush = new Syncfusion.Pdf.Graphics.PdfSolidBrush(
                        new Syncfusion.Pdf.Graphics.PdfColor(248, 249, 250));

                    float yPosition = 20;

                    // ENCABEZADO CORPORATIVO

                    // 1. LOGO (izquierda)
                    try
                    {
                        using var logoStream = await FileSystem.OpenAppPackageFileAsync("Resources/Raw/paquitaaa.png");
                        PdfBitmap logo = new PdfBitmap(logoStream);
                        graphics.DrawImage(logo, 20, yPosition, 80, 60);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        // Fallback si no se encuentra el logo
                        graphics.DrawRectangle(new Syncfusion.Pdf.Graphics.PdfPen(redBrush), 20, yPosition, 80, 60);
                        graphics.DrawString("LOGO", normalFont, redBrush, 35, yPosition + 25);
                    }

                    // 2. INFORMACIÓN DE LA EMPRESA (centro)
                    float centerX = page.Size.Width / 2;

                    // Nombre de la empresa
                    var companyNameSize = companyFont.MeasureString("TRANSPORTES PAQUITA S.R.L");
                    graphics.DrawString("TRANSPORTES PAQUITA S.R.L", companyFont, redBrush,
                        centerX - (companyNameSize.Width / 2), yPosition + 5);

                    // RUC
                    var rucSize = infoFont.MeasureString("RUC: 20102423985");
                    graphics.DrawString("RUC: 20102423985", infoFont, blackBrush,
                        centerX - (rucSize.Width / 2), yPosition + 28);

                    // Teléfono
                    var phoneSize = infoFont.MeasureString("Teléfono: 981229253");
                    graphics.DrawString("Teléfono: 981229253", infoFont, blackBrush,
                        centerX - (phoneSize.Width / 2), yPosition + 48);

                    yPosition += 80;

                    // 3. LÍNEA SEPARADORA ROJA
                    var redPen = new Syncfusion.Pdf.Graphics.PdfPen(redBrush, 2);
                    graphics.DrawLine(redPen, 20, yPosition, page.Size.Width - 20, yPosition);
                    yPosition += 25;

                    // TÍTULO DEL REPORTE
                    var titleSize = titleFont.MeasureString("REPORTE DE TRABAJOS POR TRABAJADOR");
                    graphics.DrawString("REPORTE DE TRABAJOS POR TRABAJADOR", titleFont, redBrush,
                        centerX - (titleSize.Width / 2), yPosition);
                    yPosition += 35;

                    // INFORMACIÓN DEL CLIENTE Y FECHA
                    graphics.DrawString($"Cliente ID: {_idCliente}", headerFont, blackBrush, 20, yPosition);
                    yPosition += 20;
                    graphics.DrawString($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", infoFont, blackBrush, 20, yPosition);
                    yPosition += 30;

                    // INFORMACIÓN DEL TRABAJADOR
                    var trabajador = _resumenTipado[0];
                    var grayPen = new Syncfusion.Pdf.Graphics.PdfPen(
                        new Syncfusion.Pdf.Graphics.PdfColor(200, 200, 200));

                    graphics.DrawRectangle(grayPen, lightGrayBrush, 20, yPosition, 555, 120);

                    // Título del trabajador
                    graphics.DrawString("INFORMACIÓN DEL TRABAJADOR", boldFont, blackBrush, 30, yPosition + 10);

                    // Línea separadora
                    graphics.DrawLine(grayPen, 30, yPosition + 25, 565, yPosition + 25);

                    // Información del trabajador en dos columnas
                    float col1X = 50, col2X = 300;
                    float trabajadorY = yPosition + 40;

                    // Columna 1
                    graphics.DrawString("Trabajador:", normalFont, grayBrush, col1X, trabajadorY);
                    graphics.DrawString(trabajador.NombreCompleto, boldFont, blackBrush, col1X, trabajadorY + 15);

                    graphics.DrawString("Categoría:", normalFont, grayBrush, col1X, trabajadorY + 40);
                    graphics.DrawString(trabajador.Categoria, boldFont, blackBrush, col1X, trabajadorY + 55);

                    // Columna 2
                    graphics.DrawString("Total Viajes:", normalFont, grayBrush, col2X, trabajadorY);
                    graphics.DrawString(trabajador.TotalViajesRealizados.ToString(), boldFont, blackBrush, col2X, trabajadorY + 15);

                    graphics.DrawString("Volumen Total:", normalFont, grayBrush, col2X, trabajadorY + 40);
                    graphics.DrawString($"{trabajador.VolumenTotalTransportado:N2} L", boldFont, blackBrush, col2X, trabajadorY + 55);

                    yPosition += 140;

                    // TABLA DE DETALLE DE VIAJES
                    graphics.DrawString("DETALLE DE VIAJES REALIZADOS", headerFont, blackBrush, 20, yPosition);
                    yPosition += 25;

                    // Crear tabla de detalle
                    var detalleTable = new Syncfusion.Pdf.Grid.PdfGrid();
                    detalleTable.Columns.Add(6);

                    // Configurar anchos de columna
                    detalleTable.Columns[0].Width = 70;  // ID Viaje
                    detalleTable.Columns[1].Width = 80;  // Fecha
                    detalleTable.Columns[2].Width = 70;  // Volumen
                    detalleTable.Columns[3].Width = 120; // Origen-Destino
                    detalleTable.Columns[4].Width = 100; // Tracto/Cisterna
                    detalleTable.Columns[5].Width = 95;  // Estado

                    // Estilo de encabezado
                    var headerRowStyle = new Syncfusion.Pdf.Grid.PdfGridRowStyle();
                    headerRowStyle.BackgroundBrush = redBrush;
                    headerRowStyle.TextBrush = whiteBrush;
                    headerRowStyle.Font = boldFont;

                    // Agregar fila de encabezado
                    var headerRow = detalleTable.Headers.Add(1)[0];
                    headerRow.Style = headerRowStyle;
                    headerRow.Height = 28;

                    headerRow.Cells[0].Value = "ID Viaje";
                    headerRow.Cells[1].Value = "Fecha";
                    headerRow.Cells[2].Value = "Volumen (L)";
                    headerRow.Cells[3].Value = "Ruta";
                    headerRow.Cells[4].Value = "Vehículo";
                    headerRow.Cells[5].Value = "Estado";

                    // Alineación de encabezados
                    for (int i = 0; i < headerRow.Cells.Count; i++)
                    {
                        headerRow.Cells[i].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                    }

                    // Agregar datos
                    bool colorAlternado = false;
                    foreach (var viaje in _detalleTipado)
                    {
                        var gridRow = detalleTable.Rows.Add();
                        gridRow.Height = 22;

                        // Alternar color de filas
                        if (colorAlternado)
                        {
                            gridRow.Style.BackgroundBrush = new Syncfusion.Pdf.Graphics.PdfSolidBrush(
                                new Syncfusion.Pdf.Graphics.PdfColor(245, 245, 245));
                        }
                        colorAlternado = !colorAlternado;

                        gridRow.Cells[0].Value = viaje.IdViaje.ToString();
                        gridRow.Cells[1].Value = viaje.FechaProgramada.ToString("dd/MM/yyyy");
                        gridRow.Cells[2].Value = viaje.Volumen.ToString("N0");
                        gridRow.Cells[3].Value = $"{viaje.Origen} ? {viaje.Destino}";
                        gridRow.Cells[4].Value = $"{viaje.Tracto}/{viaje.Cisterna}";
                        gridRow.Cells[5].Value = viaje.UltimoEstado;

                        // Aplicar fuente
                        for (int j = 0; j < gridRow.Cells.Count; j++)
                        {
                            gridRow.Cells[j].Style.Font = normalFont;
                            gridRow.Cells[j].Style.TextBrush = blackBrush;
                        }

                        // Alineación de celdas
                        gridRow.Cells[0].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                        gridRow.Cells[1].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                        gridRow.Cells[2].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                        gridRow.Cells[3].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Left);
                        gridRow.Cells[4].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                        gridRow.Cells[5].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                    }

                    // Dibujar la tabla
                    var result = detalleTable.Draw(page, 20, yPosition);
                    yPosition = result.Bounds.Bottom + 20;

                    // RESUMEN FINAL
                    graphics.DrawRectangle(grayPen, lightGrayBrush, 20, yPosition, 555, 60);
                    
                    graphics.DrawString("RESUMEN TOTAL", boldFont, blackBrush, 30, yPosition + 10);
                    graphics.DrawLine(grayPen, 30, yPosition + 25, 565, yPosition + 25);

                    // Datos del resumen final
                    float resumenY = yPosition + 35;
                    graphics.DrawString($"Total de Viajes: {trabajador.TotalViajesRealizados}", normalFont, blackBrush, 50, resumenY);
                    graphics.DrawString($"Total de Pedidos: {trabajador.TotalPedidosAtendidos}", normalFont, blackBrush, 200, resumenY);
                    graphics.DrawString($"Volumen Total Transportado: {trabajador.VolumenTotalTransportado:N2} L", normalFont, blackBrush, 350, resumenY);

                    yPosition += 80;

                    // PIE DE PÁGINA
                    string fechaGeneracion = $"Reporte generado por App Transporte • {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    graphics.DrawString(fechaGeneracion, normalFont, grayBrush, 20, page.Size.Height - 40);

                    // Número de página
                    string numeroPagina = "Página 1 de 1";
                    var textSize = normalFont.MeasureString(numeroPagina);
                    graphics.DrawString(numeroPagina, normalFont, grayBrush,
                        page.Size.Width - textSize.Width - 20, page.Size.Height - 40);

                    // Convertir a bytes
                    using (var stream = new MemoryStream())
                    {
                        document.Save(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al generar PDF con Syncfusion: {ex}");
                throw new Exception($"Error al generar PDF: {ex.Message}", ex);
            }
        }
    }
}