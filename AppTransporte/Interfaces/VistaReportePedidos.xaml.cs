using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = Microsoft.Maui.Graphics.Font;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class VistaReportePedidos : ContentPage
    {
        private List<ResumenPedido> _datosResumen;
        private List<DetallePedido> _datosDetalle;

        public VistaReportePedidos()
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
                var resultado = await Task.Run(() => ObtenerDatosReporte(FechaInicio.Date, FechaFin.Date));
                _datosResumen = resultado.Resumen;
                _datosDetalle = resultado.Detalle;

                // Actualizar la colección de datos
                resumenCollectionView.ItemsSource = _datosResumen;
                detalleCollectionView.ItemsSource = _datosDetalle;

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

        private (List<ResumenPedido> Resumen, List<DetallePedido> Detalle) ObtenerDatosReporte(DateTime fechaInicio, DateTime fechaFin)
        {
            List<ResumenPedido> resumen = new List<ResumenPedido>();
            List<DetallePedido> detalle = new List<DetallePedido>();

            try
            {
                // Utilizar el servicio existente en App.Database
                var resultado = App.Database.GetReportePedidos(fechaInicio, fechaFin);
                DataTable resumenTable = resultado.Resumen;
                DataTable detalleTable = resultado.Detalle;

                // Procesar datos de resumen
                bool alternarFila = false;
                foreach (DataRow fila in resumenTable.Rows)
                {
                    var item = new ResumenPedido
                    {
                        IdEstadoPedido = Convert.ToInt32(fila["id_estadoPedido"]),
                        EstadoPedido = fila["Estado del Pedido"].ToString(),
                        CantidadPedidos = Convert.ToInt32(fila["Cantidad de Pedidos"]),
                        VolumenTotal = Convert.ToInt32(fila["Volumen Total"]),
                        ViajesSolicitados = Convert.ToInt32(fila["Viajes Solicitados"]),
                        PedidosEntregados = Convert.ToInt32(fila["Pedidos Entregados"]),
                        PromedioDiasAtencion = Convert.ToDouble(fila["Promedio Días Atención"]),
                        Row = alternarFila
                    };

                    resumen.Add(item);
                    alternarFila = !alternarFila;
                }

                // Procesar datos de detalle
                alternarFila = false;
                foreach (DataRow fila in detalleTable.Rows)
                {
                    var item = new DetallePedido
                    {
                        IdPedido = Convert.ToInt32(fila["id_pedido"]),
                        IdSolicitud = Convert.ToInt32(fila["id_solicitud"]),
                        IdCliente = Convert.ToInt32(fila["id_cliente"]),
                        Cliente = fila["Cliente"].ToString(),
                        FechaSolicitud = Convert.ToDateTime(fila["fecha_solicitud"]),
                        FechaEntrega = fila["fecha_entrega"] != DBNull.Value ? Convert.ToDateTime(fila["fecha_entrega"]) : (DateTime?)null,
                        Estado = fila["Estado"].ToString(),
                        Origen = fila["Origen"].ToString(),
                        Destino = fila["Destino"].ToString(),
                        Volumen = Convert.ToInt32(fila["Volumen"]),
                        ViajesSolicitados = Convert.ToInt32(fila["Viajes Solicitados"]),
                        ViajesRealizados = Convert.ToInt32(fila["Viajes Realizados"]),
                        Row = alternarFila
                    };

                    detalle.Add(item);
                    alternarFila = !alternarFila;
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción durante la carga
                Console.WriteLine($"Error al obtener datos del reporte: {ex.Message}");
            }

            return (resumen, detalle);
        }

        private void ActualizarResumen()
        {
            if (_datosResumen != null && _datosResumen.Any())
            {
                int totalPedidos = _datosResumen.Sum(r => r.CantidadPedidos);
                int volumenTotal = _datosResumen.Sum(r => r.VolumenTotal);
                int pedidosEntregados = _datosResumen.Sum(r => r.PedidosEntregados);

                TotalPedidosLabel.Text = totalPedidos.ToString();
                VolumenTotalLabel.Text = $"{volumenTotal:N0} L";
                PedidosEntregadosLabel.Text = $"{pedidosEntregados:N0} ({(double)pedidosEntregados / totalPedidos * 100:N0}%)";
            }
            else
            {
                TotalPedidosLabel.Text = "0";
                VolumenTotalLabel.Text = "0 L";
                PedidosEntregadosLabel.Text = "0 (0%)";
            }
        }

        private void ActualizarGrafico()
        {
            // Implementar el dibujo del gráfico utilizando Microsoft.Maui.Graphics
            graficoView.Drawable = new GraficoPedidos(_datosResumen);
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            if ((_datosResumen == null || !_datosResumen.Any()) &&
                (_datosDetalle == null || !_datosDetalle.Any()))
            {
                await DisplayAlert("Sin datos", "No hay datos para exportar", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                byte[] pdfBytes = await Task.Run(() => GenerarReportePedidosPDF(_datosResumen, _datosDetalle, FechaInicio.Date, FechaFin.Date));

                // Guardar el PDF en el almacenamiento
                string nombreArchivo = $"Reporte_Pedidos_{DateTime.Now:yyyyMMddHHmmss}.pdf";
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
            if ((_datosResumen == null || !_datosResumen.Any()) &&
                (_datosDetalle == null || !_datosDetalle.Any()))
            {
                await DisplayAlert("Sin datos", "No hay datos para compartir", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                byte[] pdfBytes = await Task.Run(() => GenerarReportePedidosPDF(_datosResumen, _datosDetalle, FechaInicio.Date, FechaFin.Date));

                // Guardar el PDF en el almacenamiento
                string nombreArchivo = $"Reporte_Pedidos_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);

                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Compartir el archivo
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Pedidos",
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

        public byte[] GenerarReportePedidosPDF(List<ResumenPedido> resumenData, List<DetallePedido> detalleData, DateTime fechaInicio, DateTime fechaFin)
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
                Paragraph titulo = new Paragraph("Reporte de Pedidos", titleFont);
                titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                titulo.SpacingAfter = 20;
                document.Add(titulo);

                // Información del reporte
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                Paragraph info = new Paragraph($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}", normalFont);
                info.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                info.SpacingAfter = 20;
                document.Add(info);

                // Resumen
                iTextSharp.text.Font subtitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD);
                Paragraph resumenTitulo = new Paragraph("Resumen de Pedidos", subtitleFont);
                resumenTitulo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                resumenTitulo.SpacingAfter = 10;
                document.Add(resumenTitulo);

                // Tabla de resumen global
                PdfPTable resumenGlobalTable = new PdfPTable(2);
                resumenGlobalTable.WidthPercentage = 100;
                resumenGlobalTable.SpacingAfter = 20;

                // Cabecera de la tabla resumen global
                PdfPCell headerCell1 = new PdfPCell(new Phrase("Descripción", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
                headerCell1.BackgroundColor = new BaseColor(220, 220, 220); // Light gray
                headerCell1.Padding = 5;

                PdfPCell headerCell2 = new PdfPCell(new Phrase("Valor", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
                headerCell2.BackgroundColor = new BaseColor(220, 220, 220); // Light gray
                headerCell2.Padding = 5;

                resumenGlobalTable.AddCell(headerCell1);
                resumenGlobalTable.AddCell(headerCell2);

                // Datos del resumen global
                int totalPedidos = resumenData.Sum(r => r.CantidadPedidos);
                int volumenTotal = resumenData.Sum(r => r.VolumenTotal);
                int pedidosEntregados = resumenData.Sum(r => r.PedidosEntregados);
                double porcentajeCompletado = totalPedidos > 0 ? (double)pedidosEntregados / totalPedidos * 100 : 0;

                AddRowToTable(resumenGlobalTable, "Total de Pedidos", totalPedidos.ToString(), normalFont);
                AddRowToTable(resumenGlobalTable, "Volumen Total", $"{volumenTotal:N0} L", normalFont);
                AddRowToTable(resumenGlobalTable, "Pedidos Entregados", $"{pedidosEntregados} ({porcentajeCompletado:N0}%)", normalFont);

                document.Add(resumenGlobalTable);

                // Tabla de resumen por estado
                Paragraph resumenEstadoTitulo = new Paragraph("Resumen por Estado", subtitleFont);
                resumenEstadoTitulo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                resumenEstadoTitulo.SpacingAfter = 10;
                document.Add(resumenEstadoTitulo);

                PdfPTable resumenTable = new PdfPTable(6);
                resumenTable.WidthPercentage = 100;
                resumenTable.SpacingAfter = 20;

                // Cabecera de la tabla resumen por estado
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);
                BaseColor headerColor = new BaseColor(220, 220, 220); // Light gray

                AddHeaderToTable(resumenTable, "Estado", headerFont, headerColor);
                AddHeaderToTable(resumenTable, "Pedidos", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);
                AddHeaderToTable(resumenTable, "Volumen", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);
                AddHeaderToTable(resumenTable, "Viajes", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);
                AddHeaderToTable(resumenTable, "Entregados", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);
                AddHeaderToTable(resumenTable, "Prom. Días", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);

                // Filas de datos de resumen por estado
                bool colorAlternado = false;
                foreach (var item in resumenData)
                {
                    BaseColor bgColor = colorAlternado
                        ? BaseColor.WHITE
                        : new BaseColor(245, 245, 245); // Very light gray

                    colorAlternado = !colorAlternado;

                    AddCellToTable(resumenTable, item.EstadoPedido, normalFont, bgColor);
                    AddCellToTable(resumenTable, item.CantidadPedidos.ToString(), normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AddCellToTable(resumenTable, $"{item.VolumenTotal:N0}", normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AddCellToTable(resumenTable, item.ViajesSolicitados.ToString(), normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AddCellToTable(resumenTable, item.PedidosEntregados.ToString(), normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AddCellToTable(resumenTable, $"{item.PromedioDiasAtencion:N1}", normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                }

                document.Add(resumenTable);

                // Tabla de detalle
                Paragraph detalleTitulo = new Paragraph("Detalle de Pedidos", subtitleFont);
                detalleTitulo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                detalleTitulo.SpacingAfter = 10;
                document.Add(detalleTitulo);

                PdfPTable detalleTable = new PdfPTable(7);
                detalleTable.WidthPercentage = 100;
                float[] anchos = new float[] { 0.5f, 2f, 1.5f, 1.5f, 1f, 1f, 1f };
                detalleTable.SetWidths(anchos);

                // Cabecera de la tabla de detalle
                AddHeaderToTable(detalleTable, "#", headerFont, headerColor);
                AddHeaderToTable(detalleTable, "Cliente", headerFont, headerColor);
                AddHeaderToTable(detalleTable, "Origen", headerFont, headerColor);
                AddHeaderToTable(detalleTable, "Destino", headerFont, headerColor);
                AddHeaderToTable(detalleTable, "Estado", headerFont, headerColor);
                AddHeaderToTable(detalleTable, "Volumen", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);
                AddHeaderToTable(detalleTable, "Viajes", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);

                // Limitar a 30 registros para evitar PDFs demasiado grandes
                var pedidosLimitados = detalleData.Take(30).ToList();

                // Filas de datos de detalle
                colorAlternado = false;
                foreach (var item in pedidosLimitados)
                {
                    BaseColor bgColor = colorAlternado
                        ? BaseColor.WHITE
                        : new BaseColor(245, 245, 245); // Very light gray

                    colorAlternado = !colorAlternado;

                    AddCellToTable(detalleTable, item.IdPedido.ToString(), normalFont, bgColor);
                    AddCellToTable(detalleTable, item.Cliente, normalFont, bgColor);
                    AddCellToTable(detalleTable, item.Origen, normalFont, bgColor);
                    AddCellToTable(detalleTable, item.Destino, normalFont, bgColor);
                    AddCellToTable(detalleTable, item.Estado, normalFont, bgColor);
                    AddCellToTable(detalleTable, $"{item.Volumen:N0}", normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AddCellToTable(detalleTable, $"{item.ViajesRealizados}/{item.ViajesSolicitados}", normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                }

                document.Add(detalleTable);

                // Si hay más de 30 registros, agregar nota
                if (detalleData.Count > 30)
                {
                    iTextSharp.text.Font noteFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.ITALIC);
                    noteFont.Color = BaseColor.GRAY;

                    Paragraph nota = new Paragraph($"Nota: Se muestran solo los primeros 30 pedidos de un total de {detalleData.Count}.", noteFont);
                    nota.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                    nota.SpacingBefore = 10;
                    document.Add(nota);
                }

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

        // Métodos auxiliares para crear tablas PDF
        private void AddRowToTable(PdfPTable table, string descripcion, string valor, iTextSharp.text.Font font)
        {
            PdfPCell cellDesc = new PdfPCell(new Phrase(descripcion, font));
            cellDesc.Padding = 5;

            PdfPCell cellVal = new PdfPCell(new Phrase(valor, font));
            cellVal.Padding = 5;

            table.AddCell(cellDesc);
            table.AddCell(cellVal);
        }

        private void AddHeaderToTable(PdfPTable table, string texto, iTextSharp.text.Font font, BaseColor bgColor, int alignment = iTextSharp.text.Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(texto, font));
            cell.BackgroundColor = bgColor;
            cell.Padding = 5;
            cell.HorizontalAlignment = alignment;

            table.AddCell(cell);
        }

        private void AddCellToTable(PdfPTable table, string texto, iTextSharp.text.Font font, BaseColor bgColor, int alignment = iTextSharp.text.Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(texto, font));
            cell.BackgroundColor = bgColor;
            cell.Padding = 5;
            cell.HorizontalAlignment = alignment;

            table.AddCell(cell);
        }
    }

    // Clase para el gráfico de pedidos por estado
    public class GraficoPedidos : IDrawable
    {
        private List<ResumenPedido> _datos;

        public GraficoPedidos(List<ResumenPedido> datos)
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
            float margenIzquierdo = 20;
            float margenDerecho = 20;
            float margenSuperior = 20;
            float margenInferior = 60;

            float centroX = width / 2;
            float centroY = (height - margenInferior) / 2;
            float radio = Math.Min(centroY - margenSuperior, (width - margenIzquierdo - margenDerecho) / 2);

            // Colores para el gráfico
            Color[] colores = new Color[]
            {
                Colors.DodgerBlue,
                Colors.OrangeRed,
                Colors.Green,
                Colors.Purple,
                Colors.Orange,
                Colors.DeepPink,
                Colors.Teal
            };

            // Total de pedidos
            int totalPedidos = _datos.Sum(d => d.CantidadPedidos);

            // Dibujar gráfico de torta
            float anguloInicial = 0;

            for (int i = 0; i < _datos.Count; i++)
            {
                var estado = _datos[i];
                float porcentaje = (float)estado.CantidadPedidos / totalPedidos;
                float angulo = porcentaje * 360f;

                // Seleccionar color
                canvas.FillColor = colores[i % colores.Length];

                // Dibujar sector
                canvas.FillArc(centroX - radio, centroY - radio, centroX + radio, centroY + radio, anguloInicial, anguloInicial + angulo, true);

                // Calcular la posición para la leyenda
                double anguloMedio = Math.PI * (2 * anguloInicial + angulo) / 360;
                float posX = margenIzquierdo + (i % 3) * ((width - margenIzquierdo - margenDerecho) / 3);
                float posY = height - margenInferior + 15 + (i / 3) * 20;

                // Dibujar cuadrado de color para la leyenda
                canvas.FillColor = colores[i % colores.Length];
                canvas.FillRectangle(posX, posY, 10, 10);

                // Dibujar texto de la leyenda
                canvas.FontColor = Colors.Black;
                canvas.FontSize = 10;

                // Abreviar texto si es muy largo
                string etiqueta = estado.EstadoPedido;
                if (etiqueta.Length > 15)
                    etiqueta = etiqueta.Substring(0, 12) + "...";

                canvas.DrawString($"{etiqueta} ({estado.CantidadPedidos}, {porcentaje:P0})", posX + 15, posY + 7, HorizontalAlignment.Left);

                // Actualizar ángulo inicial para el siguiente sector
                anguloInicial += angulo;
            }

            // Dibujar título del gráfico
            canvas.FontColor = Colors.Black;
            canvas.FontSize = 12;
            canvas.DrawString("Distribución de Pedidos por Estado", width / 2, margenSuperior - 5, HorizontalAlignment.Center);
        }
    }
}
    