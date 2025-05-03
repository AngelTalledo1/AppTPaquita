using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = Microsoft.Maui.Graphics.Font;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class VistaReporteServicios : ContentPage
    {
        private List<ReporteServicio> _datosReporte;

        public VistaReporteServicios()
        {
            InitializeComponent();

            // Inicializar fechas predeterminadas (último mes)
            FechaFin.Date = DateTime.Now;
            FechaInicio.Date = DateTime.Now.AddMonths(-1);

            ActualizarEtiquetaFechas();

            // Cargar los datos iniciales
            CargarDatosReporte();
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OnFechaSeleccionada(object sender, DateChangedEventArgs e)
        {
            ActualizarEtiquetaFechas();
        }

        private void ActualizarEtiquetaFechas()
        {
            FechasReporte.Text = $"Periodo: {FechaInicio.Date:dd/MM/yyyy} - {FechaFin.Date:dd/MM/yyyy}";
        }

        private async void ActualizarReporte_Clicked(object sender, EventArgs e)
        {
            // Validar que la fecha de inicio no sea posterior a la fecha fin
            if (FechaInicio.Date > FechaFin.Date)
            {
                await DisplayAlert("Error", "La fecha de inicio no puede ser posterior a la fecha fin", "Ok");
                return;
            }

            await CargarDatosReporte();
        }

        private async Task CargarDatosReporte()
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                _datosReporte = await Task.Run(() => ObtenerDatosReporte(FechaInicio.Date, FechaFin.Date));

                // Actualizar la colección de datos
                reporteCollectionView.ItemsSource = _datosReporte;

                // Actualizar etiquetas de resumen
                ActualizarResumen();

                // Actualizar gráfico
                ActualizarGrafico();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al cargar los datos: {ex.Message}", "Ok");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

       private List<ReporteServicio> ObtenerDatosReporte(DateTime fechaInicio, DateTime fechaFin)
        {
            List<ReporteServicio> resultado = new List<ReporteServicio>();

            try
            {
                // Utilizar el servicio existente en App.Database
                DataTable datosReporte = App.Database.ObtenerReporteServicios(fechaInicio, fechaFin);

                bool alternarFila = false;
                foreach (DataRow fila in datosReporte.Rows)
                {
                    var reporte = new ReporteServicio
                    {
                        IdServicio = Convert.ToInt32(fila["id_servicio"]),
                        TipoServicio = fila["Tipo de Servicio"].ToString(),
                        CantidadPedidos = Convert.ToInt32(fila["Cantidad de Pedidos"]),
                        VolumenSolicitado = Convert.ToInt32(fila["Volumen Total Solicitado"]),
                        VolumenTransportado = Convert.ToInt32(fila["Volumen Total Transportado"]),
                        PorcentajeCumplimiento = Convert.ToDouble(fila["Porcentaje Cumplimiento"]),
                        Row = alternarFila
                    };

                    resultado.Add(reporte);
                    alternarFila = !alternarFila;
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción durante la carga
                Console.WriteLine($"Error al obtener datos del reporte: {ex.Message}");
            }

            return resultado;
        }

        private void ActualizarResumen()
        {
            if (_datosReporte != null && _datosReporte.Any())
            {
                TotalServiciosLabel.Text = _datosReporte.Count.ToString();
                TotalPedidosLabel.Text = _datosReporte.Sum(r => r.CantidadPedidos).ToString();
                VolumenTotalLabel.Text = $"{_datosReporte.Sum(r => r.VolumenTransportado):N0} L";
            }
            else
            {
                TotalServiciosLabel.Text = "0";
                TotalPedidosLabel.Text = "0";
                VolumenTotalLabel.Text = "0.00 L";
            }
        }

        private void ActualizarGrafico()
        {
            // Implementar el dibujo del gráfico utilizando Microsoft.Maui.Graphics
            graficoView.Drawable = new GraficoBarras(_datosReporte);
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            if (_datosReporte == null || !_datosReporte.Any())
            {
                await DisplayAlert("Sin datos", "No hay datos para exportar", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                byte[] pdfBytes = await Task.Run(() => GenerarReporteServiciosPDF(_datosReporte, FechaInicio.Date, FechaFin.Date));

                // Guardar el PDF en el almacenamiento
                string nombreArchivo = $"Reporte_Servicios_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);

                File.WriteAllBytes(rutaArchivo, pdfBytes);

                await DisplayAlert("Éxito", "PDF generado correctamente", "Ok");

                // Abrir el PDF (implementación específica de la plataforma)
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(rutaArchivo)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al generar PDF: {ex.Message}", "Ok");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void Compartir_Clicked(object sender, EventArgs e)
        {
            if (_datosReporte == null || !_datosReporte.Any())
            {
                await DisplayAlert("Sin datos", "No hay datos para compartir", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                byte[] pdfBytes = await Task.Run(() => GenerarReporteServiciosPDF(_datosReporte, FechaInicio.Date, FechaFin.Date));

                // Guardar el PDF en el almacenamiento
                string nombreArchivo = $"Reporte_Servicios_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);

                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Compartir el archivo
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Servicios",
                    File = new ShareFile(rutaArchivo)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al compartir: {ex.Message}", "Ok");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        public byte[] GenerarReporteServiciosPDF(List<ReporteServicio> reporteData, DateTime fechaInicio, DateTime fechaFin)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Crear documento PDF con iTextSharp
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 36, 36, 36, 36);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Título del documento
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA,
                                                                          18,
                                                                          iTextSharp.text.Font.BOLD);
                Paragraph titulo = new Paragraph("Reporte de Servicios", titleFont);
                titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                titulo.SpacingAfter = 20;
                document.Add(titulo);

                // Información del reporte
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                Paragraph info = new Paragraph($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}", normalFont);
                info.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                info.SpacingAfter = 20;
                document.Add(info);

                // Tabla de resumen
                iTextSharp.text.Font subtitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD);
                Paragraph resumenTitulo = new Paragraph("Resumen", subtitleFont);
                resumenTitulo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                resumenTitulo.SpacingAfter = 10;
                document.Add(resumenTitulo);

                PdfPTable resumenTable = new PdfPTable(2);
                resumenTable.WidthPercentage = 100;
                resumenTable.SpacingAfter = 20;

                // Cabecera de la tabla resumen
                PdfPCell headerCell1 = new PdfPCell(new Phrase("Descripción", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
                headerCell1.BackgroundColor = new BaseColor(220, 220, 220); // Light gray
                headerCell1.Padding = 5;

                PdfPCell headerCell2 = new PdfPCell(new Phrase("Valor", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
                headerCell2.BackgroundColor = new BaseColor(220, 220, 220); // Light gray
                headerCell2.Padding = 5;

                resumenTable.AddCell(headerCell1);
                resumenTable.AddCell(headerCell2);

                // Datos del resumen
                int totalServicios = reporteData.Count;
                int totalPedidos = reporteData.Sum(r => r.CantidadPedidos);
                int volumenTotal = reporteData.Sum(r => r.VolumenTransportado);

                PdfPCell cellDesc1 = new PdfPCell(new Phrase("Total de Servicios", normalFont));
                cellDesc1.Padding = 5;
                PdfPCell cellVal1 = new PdfPCell(new Phrase(totalServicios.ToString(), normalFont));
                cellVal1.Padding = 5;

                PdfPCell cellDesc2 = new PdfPCell(new Phrase("Total de Pedidos", normalFont));
                cellDesc2.Padding = 5;
                PdfPCell cellVal2 = new PdfPCell(new Phrase(totalPedidos.ToString(), normalFont));
                cellVal2.Padding = 5;

                PdfPCell cellDesc3 = new PdfPCell(new Phrase("Volumen Total Transportado", normalFont));
                cellDesc3.Padding = 5;
                PdfPCell cellVal3 = new PdfPCell(new Phrase($"{volumenTotal:N0} L", normalFont));
                cellVal3.Padding = 5;

                resumenTable.AddCell(cellDesc1);
                resumenTable.AddCell(cellVal1);
                resumenTable.AddCell(cellDesc2);
                resumenTable.AddCell(cellVal2);
                resumenTable.AddCell(cellDesc3);
                resumenTable.AddCell(cellVal3);

                document.Add(resumenTable);

                // Tabla de detalle
                Paragraph detalleTitulo = new Paragraph("Detalle por Servicio", subtitleFont);
                detalleTitulo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                detalleTitulo.SpacingAfter = 10;
                document.Add(detalleTitulo);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;

                // Cabecera de la tabla de detalle
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);
                BaseColor headerColor = new BaseColor(220, 220, 220); // Light gray

                PdfPCell headerServicio = new PdfPCell(new Phrase("Tipo de Servicio", headerFont));
                headerServicio.BackgroundColor = headerColor;
                headerServicio.Padding = 5;

                PdfPCell headerPedidos = new PdfPCell(new Phrase("Pedidos", headerFont));
                headerPedidos.BackgroundColor = headerColor;
                headerPedidos.Padding = 5;
                headerPedidos.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;

                PdfPCell headerVolSol = new PdfPCell(new Phrase("Vol. Solicitado", headerFont));
                headerVolSol.BackgroundColor = headerColor;
                headerVolSol.Padding = 5;
                headerVolSol.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;

                PdfPCell headerVolTrans = new PdfPCell(new Phrase("Vol. Transportado", headerFont));
                headerVolTrans.BackgroundColor = headerColor;
                headerVolTrans.Padding = 5;
                headerVolTrans.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;

                PdfPCell headerPorcentaje = new PdfPCell(new Phrase("% Cumplimiento", headerFont));
                headerPorcentaje.BackgroundColor = headerColor;
                headerPorcentaje.Padding = 5;
                headerPorcentaje.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;

                table.AddCell(headerServicio);
                table.AddCell(headerPedidos);
                table.AddCell(headerVolSol);
                table.AddCell(headerVolTrans);
                table.AddCell(headerPorcentaje);

                // Filas de datos
                bool colorAlternado = false;
                foreach (var item in reporteData)
                {
                    BaseColor bgColor = colorAlternado
                        ? BaseColor.WHITE
                        : new BaseColor(245, 245, 245); // Very light gray

                    colorAlternado = !colorAlternado;

                    PdfPCell cellServicio = new PdfPCell(new Phrase(item.TipoServicio, normalFont));
                    cellServicio.BackgroundColor = bgColor;
                    cellServicio.Padding = 5;

                    PdfPCell cellPedidos = new PdfPCell(new Phrase(item.CantidadPedidos.ToString(), normalFont));
                    cellPedidos.BackgroundColor = bgColor;
                    cellPedidos.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cellPedidos.Padding = 5;

                    PdfPCell cellVolSol = new PdfPCell(new Phrase($"{item.VolumenSolicitado:N0}", normalFont));
                    cellVolSol.BackgroundColor = bgColor;
                    cellVolSol.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cellVolSol.Padding = 5;

                    PdfPCell cellVolTrans = new PdfPCell(new Phrase($"{item.VolumenTransportado:N0}", normalFont));
                    cellVolTrans.BackgroundColor = bgColor;
                    cellVolTrans.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cellVolTrans.Padding = 5;

                    PdfPCell cellPorcentaje = new PdfPCell(new Phrase($"{item.PorcentajeCumplimiento:N2}%", normalFont));
                    cellPorcentaje.BackgroundColor = bgColor;
                    cellPorcentaje.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                    cellPorcentaje.Padding = 5;

                    table.AddCell(cellServicio);
                    table.AddCell(cellPedidos);
                    table.AddCell(cellVolSol);
                    table.AddCell(cellVolTrans);
                    table.AddCell(cellPorcentaje);
                }

                document.Add(table);

                // Añadir pie de página
                iTextSharp.text.Font footerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10);
                footerFont.Color = BaseColor.GRAY;

                Paragraph footer = new Paragraph($"Reporte generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}", footerFont);
                footer.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                footer.SpacingBefore = 20;
                document.Add(footer);

                document.Close();
                return ms.ToArray();
            }
        }
    }

    // Clase para el gráfico de barras
    public class GraficoBarras : IDrawable
    {
        private List<ReporteServicio> _datos;

        public GraficoBarras(List<ReporteServicio> datos)
        {
            _datos = datos;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (_datos == null || !_datos.Any())
                return;

            // Configuración del gráfico
            float width = dirtyRect.Width;
            float height = dirtyRect.Height;
            float margenIzquierdo = 40;
            float margenDerecho = 20;
            float margenSuperior = 20;
            float margenInferior = 60;

            float areaGraficoAncho = width - margenIzquierdo - margenDerecho;
            float areaGraficoAlto = height - margenSuperior - margenInferior;

            // Dibujar ejes
            canvas.StrokeColor = Colors.Gray;
            canvas.StrokeSize = 1;

            // Eje Y
            canvas.DrawLine(margenIzquierdo, margenSuperior, margenIzquierdo, height - margenInferior);
            // Eje X
            canvas.DrawLine(margenIzquierdo, height - margenInferior, width - margenDerecho, height - margenInferior);

            // Dibujar barras
            int numServicios = _datos.Count;
            float anchoBarras = Math.Min(30, areaGraficoAncho / numServicios * 0.6f);
            float espacioEntreBarras = areaGraficoAncho / numServicios - anchoBarras;

            for (int i = 0; i < numServicios; i++)
            {
                var servicio = _datos[i];
                float x = margenIzquierdo + (i * (anchoBarras + espacioEntreBarras)) + espacioEntreBarras / 2;

                // Altura de la barra proporcional al porcentaje de cumplimiento (máximo 100%)
                float porcentaje = (float)Math.Min(servicio.PorcentajeCumplimiento, 100) / 100f;
                float altoBarra = areaGraficoAlto * porcentaje;
                float y = height - margenInferior - altoBarra;

                // Color de la barra según el porcentaje
                if (porcentaje >= 0.8f)
                    canvas.FillColor = Colors.Green;
                else if (porcentaje >= 0.5f)
                    canvas.FillColor = Colors.Orange;
                else
                    canvas.FillColor = Colors.Red;

                // Dibujar barra
                canvas.FillRectangle(x, y, anchoBarras, altoBarra);

                // Dibujar etiqueta del servicio
                canvas.FontColor = Colors.Black;
                canvas.FontSize = 10;

                // Etiqueta de servicio abreviada
                string etiqueta = servicio.TipoServicio;
                if (etiqueta.Length > 10)
                    etiqueta = etiqueta.Substring(0, 7) + "...";

                // Rotar texto para etiquetas en eje X
                canvas.SaveState();
                canvas.Rotate(-45, x + anchoBarras / 2, height - margenInferior + 5);
                canvas.DrawString(etiqueta, x, height - margenInferior + 5, HorizontalAlignment.Left);
                canvas.RestoreState();

                // Porcentaje encima de la barra
                canvas.DrawString($"{servicio.PorcentajeCumplimiento:N0}%", x + anchoBarras / 2, y - 15, HorizontalAlignment.Center);
            }

            // Dibujar título del eje Y
            canvas.SaveState();
            canvas.Rotate(-90, margenIzquierdo - 25, height / 2);
            canvas.DrawString("% Cumplimiento", margenIzquierdo - 25, height / 2, HorizontalAlignment.Center);
            canvas.RestoreState();

            // Escala en eje Y (0%, 25%, 50%, 75%, 100%)
            for (int i = 0; i <= 4; i++)
            {
                float porcentaje = i * 25;
                float y = height - margenInferior - (areaGraficoAlto * (porcentaje / 100f));

                // Línea de guía
                canvas.StrokeColor = Colors.LightGray;
                canvas.StrokeSize = 1;
                canvas.DrawLine(margenIzquierdo - 5, y, width - margenDerecho, y);

                // Texto del porcentaje
                canvas.FontColor = Colors.Gray;
                canvas.FontSize = 10;
                canvas.DrawString($"{porcentaje}%", margenIzquierdo - 25, y, HorizontalAlignment.Center);
            }
        }
    }
}