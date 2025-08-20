using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Syncfusion.Pdf.Graphics;

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
                // Verificar que tengamos datos para exportar
                if (_datosReporte == null || _datosReporte.Rows.Count == 0)
                {
                    await DisplayAlert("Sin datos", 
                        "No hay datos disponibles para exportar. Actualice el reporte primero.", "OK");
                    return;
                }

                // Generar el PDF usando Syncfusion
                byte[] pdfBytes = await GenerarReporteProgramacionClientePDF();

                // Guardar el PDF en el directorio de caché de la aplicación
                string nombreArchivo = $"ReporteProgramacion_Cliente{_idCliente}_{_periodoActual}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
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
                byte[] pdfBytes = await GenerarReporteProgramacionClientePDF();

                // Guardar el PDF temporalmente
                string nombreArchivo = $"ReporteProgramacion_Cliente{_idCliente}_{_periodoActual}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);
                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Compartir el archivo usando el selector nativo de compartir
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Programación",
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

        private async Task<byte[]> GenerarReporteProgramacionClientePDF()
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
                    var titleSize = titleFont.MeasureString("REPORTE DE PROGRAMACIÓN DE VIAJES");
                    graphics.DrawString("REPORTE DE PROGRAMACIÓN DE VIAJES", titleFont, redBrush,
                        centerX - (titleSize.Width / 2), yPosition);
                    yPosition += 35;

                    // INFORMACIÓN DEL CLIENTE Y FECHA
                    graphics.DrawString($"Cliente ID: {_idCliente}", headerFont, blackBrush, 20, yPosition);
                    yPosition += 20;
                    graphics.DrawString($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", infoFont, blackBrush, 20, yPosition);
                    yPosition += 20;

                    // Período del reporte
                    string periodoTexto = ObtenerTextoPerido();
                    graphics.DrawString($"Período: {periodoTexto}", infoFont, blackBrush, 20, yPosition);
                    yPosition += 30;

                    // RESUMEN DEL REPORTE
                    var grayPen = new Syncfusion.Pdf.Graphics.PdfPen(
                        new Syncfusion.Pdf.Graphics.PdfColor(200, 200, 200));

                    graphics.DrawRectangle(grayPen, lightGrayBrush, 20, yPosition, 555, 80);

                    // Título del resumen
                    graphics.DrawString("RESUMEN", boldFont, blackBrush, 30, yPosition + 10);

                    // Línea separadora
                    graphics.DrawLine(grayPen, 30, yPosition + 25, 565, yPosition + 25);

                    // Calcular totales
                    int totalViajes = _datosReporte.Rows.Count;
                    var viajesConVolumen = _datosReporte.AsEnumerable()
                        .Where(r => r["volumen"] != DBNull.Value);
                    decimal volumenTotal = viajesConVolumen.Any() ? 
                        viajesConVolumen.Sum(r => Convert.ToDecimal(r["volumen"])) : 0;

                    // Datos del resumen en columnas
                    float col1X = 50, col2X = 250, col3X = 450;
                    float resumenY = yPosition + 40;

                    // Columna 1: Total viajes programados
                    graphics.DrawString("Viajes Programados:", normalFont, grayBrush, col1X, resumenY);
                    graphics.DrawString(totalViajes.ToString(), boldFont, blackBrush, col1X, resumenY + 15);

                    // Columna 2: Período
                    graphics.DrawString("Período:", normalFont, grayBrush, col2X, resumenY);
                    graphics.DrawString(periodoTexto, boldFont, blackBrush, col2X, resumenY + 15);

                    // Columna 3: Volumen total
                    graphics.DrawString("Volumen Total:", normalFont, grayBrush, col3X, resumenY);
                    graphics.DrawString($"{volumenTotal:N2} L", boldFont, blackBrush, col3X, resumenY + 15);

                    yPosition += 100;

                    // TABLA DE DETALLE DE PROGRAMACIÓN
                    graphics.DrawString("DETALLE DE PROGRAMACIÓN", headerFont, blackBrush, 20, yPosition);
                    yPosition += 25;

                    // Crear tabla de detalle
                    var detalleTable = new Syncfusion.Pdf.Grid.PdfGrid();
                    detalleTable.Columns.Add(6);

                    // Configurar anchos de columna
                    detalleTable.Columns[0].Width = 80;  // Fecha Programada
                    detalleTable.Columns[1].Width = 90;  // Día Semana
                    detalleTable.Columns[2].Width = 70;  // Volumen
                    detalleTable.Columns[3].Width = 130; // Origen-Destino
                    detalleTable.Columns[4].Width = 90;  // Vehículo
                    detalleTable.Columns[5].Width = 75;  // Estado

                    // Estilo de encabezado
                    var headerRowStyle = new Syncfusion.Pdf.Grid.PdfGridRowStyle();
                    headerRowStyle.BackgroundBrush = redBrush;
                    headerRowStyle.TextBrush = whiteBrush;
                    headerRowStyle.Font = boldFont;

                    // Agregar fila de encabezado
                    var headerRow = detalleTable.Headers.Add(1)[0];
                    headerRow.Style = headerRowStyle;
                    headerRow.Height = 28;

                    headerRow.Cells[0].Value = "Fecha";
                    headerRow.Cells[1].Value = "Día";
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
                    foreach (DataRow row in _datosReporte.Rows)
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

                        // Extraer y formatear datos
                        DateTime fechaProgramada = row["fecha_programada"] != DBNull.Value ? 
                            Convert.ToDateTime(row["fecha_programada"]) : DateTime.MinValue;
                        string diaSemana = row["DiaSemanaTexto"]?.ToString() ?? "";
                        decimal volumen = row["volumen"] != DBNull.Value ? 
                            Convert.ToDecimal(row["volumen"]) : 0;
                        string origen = row["origen"]?.ToString() ?? "";
                        string destino = row["destino"]?.ToString() ?? "";
                        string vehiculo = $"{row["placa_tracto"]?.ToString()}/{row["placa_cisterna"]?.ToString()}";
                        string estado = row["estado"]?.ToString() ?? "";

                        gridRow.Cells[0].Value = fechaProgramada != DateTime.MinValue ? 
                            fechaProgramada.ToString("dd/MM/yyyy") : "";
                        gridRow.Cells[1].Value = diaSemana;
                        gridRow.Cells[2].Value = volumen > 0 ? volumen.ToString("N0") : "";
                        gridRow.Cells[3].Value = $"{origen} ? {destino}";
                        gridRow.Cells[4].Value = vehiculo;
                        gridRow.Cells[5].Value = estado;

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

        private string ObtenerTextoPerido()
        {
            return _periodoActual switch
            {
                "SemanaActual" => "Semana Actual",
                "MesActual" => "Mes Actual",
                "Personalizado" => $"{fechaInicio.Date:dd/MM/yyyy} - {fechaFin.Date:dd/MM/yyyy}",
                _ => _periodoActual
            };
        }
    }
}
