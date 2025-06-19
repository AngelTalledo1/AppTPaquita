// AppTransporte/Interfaces/VistaReporteTrabajosTrabajadorCliente.xaml.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using AppTransporte.model;
using Microsoft.Maui.Controls;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
                // Verificar si hay datos para exportar
                if (_resumenTipado == null || _resumenTipado.Count == 0)
                {
                    await DisplayAlert("Advertencia", "No hay datos para exportar", "OK");
                    return;
                }

                // Generar el PDF
                byte[] pdfBytes = GenerarPDF();

                // Guardar el PDF en el directorio de caché de la aplicación
                string nombreArchivo = $"Trabajador_Viajes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);

                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Mostrar mensaje de éxito
                await DisplayAlert("Éxito", "Reporte exportado a PDF correctamente", "OK");

                // Abrir el archivo
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(rutaArchivo)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al exportar el reporte: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al exportar PDF: {ex.Message}");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private byte[] GenerarPDF()
        {
            try
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

                    // Título
                    iTextSharp.text.Paragraph titulo = new iTextSharp.text.Paragraph("Reporte de Trabajos por Trabajador", titleFont);
                    titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    titulo.SpacingAfter = 20;
                    document.Add(titulo);

                    // Información del cliente
                    document.Add(new iTextSharp.text.Paragraph($"Cliente ID: {_idCliente}", normalFont));
                    document.Add(new iTextSharp.text.Paragraph($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}", normalFont));
                    document.Add(new iTextSharp.text.Paragraph(" "));

                    // Si hay datos de resumen, mostrar información del trabajador
                    if (_resumenTipado != null && _resumenTipado.Count > 0)
                    {
                        var trabajador = _resumenTipado[0];

                        // Datos del trabajador
                        document.Add(new iTextSharp.text.Paragraph("Datos del Trabajador", subtitleFont));
                        document.Add(new iTextSharp.text.Paragraph(" "));

                        iTextSharp.text.pdf.PdfPTable tablaTrabajador = new iTextSharp.text.pdf.PdfPTable(2);
                        tablaTrabajador.WidthPercentage = 100;
                        float[] anchosTrabajador = new float[] { 1f, 2f };
                        tablaTrabajador.SetWidths(anchosTrabajador);
                        tablaTrabajador.SpacingAfter = 20;

                        // Encabezados de la tabla trabajador
                        iTextSharp.text.pdf.PdfPCell cellHeader1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Campo", headerFont));
                        cellHeader1.BackgroundColor = headerColor;
                        cellHeader1.Padding = 5;

                        iTextSharp.text.pdf.PdfPCell cellHeader2 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Valor", headerFont));
                        cellHeader2.BackgroundColor = headerColor;
                        cellHeader2.Padding = 5;

                        tablaTrabajador.AddCell(cellHeader1);
                        tablaTrabajador.AddCell(cellHeader2);

                        // Datos
                        AgregarFilaTabla(tablaTrabajador, "Nombre", trabajador.NombreCompleto, normalFont);
                        AgregarFilaTabla(tablaTrabajador, "Categoría", trabajador.Categoria, normalFont);
                        AgregarFilaTabla(tablaTrabajador, "Licencia", trabajador.NumLicencia ?? "N/A", normalFont);
                        document.Add(tablaTrabajador);

                        // Estadísticas
                        document.Add(new iTextSharp.text.Paragraph("Resumen de Actividad", subtitleFont));
                        document.Add(new iTextSharp.text.Paragraph(" "));

                        iTextSharp.text.pdf.PdfPTable tablaEstadisticas = new iTextSharp.text.pdf.PdfPTable(2);
                        tablaEstadisticas.WidthPercentage = 100;
                        tablaEstadisticas.SpacingAfter = 20;

                        // Encabezados de la tabla estadísticas
                        iTextSharp.text.pdf.PdfPCell cellHeaderEst1 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Métrica", headerFont));
                        cellHeaderEst1.BackgroundColor = headerColor;
                        cellHeaderEst1.Padding = 5;

                        iTextSharp.text.pdf.PdfPCell cellHeaderEst2 = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase("Valor", headerFont));
                        cellHeaderEst2.BackgroundColor = headerColor;
                        cellHeaderEst2.Padding = 5;

                        tablaEstadisticas.AddCell(cellHeaderEst1);
                        tablaEstadisticas.AddCell(cellHeaderEst2);

                        // Datos estadísticos
                        AgregarFilaTabla(tablaEstadisticas, "Total de viajes realizados", trabajador.TotalViajesRealizados.ToString(), normalFont);
                        AgregarFilaTabla(tablaEstadisticas, "Total de pedidos atendidos", trabajador.TotalPedidosAtendidos.ToString(), normalFont);
                        AgregarFilaTabla(tablaEstadisticas, "Volumen total transportado", $"{trabajador.VolumenTotalTransportado:N2} L", normalFont);

                        document.Add(tablaEstadisticas);
                    }

                    // Si hay detalles, mostrar tabla de viajes
                    if (_detalleTipado != null && _detalleTipado.Count > 0)
                    {
                        document.Add(new iTextSharp.text.Paragraph("Detalle de Viajes", subtitleFont));
                        document.Add(new iTextSharp.text.Paragraph(" "));

                        iTextSharp.text.pdf.PdfPTable tablaViajes = new iTextSharp.text.pdf.PdfPTable(6);
                        tablaViajes.WidthPercentage = 100;
                        float[] anchosViajes = new float[] { 0.7f, 1.2f, 2f, 1.5f, 1f, 1f };
                        tablaViajes.SetWidths(anchosViajes);

                        // Encabezados de la tabla de viajes
                        AgregarCeldaEncabezado(tablaViajes, "ID", headerFont, headerColor);
                        AgregarCeldaEncabezado(tablaViajes, "Fecha", headerFont, headerColor);
                        AgregarCeldaEncabezado(tablaViajes, "Origen ? Destino", headerFont, headerColor);
                        AgregarCeldaEncabezado(tablaViajes, "Vehículos", headerFont, headerColor);
                        AgregarCeldaEncabezado(tablaViajes, "Estado", headerFont, headerColor);
                        AgregarCeldaEncabezado(tablaViajes, "Volumen", headerFont, headerColor);

                        // Datos de viajes
                        bool colorAlternado = false;
                        foreach (var viaje in _detalleTipado)
                        {
                            iTextSharp.text.BaseColor bgColor = colorAlternado ?
                                new iTextSharp.text.BaseColor(245, 245, 245) : iTextSharp.text.BaseColor.WHITE;
                            colorAlternado = !colorAlternado;

                            AgregarCeldaDetalle(tablaViajes, viaje.IdViaje.ToString(), normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                            AgregarCeldaDetalle(tablaViajes, viaje.FechaProgramada.ToString("dd/MM/yyyy"), normalFont, bgColor, iTextSharp.text.Element.ALIGN_CENTER);
                            AgregarCeldaDetalle(tablaViajes, $"{viaje.Origen} ? {viaje.Destino}", normalFont, bgColor, iTextSharp.text.Element.ALIGN_LEFT);

                            // Columna de vehículos (tracto y cisterna)
                            iTextSharp.text.pdf.PdfPCell cellVehiculos = new iTextSharp.text.pdf.PdfPCell();
                            cellVehiculos.BackgroundColor = bgColor;
                            cellVehiculos.Padding = 5;

                            iTextSharp.text.Paragraph pVehiculos = new iTextSharp.text.Paragraph();
                            pVehiculos.Add(new iTextSharp.text.Phrase("T: " + (string.IsNullOrEmpty(viaje.Tracto) ? "N/A" : viaje.Tracto), normalFont));
                            pVehiculos.Add(iTextSharp.text.Chunk.NEWLINE);
                            pVehiculos.Add(new iTextSharp.text.Phrase("C: " + (string.IsNullOrEmpty(viaje.Cisterna) ? "N/A" : viaje.Cisterna), normalFont));

                            cellVehiculos.AddElement(pVehiculos);
                            tablaViajes.AddCell(cellVehiculos);

                            AgregarCeldaDetalle(tablaViajes, string.IsNullOrEmpty(viaje.UltimoEstado) ? "Sin estado" : viaje.UltimoEstado, normalFont, bgColor, iTextSharp.text.Element.ALIGN_LEFT);
                            AgregarCeldaDetalle(tablaViajes, $"{viaje.Volumen:N2} L", normalFont, bgColor, iTextSharp.text.Element.ALIGN_RIGHT);
                        }

                        document.Add(tablaViajes);
                    }

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
            catch (iTextSharp.text.DocumentException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error de iTextSharp al generar PDF: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error general al generar PDF: {ex.Message}");
                throw;
            }
        }

        private void AgregarFilaTabla(iTextSharp.text.pdf.PdfPTable table, string etiqueta, string valor, iTextSharp.text.Font font)
        {
            iTextSharp.text.pdf.PdfPCell cellEtiqueta = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(etiqueta, font));
            cellEtiqueta.Padding = 5;

            iTextSharp.text.pdf.PdfPCell cellValor = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(valor, font));
            cellValor.Padding = 5;

            table.AddCell(cellEtiqueta);
            table.AddCell(cellValor);
        }

        private void AgregarCeldaEncabezado(iTextSharp.text.pdf.PdfPTable table, string text, iTextSharp.text.Font font, iTextSharp.text.BaseColor color)
        {
            iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(text, font));
            cell.BackgroundColor = color;
            cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private void AgregarCeldaDetalle(iTextSharp.text.pdf.PdfPTable table, string text, iTextSharp.text.Font font, iTextSharp.text.BaseColor color, int alineacion)
        {
            iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(text, font));
            cell.BackgroundColor = color;
            cell.HorizontalAlignment = alineacion;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private async void Compartir_Clicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                // Verificar si hay datos para compartir
                if (_resumenTipado == null || _resumenTipado.Count == 0)
                {
                    await DisplayAlert("Advertencia", "No hay datos para compartir", "OK");
                    return;
                }

                // Generar el PDF
                byte[] pdfBytes = GenerarPDF();

                // Guardar temporalmente el PDF
                string nombreArchivo = $"Trabajador_Viajes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);
                File.WriteAllBytes(rutaArchivo, pdfBytes);

                // Compartir el archivo
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Trabajos por Trabajador",
                    File = new ShareFile(rutaArchivo)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al compartir el reporte: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al compartir PDF: {ex.Message}");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }
    }
}
