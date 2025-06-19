using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Maui.Controls;

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

        public VistaReportePedidosCliente(int idCliente, int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idCliente = idCliente;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;

            CargarTiposPedido();
            CargarReporte();
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
        private async void CargarReporte(string tipoPedido = null)
        {
            try
            {
                LoadingOverlay.IsVisible = true;

                string tipoSeleccionado = tipoPedido;
                if (tipoPedido == "Todos los tipos" || string.IsNullOrEmpty(tipoPedido))
                {
                    tipoSeleccionado = null;
                }

                var resultado = await Task.Run(() => App.Database.ObtenerReportePedidosPorCliente(_idCliente, tipoSeleccionado));
                _resumenPedidos = resultado.ResumenPedidos;
                _detallePedidos = resultado.DetallePedidos;

                // Depuración: imprimir las columnas y filas
                Console.WriteLine($"Filas en resumen: {_resumenPedidos?.Rows.Count ?? 0}");
                Console.WriteLine($"Filas en detalle: {_detallePedidos?.Rows.Count ?? 0}");

                // Crear objetos anónimos con la propiedad Row para alternar colores
                var resumenItems = _resumenPedidos.AsEnumerable()
                    .Select((row, index) => new {
                        EstadoPedido = row["EstadoPedido"]?.ToString(),
                        CantidadPedidos = Convert.ToInt32(row["CantidadPedidos"]),
                        VolumenTotal = Convert.ToInt32(row["VolumenTotal"]),
                        PromedioViajes = _resumenPedidos.Columns.Contains("PromedioViajes")
                            ? Convert.ToDouble(row["PromedioViajes"])
                            : 1.0, // Valor predeterminado si no existe la columna
                        Row = index % 2 == 0
                    }).ToList();

                // Procesar directamente la tabla de detalle
                // Asegúrate de que los nombres de las columnas coincidan con los de la consulta SQL
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

                // Configurar las colecciones de datos
                resumenCollectionView.ItemsSource = resumenItems;
                detalleCollectionView.ItemsSource = detalleItems;

                // Depuración: verificar si hay elementos en la lista de detalle
                Console.WriteLine($"Elementos en detalleItems: {detalleItems.Count}");

                // Actualizar resumen
                ActualizarResumen();
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
        }

        private void Btn_atras(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void TipoPedido_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tipoPedidoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
            CargarReporte(tipoPedidoSeleccionado);
        }

        private async void ActualizarReporte_Clicked(object sender, EventArgs e)
        {
            string tipoPedidoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
            CargarReporte(tipoPedidoSeleccionado);
        }

        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                // Generar el PDF en segundo plano para no bloquear la UI
                byte[] pdfBytes = await Task.Run(() => GenerarPDF());

                // Guardar el PDF en el directorio de caché de la aplicación
                string nombreArchivo = $"PedidosCliente_{_idCliente}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);

                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Mostrar mensaje de éxito y abrir el archivo
                await DisplayAlert("Éxito", $"Reporte exportado a PDF correctamente.", "OK");

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
                string nombreArchivo = $"PedidosCliente_{_idCliente}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);
                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Compartir el archivo usando el selector nativo de compartir
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Pedidos",
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
                // Configurar documento
                iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 36, 36, 36, 36);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms);
                document.Open();

                // Fuentes
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                iTextSharp.text.Font subtitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD);
                iTextSharp.text.BaseColor headerColor = new iTextSharp.text.BaseColor(220, 220, 220);

                // Añadir título
                iTextSharp.text.Paragraph titulo = new iTextSharp.text.Paragraph("Reporte de Pedidos", titleFont);
                titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                titulo.SpacingAfter = 20;
                document.Add(titulo);

                // Añadir información del cliente y fecha
                document.Add(new iTextSharp.text.Paragraph($"Cliente ID: {_idCliente}", normalFont));
                document.Add(new iTextSharp.text.Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", normalFont));
                string tipoSeleccionado = tipoPedidoPicker.SelectedItem?.ToString();
                document.Add(new iTextSharp.text.Paragraph($"Tipo de pedido: {tipoSeleccionado ?? "Todos"}", normalFont));
                document.Add(new iTextSharp.text.Paragraph(" "));

                // Añadir resumen general
                document.Add(new iTextSharp.text.Paragraph("Resumen General", subtitleFont));

                iTextSharp.text.pdf.PdfPTable resumenTable = new iTextSharp.text.pdf.PdfPTable(2);
                resumenTable.WidthPercentage = 100;
                resumenTable.SpacingAfter = 20;

                // Cabecera de la tabla resumen
                iTextSharp.text.pdf.PdfPCell headerCell1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Métrica", headerFont));
                headerCell1.BackgroundColor = headerColor;
                headerCell1.Padding = 5;

                iTextSharp.text.pdf.PdfPCell headerCell2 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Valor", headerFont));
                headerCell2.BackgroundColor = headerColor;
                headerCell2.Padding = 5;

                resumenTable.AddCell(headerCell1);
                resumenTable.AddCell(headerCell2);

                // Datos del resumen
                int totalPedidos = _resumenPedidos.AsEnumerable().Sum(r => Convert.ToInt32(r["CantidadPedidos"]));
                int volumenTotal = _resumenPedidos.AsEnumerable().Sum(r => Convert.ToInt32(r["VolumenTotal"]));

                AgregarFilaTabla(resumenTable, "Total de Pedidos", totalPedidos.ToString(), normalFont);
                AgregarFilaTabla(resumenTable, "Volumen Total", volumenTotal.ToString() + " L", normalFont);

                document.Add(resumenTable);

                // Añadir resumen por estado
                document.Add(new iTextSharp.text.Paragraph("Resumen por Estado de Pedido", subtitleFont));
                document.Add(new iTextSharp.text.Paragraph(" "));

                // Tabla resumen por estado
                iTextSharp.text.pdf.PdfPTable estadoTable = new iTextSharp.text.pdf.PdfPTable(4);
                estadoTable.WidthPercentage = 100;
                float[] anchosEstado = new float[] { 3f, 1f, 1f, 1f };
                estadoTable.SetWidths(anchosEstado);
                estadoTable.SpacingAfter = 20;

                // Cabecera de la tabla de estados
                AgregarCeldaEncabezado(estadoTable, "Estado de Pedido", headerFont, headerColor);
                AgregarCeldaEncabezado(estadoTable, "Cantidad", headerFont, headerColor);
                AgregarCeldaEncabezado(estadoTable, "Volumen", headerFont, headerColor);
                AgregarCeldaEncabezado(estadoTable, "Viajes Prom.", headerFont, headerColor);

                // Filas de datos de estados
                bool colorAlternadoResumen = false;
                foreach (System.Data.DataRow row in _resumenPedidos.Rows)
                {
                    iTextSharp.text.BaseColor bgColor = colorAlternadoResumen ?
                        new iTextSharp.text.BaseColor(245, 245, 245) : iTextSharp.text.BaseColor.WHITE;
                    colorAlternadoResumen = !colorAlternadoResumen;

                    string estado = row["EstadoPedido"].ToString();
                    int cantidad = Convert.ToInt32(row["CantidadPedidos"]);
                    int volumen = Convert.ToInt32(row["VolumenTotal"]);
                    double promedio = _resumenPedidos.Columns.Contains("PromedioViajes") ?
                        Convert.ToDouble(row["PromedioViajes"]) : 1.0;

                    AgregarCeldaDetalle(estadoTable, estado, normalFont, bgColor, iTextSharp.text.Element.ALIGN_LEFT);
                    AgregarCeldaDetalle(estadoTable, cantidad.ToString(), normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AgregarCeldaDetalle(estadoTable, volumen.ToString() + " L", normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AgregarCeldaDetalle(estadoTable, promedio.ToString("F1"), normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                }

                document.Add(estadoTable);

                // Añadir detalle de pedidos
                document.Add(new iTextSharp.text.Paragraph("Detalle de Pedidos", subtitleFont));
                document.Add(new iTextSharp.text.Paragraph(" "));

                // Tabla de detalle
                iTextSharp.text.pdf.PdfPTable detalleTable = new iTextSharp.text.pdf.PdfPTable(6);
                detalleTable.WidthPercentage = 100;
                float[] anchos = new float[] { 0.5f, 3f, 1.5f, 1f, 1f, 1f };
                detalleTable.SetWidths(anchos);

                // Cabecera de la tabla detalle
                AgregarCeldaEncabezado(detalleTable, "#", headerFont, headerColor);
                AgregarCeldaEncabezado(detalleTable, "Origen → Destino", headerFont, headerColor);
                AgregarCeldaEncabezado(detalleTable, "Estado", headerFont, headerColor);
                AgregarCeldaEncabezado(detalleTable, "Cantidad", headerFont, headerColor);
                AgregarCeldaEncabezado(detalleTable, "Viajes", headerFont, headerColor);
                AgregarCeldaEncabezado(detalleTable, "Completado", headerFont, headerColor);

                // Filas de datos de detalle
                bool colorAlternadoDetalle = false;
                foreach (System.Data.DataRow row in _detallePedidos.Rows)
                {
                    iTextSharp.text.BaseColor bgColor = colorAlternadoDetalle ?
                        new iTextSharp.text.BaseColor(245, 245, 245) : iTextSharp.text.BaseColor.WHITE;
                    colorAlternadoDetalle = !colorAlternadoDetalle;

                    // Verificar y obtener valores según los nombres de columna disponibles
                    string id = row.Table.Columns.Contains("id_pedido") ? row["id_pedido"].ToString() :
                              (row.Table.Columns.Contains("pedido") ? row["pedido"].ToString() : "");

                    string origen = row.Table.Columns.Contains("origen") ? row["origen"]?.ToString() ?? "" : "";
                    string destino = row.Table.Columns.Contains("destino") ? row["destino"]?.ToString() ?? "" : "";
                    string origenDestino = $"{origen} → {destino}";

                    string estado = row.Table.Columns.Contains("estado") ? row["estado"]?.ToString() ?? "" : "";

                    string cantidad = row.Table.Columns.Contains("cantidad") ?
                                    row["cantidad"]?.ToString() ?? "0" : "0";

                    string viajes = row.Table.Columns.Contains("viajes") ? row["viajes"]?.ToString() ?? "1" :
                                   (row.Table.Columns.Contains("total_viajes") ? row["total_viajes"]?.ToString() ?? "1" : "1");

                    string completado = row.Table.Columns.Contains("completado") ? row["completado"]?.ToString() ?? "No" : "No";

                    AgregarCeldaDetalle(detalleTable, id, normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AgregarCeldaDetalle(detalleTable, origenDestino, normalFont, bgColor, iTextSharp.text.Element.ALIGN_LEFT);
                    AgregarCeldaDetalle(detalleTable, estado, normalFont, bgColor, iTextSharp.text.Element.ALIGN_LEFT);
                    AgregarCeldaDetalle(detalleTable, cantidad, normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AgregarCeldaDetalle(detalleTable, viajes, normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                    AgregarCeldaDetalle(detalleTable, completado, normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                }

                document.Add(detalleTable);

                // Pie de página
                iTextSharp.text.Font footerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10);
                footerFont.Color = iTextSharp.text.BaseColor.GRAY;

                iTextSharp.text.Paragraph footer = new iTextSharp.text.Paragraph($"Reporte generado por App Transporte • {DateTime.Now:dd/MM/yyyy HH:mm:ss}", footerFont);
                footer.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                footer.SpacingBefore = 20;
                document.Add(footer);

                document.Close();
                return ms.ToArray();
            }
        }


        private void AgregarFilaTabla(iTextSharp.text.pdf.PdfPTable table, string columna1, string columna2, iTextSharp.text.Font font)
        {
            iTextSharp.text.pdf.PdfPCell cell1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(columna1, font));
            cell1.Padding = 5;

            iTextSharp.text.pdf.PdfPCell cell2 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(columna2, font));
            cell2.Padding = 5;

            table.AddCell(cell1);
            table.AddCell(cell2);
        }

        private void AgregarCeldaEncabezado(iTextSharp.text.pdf.PdfPTable table, string texto, iTextSharp.text.Font font, iTextSharp.text.BaseColor color)
        {
            iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(texto, font));
            cell.BackgroundColor = color;
            cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private void AgregarCeldaDetalle(iTextSharp.text.pdf.PdfPTable table, string texto, iTextSharp.text.Font font, iTextSharp.text.BaseColor color, int alineacion)
        {
            iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(texto, font));
            cell.BackgroundColor = color;
            cell.HorizontalAlignment = alineacion;
            cell.Padding = 5;
            table.AddCell(cell);
        }
    }
}
