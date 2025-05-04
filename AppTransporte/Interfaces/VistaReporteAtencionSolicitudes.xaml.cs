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
    public partial class VistaReporteAtencionSolicitudes : ContentPage
    {
        private ResumenAtencion _metricas;
        private List<AtencionCliente> _datosClientes;
        private List<DetalleSolicitud> _datosDetalle;

        public VistaReporteAtencionSolicitudes()
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
                _metricas = resultado.Metricas;
                _datosClientes = resultado.Clientes;
                _datosDetalle = resultado.Detalle;

                // Actualizar la colección de datos
                clientesCollectionView.ItemsSource = _datosClientes;
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

        private (ResumenAtencion Metricas, List<AtencionCliente> Clientes, List<DetalleSolicitud> Detalle) ObtenerDatosReporte(DateTime fechaInicio, DateTime fechaFin)
        {
            ResumenAtencion metricas = null;
            List<AtencionCliente> clientes = new List<AtencionCliente>();
            List<DetalleSolicitud> detalle = new List<DetalleSolicitud>();

            try
            {
                // Utilizar el servicio existente en App.Database
                var resultado = App.Database.ObtenerReporteAtencionSolicitudes(fechaInicio, fechaFin);
                DataTable metricasTable = resultado.MetricasGenerales;
                DataTable clientesTable = resultado.PorCliente;
                DataTable detalleTable = resultado.DetalleSolicitudes;

                // Procesar datos de métricas generales
                if (metricasTable.Rows.Count > 0)
                {
                    DataRow fila = metricasTable.Rows[0];
                    metricas = new ResumenAtencion
                    {
                        PromedioHorasSolicitudPedido = fila["Promedio Horas Solicitud a Pedido"] != DBNull.Value ? Convert.ToDouble(fila["Promedio Horas Solicitud a Pedido"]) : 0,
                        PromedioHorasPedidoEntrega = fila["Promedio Horas Pedido a Entrega (Completados)"] != DBNull.Value ? Convert.ToDouble(fila["Promedio Horas Pedido a Entrega (Completados)"]) : 0,
                        PromedioHorasTotalesCompletados = fila["Promedio Horas Totales (Completados)"] != DBNull.Value ? Convert.ToDouble(fila["Promedio Horas Totales (Completados)"]) : 0,
                        PromedioHorasTotalesTodos = fila["Promedio Horas Totales (Todos)"] != DBNull.Value ? Convert.ToDouble(fila["Promedio Horas Totales (Todos)"]) : 0,
                        TotalSolicitudes = Convert.ToInt32(fila["Total Solicitudes"]),
                        SolicitudesCompletadas = Convert.ToInt32(fila["Solicitudes Completadas"]),
                        PorcentajeCompletado = Convert.ToDouble(fila["Porcentaje Completado"])
                    };
                }

                // Procesar datos por cliente
                bool alternarFila = false;
                foreach (DataRow fila in clientesTable.Rows)
                {
                    int totalSolicitudes = Convert.ToInt32(fila["Total Solicitudes"]);
                    int solicitudesCompletadas = Convert.ToInt32(fila["Solicitudes Completadas"]);

                    var item = new AtencionCliente
                    {
                        IdCliente = Convert.ToInt32(fila["id_cliente"]),
                        Cliente = fila["Cliente"].ToString(),
                        TotalSolicitudes = totalSolicitudes,
                        SolicitudesCompletadas = solicitudesCompletadas,
                        PorcentajeCompletadas = totalSolicitudes > 0 ? (double)solicitudesCompletadas / totalSolicitudes : 0,
                        PromedioHorasSolicitudPedido = fila["Promedio Horas Solicitud a Pedido"] != DBNull.Value ? Convert.ToDouble(fila["Promedio Horas Solicitud a Pedido"]) : 0,
                        PromedioHorasPedidoEntrega = fila["Promedio Horas Pedido a Entrega"] != DBNull.Value ? Convert.ToDouble(fila["Promedio Horas Pedido a Entrega"]) : 0,
                        PromedioHorasTotales = fila["Promedio Horas Totales"] != DBNull.Value ? Convert.ToDouble(fila["Promedio Horas Totales"]) : 0,
                        Row = alternarFila
                    };

                    clientes.Add(item);
                    alternarFila = !alternarFila;
                }

                // Procesar datos de detalle
                alternarFila = false;
                foreach (DataRow fila in detalleTable.Rows)
                {
                    var item = new DetalleSolicitud
                    {
                        IdSolicitud = Convert.ToInt32(fila["id_solicitud"]),
                        FechaSolicitud = Convert.ToDateTime(fila["fecha_solicitud"]),
                        FechaPedido = fila["fecha_pedido"] != DBNull.Value ? Convert.ToDateTime(fila["fecha_pedido"]) : (DateTime?)null,
                        FechaEntrega = fila["fecha_entrega"] != DBNull.Value ? Convert.ToDateTime(fila["fecha_entrega"]) : (DateTime?)null,
                        Cliente = fila["Cliente"].ToString(),
                        DescripcionSolicitud = fila["Descripcion Solicitud"].ToString(),
                        Comentario = fila["Comentario"] != DBNull.Value ? fila["Comentario"].ToString() : string.Empty,
                        Estado = fila["Estado"].ToString(),
                        HorasHastaPedido = Convert.ToDouble(fila["Horas hasta Pedido"]),
                        HorasHastaEntrega = Convert.ToDouble(fila["Horas hasta Entrega"]),
                        HorasTotales = Convert.ToDouble(fila["Horas Totales"]),
                        Completado = fila["Completado"].ToString(),
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

            return (metricas, clientes, detalle);
        }
        private void ActualizarResumen()
        {
            if (_metricas != null)
            {
                TiempoPromSolPedLabel.Text = $"{_metricas.PromedioHorasSolicitudPedido:N1} h";
                TiempoPromPedEntLabel.Text = $"{_metricas.PromedioHorasPedidoEntrega:N1} h";
                TiempoPromTotalCompLabel.Text = $"{_metricas.PromedioHorasTotalesCompletados:N1} h";
                PorcentajeCompletadasLabel.Text = $"{_metricas.PorcentajeCompletado:N1}% ({_metricas.SolicitudesCompletadas}/{_metricas.TotalSolicitudes})";
            }
            else
            {
                TiempoPromSolPedLabel.Text = "0 h";
                TiempoPromPedEntLabel.Text = "0 h";
                TiempoPromTotalCompLabel.Text = "0 h";
                PorcentajeCompletadasLabel.Text = "0%";
            }
        }

        private void ActualizarGrafico()
        {
            // Implementar el dibujo del gráfico utilizando Microsoft.Maui.Graphics
            graficoView.Drawable = new GraficoTiemposAtencion(_metricas, _datosClientes);
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            if (_metricas == null || (_datosClientes == null || !_datosClientes.Any()) ||
                (_datosDetalle == null || !_datosDetalle.Any()))
            {
                await DisplayAlert("Sin datos", "No hay datos para exportar", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                byte[] pdfBytes = await Task.Run(() => GenerarReporteAtencionSolicitudesPDF(_metricas, _datosClientes, _datosDetalle, FechaInicio.Date, FechaFin.Date));

                // Guardar el PDF en el almacenamiento
                string nombreArchivo = $"Reporte_Atencion_Solicitudes_{DateTime.Now:yyyyMMddHHmmss}.pdf";
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
            if (_metricas == null || (_datosClientes == null || !_datosClientes.Any()) ||
                (_datosDetalle == null || !_datosDetalle.Any()))
            {
                await DisplayAlert("Sin datos", "No hay datos para compartir", "Ok");
                return;
            }

            LoadingOverlay.IsVisible = true;

            try
            {
                byte[] pdfBytes = await Task.Run(() => GenerarReporteAtencionSolicitudesPDF(_metricas, _datosClientes, _datosDetalle, FechaInicio.Date, FechaFin.Date));

                // Guardar el PDF en el almacenamiento
                string nombreArchivo = $"Reporte_Atencion_Solicitudes_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);

                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Compartir el archivo
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Atención de Solicitudes",
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

        public byte[] GenerarReporteAtencionSolicitudesPDF(ResumenAtencion metricas, List<AtencionCliente> clientesData,
            List<DetalleSolicitud> detalleData, DateTime fechaInicio, DateTime fechaFin)
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
                Paragraph titulo = new Paragraph("Reporte de Atención de Solicitudes", titleFont);
                titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                titulo.SpacingAfter = 20;
                document.Add(titulo);

                // Información del reporte
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                Paragraph info = new Paragraph($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}", normalFont);
                info.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                info.SpacingAfter = 20;
                document.Add(info);

                // Métricas generales
                iTextSharp.text.Font subtitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD);
                Paragraph metricasTitulo = new Paragraph("Métricas de Atención", subtitleFont);
                metricasTitulo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                metricasTitulo.SpacingAfter = 10;
                document.Add(metricasTitulo);

                PdfPTable metricasTable = new PdfPTable(2);
                metricasTable.WidthPercentage = 100;
                metricasTable.SpacingAfter = 20;

                // Cabecera de la tabla de métricas
                PdfPCell headerCell1 = new PdfPCell(new Phrase("Métrica", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
                headerCell1.BackgroundColor = new BaseColor(220, 220, 220); // Light gray
                headerCell1.Padding = 5;

                PdfPCell headerCell2 = new PdfPCell(new Phrase("Valor", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD)));
                headerCell2.BackgroundColor = new BaseColor(220, 220, 220); // Light gray
                headerCell2.Padding = 5;

                metricasTable.AddCell(headerCell1);
                metricasTable.AddCell(headerCell2);

                // Datos de métricas
                AddRowToTable(metricasTable, "Tiempo promedio solicitud a pedido", $"{metricas.PromedioHorasSolicitudPedido:N1} horas", normalFont);
                AddRowToTable(metricasTable, "Tiempo promedio pedido a entrega", $"{metricas.PromedioHorasPedidoEntrega:N1} horas", normalFont);
                AddRowToTable(metricasTable, "Tiempo promedio total (completados)", $"{metricas.PromedioHorasTotalesCompletados:N1} horas", normalFont);
                AddRowToTable(metricasTable, "Tiempo promedio total (todos)", $"{metricas.PromedioHorasTotalesTodos:N1} horas", normalFont);
                AddRowToTable(metricasTable, "Total de solicitudes", metricas.TotalSolicitudes.ToString(), normalFont);
                AddRowToTable(metricasTable, "Solicitudes completadas", $"{metricas.SolicitudesCompletadas} ({metricas.PorcentajeCompletado:N1}%)", normalFont);

                document.Add(metricasTable);

                // Tabla de atención por cliente
                Paragraph clientesTitulo = new Paragraph("Atención por Cliente", subtitleFont);
                clientesTitulo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                clientesTitulo.SpacingAfter = 10;
                document.Add(clientesTitulo);

                PdfPTable clientesTable = new PdfPTable(5);
                clientesTable.WidthPercentage = 100;
                float[] anchosClientes = new float[] { 2f, 1f, 1f, 1f, 1f };
                clientesTable.SetWidths(anchosClientes);
                clientesTable.SpacingAfter = 20;

                // Cabecera de la tabla de clientes
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);
                BaseColor headerColor = new BaseColor(220, 220, 220); // Light gray

                AddHeaderToTable(clientesTable, "Cliente", headerFont, headerColor);
                AddHeaderToTable(clientesTable, "Solicitudes", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);
                AddHeaderToTable(clientesTable, "Completadas", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);
                AddHeaderToTable(clientesTable, "Hrs Pedido", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);
                AddHeaderToTable(clientesTable, "Hrs Total", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);

                // Filas de datos de clientes
                bool colorAlternado = false;
                foreach (var item in clientesData)
                {
                    BaseColor bgColor = colorAlternado
                        ? BaseColor.WHITE
                        : new BaseColor(245, 245, 245); // Very light gray

                    colorAlternado = !colorAlternado;

                    AddCellToTable(clientesTable, item.Cliente, normalFont, bgColor);
                    AddCellToTable(clientesTable, item.TotalSolicitudes.ToString(), normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AddCellToTable(clientesTable, $"{item.SolicitudesCompletadas} ({item.PorcentajeCompletadas:P0})", normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AddCellToTable(clientesTable, $"{item.PromedioHorasSolicitudPedido:N1}", normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AddCellToTable(clientesTable, $"{item.PromedioHorasTotales:N1}", normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                }

                document.Add(clientesTable);

                // Tabla de detalle de solicitudes
                Paragraph detalleTitulo = new Paragraph("Detalle de Solicitudes", subtitleFont);
                detalleTitulo.Alignment = iTextSharp.text.Element.ALIGN_LEFT;
                detalleTitulo.SpacingAfter = 10;
                document.Add(detalleTitulo);

                PdfPTable detalleTable = new PdfPTable(7);
                detalleTable.WidthPercentage = 100;
                float[] anchosDetalle = new float[] { 0.5f, 1.5f, 2f, 1f, 1f, 1f, 1f };
                detalleTable.SetWidths(anchosDetalle);

                // Cabecera de la tabla de detalle
                AddHeaderToTable(detalleTable, "#", headerFont, headerColor);
                AddHeaderToTable(detalleTable, "Cliente", headerFont, headerColor);
                AddHeaderToTable(detalleTable, "Descripción", headerFont, headerColor);
                AddHeaderToTable(detalleTable, "Estado", headerFont, headerColor);
                AddHeaderToTable(detalleTable, "Hrs Pedido", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);
                AddHeaderToTable(detalleTable, "Hrs Total", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);
                AddHeaderToTable(detalleTable, "Completada", headerFont, headerColor, iTextSharp.text.Element.ALIGN_CENTER);

                // Limitar a 30 registros para evitar PDFs demasiado grandes
                var solicitudesLimitadas = detalleData.Take(30).ToList();

                // Filas de datos de detalle
                colorAlternado = false;
                foreach (var item in solicitudesLimitadas)
                {
                    BaseColor bgColor = colorAlternado
                        ? BaseColor.WHITE
                        : new BaseColor(245, 245, 245); // Very light gray

                    colorAlternado = !colorAlternado;

                    // Limitar la descripción a 50 caracteres para que quepa en el PDF
                    string descripcionCorta = item.DescripcionSolicitud;
                    if (descripcionCorta.Length > 50)
                        descripcionCorta = descripcionCorta.Substring(0, 47) + "...";

                    AddCellToTable(detalleTable, item.IdSolicitud.ToString(), normalFont, bgColor);
                    AddCellToTable(detalleTable, item.Cliente, normalFont, bgColor);
                    AddCellToTable(detalleTable, descripcionCorta, normalFont, bgColor);
                    AddCellToTable(detalleTable, item.Estado, normalFont, bgColor);
                    AddCellToTable(detalleTable, $"{item.HorasHastaPedido:N1}", normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AddCellToTable(detalleTable, $"{item.HorasTotales:N1}", normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AddCellToTable(detalleTable, item.Completado, normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                }

                document.Add(detalleTable);

                // Si hay más de 30 registros, agregar nota
                if (detalleData.Count > 30)
                {
                    iTextSharp.text.Font noteFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.ITALIC);
                    noteFont.Color = BaseColor.GRAY;

                    Paragraph nota = new Paragraph($"Nota: Se muestran solo las primeras 30 solicitudes de un total de {detalleData.Count}.", noteFont);
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

    // Clase para el gráfico de tiempos de atención
    public class GraficoTiemposAtencion : IDrawable
    {
        private ResumenAtencion _metricas;
        private List<AtencionCliente> _datosClientes;
        private float y3;

        public GraficoTiemposAtencion(ResumenAtencion metricas, List<AtencionCliente> datosClientes)
        {
            _metricas = metricas;
            _datosClientes = datosClientes;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (_metricas == null)
                return;

            // Configuración del gráfico
            float width = dirtyRect.Width;
            float height = dirtyRect.Height;
            float margenIzquierdo = 60;
            float margenDerecho = 20;
            float margenSuperior = 20;
            float margenInferior = 40;

            float areaGraficoAncho = width - margenIzquierdo - margenDerecho;
            float areaGraficoAlto = height - margenSuperior - margenInferior;

            // Determinar la escala para el eje Y
            double maxHoras = Math.Max(
                Math.Max(_metricas.PromedioHorasSolicitudPedido, _metricas.PromedioHorasPedidoEntrega),
                _metricas.PromedioHorasTotalesCompletados);

            // Redondear hacia arriba para tener un número entero como máximo
            maxHoras = Math.Ceiling(maxHoras / 10) * 10;
            maxHoras = Math.Max(maxHoras, 10); // Asegurar un mínimo

            // Dibujar ejes
            canvas.StrokeColor = Colors.Gray;
            canvas.StrokeSize = 1;

            // Eje Y
            canvas.DrawLine(margenIzquierdo, margenSuperior, margenIzquierdo, height - margenInferior);
            // Eje X
            canvas.DrawLine(margenIzquierdo, height - margenInferior, width - margenDerecho, height - margenInferior);

            // Escala en eje Y (intervalo de 10 horas)
            for (int i = 0; i <= maxHoras; i += 10)
            {
                float y = height - margenInferior - (float)(i * areaGraficoAlto / maxHoras);

                // Línea de guía
                canvas.StrokeColor = Colors.LightGray;
                canvas.StrokeSize = 1;
                canvas.DrawLine(margenIzquierdo - 5, y, width - margenDerecho, y);

                // Texto de la hora
                canvas.FontColor = Colors.Gray;
                canvas.FontSize = 10;
                canvas.DrawString($"{i}h", margenIzquierdo - 25, y, HorizontalAlignment.Center);
            }

            // Dibujar barras
            float anchoGrupo = areaGraficoAncho / 3;
            float anchoBarra = anchoGrupo * 0.6f;

            // Colores para las barras
            Color solicitudPedidoColor = Colors.DodgerBlue;
            Color pedidoEntregaColor = Colors.Orange;
            Color totalColor = Colors.Green;

            // Dibujar las barras
            // Barra 1: Solicitud a Pedido
            float x1 = margenIzquierdo + (anchoGrupo - anchoBarra) / 2;
            float altura1 = (float)(_metricas.PromedioHorasSolicitudPedido * areaGraficoAlto / maxHoras);
            float y1 = height - margenInferior - altura1;

            canvas.FillColor = solicitudPedidoColor;
            canvas.FillRectangle(x1, y1, anchoBarra, altura1);

            // Barra 2: Pedido a Entrega
            float x2 = x1 + anchoGrupo;
            float altura2 = (float)(_metricas.PromedioHorasPedidoEntrega * areaGraficoAlto / maxHoras);
            float y2 = height - margenInferior - altura2;

            canvas.FillColor = pedidoEntregaColor;
            canvas.FillRectangle(x2, y2, anchoBarra, altura2);

            // Barra 3: Total
            float x3 = x2 + anchoGrupo;
            float altura3 = (float)(_metricas.PromedioHorasTotalesCompletados * areaGraficoAlto / maxHoras);

            canvas.FillColor = totalColor;
            canvas.FillRectangle(x3, y3, anchoBarra, altura3);

            // Etiquetas de las barras
            canvas.FontColor = Colors.Black;
            canvas.FontSize = 10;

            canvas.DrawString($"{_metricas.PromedioHorasSolicitudPedido:N1}h", x1 + anchoBarra / 2, y1 - 15, HorizontalAlignment.Center);
            canvas.DrawString($"{_metricas.PromedioHorasPedidoEntrega:N1}h", x2 + anchoBarra / 2, y2 - 15, HorizontalAlignment.Center);
            canvas.DrawString($"{_metricas.PromedioHorasTotalesCompletados:N1}h", x3 + anchoBarra / 2, y3 - 15, HorizontalAlignment.Center);

            // Etiquetas del eje X
            canvas.DrawString("Sol. → Pedido", x1 + anchoBarra / 2, height - margenInferior + 15, HorizontalAlignment.Center);
            canvas.DrawString("Pedido → Entrega", x2 + anchoBarra / 2, height - margenInferior + 15, HorizontalAlignment.Center);
            canvas.DrawString("Total", x3 + anchoBarra / 2, height - margenInferior + 15, HorizontalAlignment.Center);

            // Título del eje Y
            canvas.SaveState();
            canvas.Rotate(-90, margenIzquierdo - 40, height / 2);
            canvas.DrawString("Horas", margenIzquierdo - 40, height / 2, HorizontalAlignment.Center);
            canvas.RestoreState();

            // Título del gráfico
            canvas.FontSize = 12;
            canvas.DrawString("Tiempos Promedio de Atención de Solicitudes", width / 2, margenSuperior - 5, HorizontalAlignment.Center);
        }
    }
}
