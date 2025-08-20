using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Syncfusion.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppTransporte.Interfaces
{
    public partial class VistaReporteTrabajadoresCliente : ContentPage
    {
        private int _idCliente;
        private int _idUsuario;
        private int _idTipoUsuario;
        private DataTable _datosReporte;

        public VistaReporteTrabajadoresCliente(int idCliente, int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;

            // Cargar datos al aparecer la página
            CargarReporte();
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                // Verificar que tengamos datos para exportar
                if (_datosReporte == null || _datosReporte.Rows.Count == 0)
                {
                    await DisplayAlert("Sin datos", 
                        "No hay datos disponibles para exportar. Actualice el reporte primero.", "OK");
                    return;
                }

                // Generar el PDF usando Syncfusion
                byte[] pdfBytes = await GenerarReporteTrabajadoresClientePDF();

                // Guardar el PDF en el directorio de caché de la aplicación
                string nombreArchivo = $"TrabajadoresCliente_{_idCliente}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
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
                if (_datosReporte == null || _datosReporte.Rows.Count == 0)
                {
                    await DisplayAlert("Sin datos", 
                        "No hay datos disponibles para compartir. Actualice el reporte primero.", "OK");
                    return;
                }

                // Generar el PDF usando Syncfusion
                byte[] pdfBytes = await GenerarReporteTrabajadoresClientePDF();

                // Guardar el PDF temporalmente
                string nombreArchivo = $"TrabajadoresCliente_{_idCliente}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);
                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Compartir el archivo usando el selector nativo de compartir
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Trabajadores",
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

        private async Task<byte[]> GenerarReporteTrabajadoresClientePDF()
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

                    float yPosition = 20;

                    // ENCABEZADO CORPORATIVO

                    // 1. LOGO (izquierda)
                    try
                    {
                        using var logoStream = await FileSystem.OpenAppPackageFileAsync("Resources/Raw/paquitaaa.png");
                        PdfBitmap logo = new PdfBitmap(logoStream); // 
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
                    var titleSize = titleFont.MeasureString("REPORTE DE TRABAJADORES ASIGNADOS");
                    graphics.DrawString("REPORTE DE TRABAJADORES ASIGNADOS", titleFont, redBrush,
                        centerX - (titleSize.Width / 2), yPosition);
                    yPosition += 35;

                    // INFORMACIÓN DEL CLIENTE Y FECHA
                    graphics.DrawString($"Cliente ID: {_idCliente}", headerFont, blackBrush, 20, yPosition);
                    yPosition += 20;
                    graphics.DrawString($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", infoFont, blackBrush, 20, yPosition);
                    yPosition += 30;

                    // RESUMEN DEL REPORTE
                    var grayPen = new Syncfusion.Pdf.Graphics.PdfPen(
                        new Syncfusion.Pdf.Graphics.PdfColor(200, 200, 200));
                    var lightGrayBrush = new Syncfusion.Pdf.Graphics.PdfSolidBrush(
                        new Syncfusion.Pdf.Graphics.PdfColor(248, 249, 250));

                    graphics.DrawRectangle(grayPen, lightGrayBrush, 20, yPosition, 555, 100);

                    // Título del resumen
                    graphics.DrawString("RESUMEN", boldFont, blackBrush, 30, yPosition + 10);

                    // Línea separadora
                    graphics.DrawLine(grayPen, 30, yPosition + 25, 565, yPosition + 25);

                    // Calcular totales
                    int totalTrabajadores = _datosReporte.Rows.Count;
                    int totalViajes = _datosReporte.AsEnumerable().Sum(r => Convert.ToInt32(r["total_viajes"]));
                    int totalSeguimientos = _datosReporte.AsEnumerable().Sum(r => Convert.ToInt32(r["total_seguimientos"]));
                    int volumenTotal = _datosReporte.AsEnumerable().Sum(r => Convert.ToInt32(r["volumen_transportado"]));

                    // Datos del resumen en columnas
                    float col1X = 50, col2X = 200, col3X = 350, col4X = 500;
                    float resumenY = yPosition + 40;

                    // Columna 1: Total trabajadores
                    graphics.DrawString("Total trabajadores:", normalFont, grayBrush, col1X, resumenY);
                    graphics.DrawString(totalTrabajadores.ToString(), boldFont, blackBrush, col1X, resumenY + 15);

                    // Columna 2: Total viajes
                    graphics.DrawString("Total viajes:", normalFont, grayBrush, col2X, resumenY);
                    graphics.DrawString(totalViajes.ToString(), boldFont, blackBrush, col2X, resumenY + 15);

                    // Columna 3: Total seguimientos
                    graphics.DrawString("Total seguimientos:", normalFont, grayBrush, col3X, resumenY);
                    graphics.DrawString(totalSeguimientos.ToString(), boldFont, blackBrush, col3X, resumenY + 15);

                    // Columna 4: Volumen total
                    graphics.DrawString("Volumen total:", normalFont, grayBrush, col4X, resumenY);
                    graphics.DrawString($"{volumenTotal} L", boldFont, blackBrush, col4X, resumenY + 15);

                    yPosition += 120;

                    // TABLA DE DETALLE DE TRABAJADORES
                    graphics.DrawString("DETALLE DE TRABAJADORES", headerFont, blackBrush, 20, yPosition);
                    yPosition += 25;

                    // Crear tabla de detalle
                    var detalleTable = new Syncfusion.Pdf.Grid.PdfGrid();
                    detalleTable.Columns.Add(5);

                    // Configurar anchos de columna
                    detalleTable.Columns[0].Width = 140; // Nombre
                    detalleTable.Columns[1].Width = 100; // Categoría
                    detalleTable.Columns[2].Width = 70;  // Viajes
                    detalleTable.Columns[3].Width = 90;  // Seguimientos
                    detalleTable.Columns[4].Width = 95;  // Volumen

                    // Estilo de encabezado
                    var headerRowStyle = new Syncfusion.Pdf.Grid.PdfGridRowStyle();
                    headerRowStyle.BackgroundBrush = redBrush;
                    headerRowStyle.TextBrush = whiteBrush;
                    headerRowStyle.Font = boldFont;

                    // Agregar fila de encabezado
                    var headerRow = detalleTable.Headers.Add(1)[0];
                    headerRow.Style = headerRowStyle;
                    headerRow.Height = 28;

                    headerRow.Cells[0].Value = "Nombre Completo";
                    headerRow.Cells[1].Value = "Categoría";
                    headerRow.Cells[2].Value = "Viajes";
                    headerRow.Cells[3].Value = "Seguimientos";
                    headerRow.Cells[4].Value = "Volumen (L)";

                    // Alineación de encabezados
                    headerRow.Cells[0].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                        Syncfusion.Pdf.Graphics.PdfTextAlignment.Left);
                    headerRow.Cells[1].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                        Syncfusion.Pdf.Graphics.PdfTextAlignment.Left);
                    headerRow.Cells[2].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                        Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                    headerRow.Cells[3].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                        Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                    headerRow.Cells[4].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                        Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);

                    // Agregar datos
                    bool colorAlternado = false;
                    foreach (System.Data.DataRow row in _datosReporte.Rows)
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

                        string nombreCompleto = $"{row["Nombre"]} {row["apePaterno"]} {row["apeMaterno"]}".Trim();
                        string categoria = row["categoria_desc"]?.ToString() ?? "";
                        int viajes = Convert.ToInt32(row["total_viajes"]);
                        int seguimientos = Convert.ToInt32(row["total_seguimientos"]);
                        int volumen = Convert.ToInt32(row["volumen_transportado"]);

                        gridRow.Cells[0].Value = nombreCompleto;
                        gridRow.Cells[1].Value = categoria;
                        gridRow.Cells[2].Value = viajes.ToString();
                        gridRow.Cells[3].Value = seguimientos.ToString();
                        gridRow.Cells[4].Value = volumen.ToString();

                        // Aplicar fuente y alineación
                        for (int j = 0; j < gridRow.Cells.Count; j++)
                        {
                            gridRow.Cells[j].Style.Font = normalFont;
                            gridRow.Cells[j].Style.TextBrush = blackBrush;
                        }

                        // Alineación de celdas
                        gridRow.Cells[0].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Left);
                        gridRow.Cells[1].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Left);
                        gridRow.Cells[2].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                        gridRow.Cells[3].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                        gridRow.Cells[4].Style.StringFormat = new Syncfusion.Pdf.Graphics.PdfStringFormat(
                            Syncfusion.Pdf.Graphics.PdfTextAlignment.Center);
                    }

                    // Dibujar la tabla
                    var result = detalleTable.Draw(page, 20, yPosition);
                    yPosition = result.Bounds.Bottom + 20;

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

        private async void CargarReporte()
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                _datosReporte = await Task.Run(() => App.Database.ObtenerReporteTrabajadoresPorCliente(_idCliente));

                // Depuración: Imprimir nombres de columnas para verificar
                foreach (DataColumn column in _datosReporte.Columns)
                {
                    System.Diagnostics.Debug.WriteLine($"Columna encontrada: {column.ColumnName}");
                }

                // Crear una lista de objetos anónimos con las propiedades correctamente nombradas
                var items = _datosReporte.AsEnumerable().Select((row, index) => new
                {
                    Nombre = $"{row["Nombre"]} {row["apePaterno"]} {row["apeMaterno"]}".Trim(),
                    categoria_desc = row["categoria_desc"]?.ToString(),
                    total_viajes = Convert.ToInt32(row["total_viajes"]),
                    total_seguimientos = Convert.ToInt32(row["total_seguimientos"]),
                    volumen_transportado = Convert.ToInt32(row["volumen_transportado"]),
                    Row = index % 2 == 0 // Alternando colores para filas pares/impares
                }).ToList();

                // Asignar la fuente de datos a la CollectionView
                trabajadoresCollectionView.ItemsSource = items;

                // Actualizar etiquetas de resumen
                ActualizarResumen();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error completo: {ex}");
                await DisplayAlert("Error", $"Error al cargar el reporte: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private void ActualizarResumen()
        {
            if (_datosReporte != null && _datosReporte.Rows.Count > 0)
            {
                int totalTrabajadores = _datosReporte.Rows.Count;
                int totalViajes = _datosReporte.AsEnumerable()
                    .Sum(r => Convert.ToInt32(r["total_viajes"]));
                int totalSeguimientos = _datosReporte.AsEnumerable()
                    .Sum(r => Convert.ToInt32(r["total_seguimientos"]));

                // Actualizar las etiquetas en la interfaz
                TotalTrabajadoresLabel.Text = totalTrabajadores.ToString();
                TotalViajesLabel.Text = totalViajes.ToString();
                TotalSeguimientosLabel.Text = totalSeguimientos.ToString();
            }
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}
