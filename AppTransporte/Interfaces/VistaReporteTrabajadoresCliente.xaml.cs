using System;
using System.Data;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

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
                // Generar el PDF en segundo plano para no bloquear la UI
                byte[] pdfBytes = await Task.Run(() => GenerarPDF());

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
                // Generar el PDF
                byte[] pdfBytes = await Task.Run(() => GenerarPDF());

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

        private byte[] GenerarPDF()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Usar alias para evitar ambigüedad
                iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 36, 36, 36, 36);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
                document.Open();

                // Fuentes
                var titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, iTextSharp.text.Font.BOLD);
                var normalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                var subtitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD);
                var headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);
                var footerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10);
                footerFont.Color = iTextSharp.text.BaseColor.GRAY;

                // Añadir título
                var titulo = new iTextSharp.text.Paragraph("Reporte de Trabajadores Asignados", titleFont);
                titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                titulo.SpacingAfter = 20;
                document.Add(titulo);

                // Añadir información del cliente y fecha
                document.Add(new iTextSharp.text.Paragraph($"Cliente ID: {_idCliente}", normalFont));
                document.Add(new iTextSharp.text.Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", normalFont));
                document.Add(new iTextSharp.text.Paragraph(" "));

                // Añadir resumen
                document.Add(new iTextSharp.text.Paragraph("Resumen", subtitleFont));

                var resumenTable = new iTextSharp.text.pdf.PdfPTable(2);
                resumenTable.WidthPercentage = 100;
                resumenTable.SpacingAfter = 20;

                // Cabecera de la tabla resumen
                var headerColor = new iTextSharp.text.BaseColor(220, 220, 220);

                var headerCell1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Métrica", headerFont));
                headerCell1.BackgroundColor = headerColor;
                headerCell1.Padding = 5;

                var headerCell2 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Valor", headerFont));
                headerCell2.BackgroundColor = headerColor;
                headerCell2.Padding = 5;

                resumenTable.AddCell(headerCell1);
                resumenTable.AddCell(headerCell2);

                // Datos del resumen
                int totalTrabajadores = _datosReporte.Rows.Count;
                int totalViajes = _datosReporte.AsEnumerable().Sum(r => Convert.ToInt32(r["total_viajes"]));
                int totalSeguimientos = _datosReporte.AsEnumerable().Sum(r => Convert.ToInt32(r["total_seguimientos"]));
                int volumenTotal = _datosReporte.AsEnumerable().Sum(r => Convert.ToInt32(r["volumen_transportado"]));

                AgregarFilaTabla(resumenTable, "Total de Trabajadores", totalTrabajadores.ToString(), normalFont);
                AgregarFilaTabla(resumenTable, "Total de Viajes", totalViajes.ToString(), normalFont);
                AgregarFilaTabla(resumenTable, "Total de Seguimientos", totalSeguimientos.ToString(), normalFont);
                AgregarFilaTabla(resumenTable, "Volumen Total Transportado", $"{volumenTotal} L", normalFont);

                document.Add(resumenTable);

                // Añadir detalle de trabajadores
                document.Add(new iTextSharp.text.Paragraph("Detalle de Trabajadores", subtitleFont));
                document.Add(new iTextSharp.text.Paragraph(" "));

                // Tabla de detalle
                var detalleTable = new iTextSharp.text.pdf.PdfPTable(5);
                detalleTable.WidthPercentage = 100;
                float[] anchos = new float[] { 3f, 2f, 1f, 1f, 2f };
                detalleTable.SetWidths(anchos);

                // Cabecera de la tabla detalle
                AgregarCeldaEncabezado(detalleTable, "Nombre", headerFont, headerColor);
                AgregarCeldaEncabezado(detalleTable, "Categoría", headerFont, headerColor);
                AgregarCeldaEncabezado(detalleTable, "Viajes", headerFont, headerColor);
                AgregarCeldaEncabezado(detalleTable, "Seguimientos", headerFont, headerColor);
                AgregarCeldaEncabezado(detalleTable, "Volumen (L)", headerFont, headerColor);

                // Filas de datos
                bool colorAlternado = false;
                foreach (System.Data.DataRow row in _datosReporte.Rows)
                {
                    var bgColor = colorAlternado ? new iTextSharp.text.BaseColor(245, 245, 245) : iTextSharp.text.BaseColor.WHITE;
                    colorAlternado = !colorAlternado;

                    string nombreCompleto = $"{row["Nombre"]} {row["apePaterno"]} {row["apeMaterno"]}".Trim();
                    string categoria = row["categoria_desc"]?.ToString() ?? "";
                    int viajes = Convert.ToInt32(row["total_viajes"]);
                    int seguimientos = Convert.ToInt32(row["total_seguimientos"]);
                    int volumen = Convert.ToInt32(row["volumen_transportado"]);

                    AgregarCeldaDetalle(detalleTable, nombreCompleto, normalFont, bgColor, iTextSharp.text.Element.ALIGN_LEFT);
                    AgregarCeldaDetalle(detalleTable, categoria, normalFont, bgColor, iTextSharp.text.Element.ALIGN_LEFT);
                    AgregarCeldaDetalle(detalleTable, viajes.ToString(), normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AgregarCeldaDetalle(detalleTable, seguimientos.ToString(), normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AgregarCeldaDetalle(detalleTable, volumen.ToString(), normalFont, bgColor, iTextSharp.text.Element.ALIGN_RIGHT);
                }

                document.Add(detalleTable);

                // Pie de página
                var footer = new iTextSharp.text.Paragraph($"Reporte generado por App Transporte • {DateTime.Now:dd/MM/yyyy HH:mm:ss}", footerFont);
                footer.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                footer.SpacingBefore = 20;
                document.Add(footer);

                document.Close();
                return ms.ToArray();
            }
        }

        // Métodos auxiliares: asegúrate de que también usen los tipos de iTextSharp explícitamente
        private void AgregarFilaTabla(iTextSharp.text.pdf.PdfPTable table, string columna1, string columna2, iTextSharp.text.Font font)
        {
            var cell1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(columna1, font));
            cell1.Padding = 5;

            var cell2 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(columna2, font));
            cell2.Padding = 5;

            table.AddCell(cell1);
            table.AddCell(cell2);
        }

        private void AgregarCeldaEncabezado(iTextSharp.text.pdf.PdfPTable table, string texto, iTextSharp.text.Font font, iTextSharp.text.BaseColor color)
        {
            var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(texto, font));
            cell.BackgroundColor = color;
            cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private void AgregarCeldaDetalle(iTextSharp.text.pdf.PdfPTable table, string texto, iTextSharp.text.Font font, iTextSharp.text.BaseColor color, int alineacion)
        {
            var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(texto, font));
            cell.BackgroundColor = color;
            cell.HorizontalAlignment = alineacion;
            cell.Padding = 5;
            table.AddCell(cell);
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
                int volumenTotal = _datosReporte.AsEnumerable()
                    .Sum(r => Convert.ToInt32(r["volumen_transportado"]));

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
