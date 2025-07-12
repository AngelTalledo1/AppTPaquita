using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Font = Microsoft.Maui.Graphics.Font;
using AppTransporte.model;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;

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
            // Funcionalidad temporalmente suspendida
            await DisplayAlert("Función en desarrollo", "La exportación a PDF se encuentra en desarrollo y estará disponible próximamente.", "OK");
            return;
        }

        private async void Compartir_Clicked(object sender, EventArgs e)
        {
            // Funcionalidad temporalmente suspendida
            await DisplayAlert("Función en desarrollo", "La funcionalidad de compartir PDF se encuentra en desarrollo y estará disponible próximamente.", "OK");
            return;
        }

        /*
        public byte[] GenerarReporteServiciosPDF(List<ReporteServicio> reporteData, DateTime fechaInicio, DateTime fechaFin)
        {
            // Funcionalidad temporalmente suspendida
            throw new NotImplementedException("Esta funcionalidad se encuentra en desarrollo");
            
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Helvetica"));

                    page.Content().Column(column =>
                    {
                        // Título del documento
                        column.Item().Text("Reporte de Servicios")
                            .FontSize(18)
                            .Bold()
                            .AlignCenter();

                        column.Item().PaddingTop(20);

                        // Información del período
                        column.Item().Text($"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}")
                            .FontSize(12);

                        column.Item().PaddingTop(20);

                        // Título de resumen
                        column.Item().Text("Resumen")
                            .FontSize(14)
                            .Bold();

                        column.Item().PaddingTop(10);

                        // Calcular totales para el resumen
                        int totalServicios = reporteData.Count;
                        int totalPedidos = reporteData.Sum(r => r.CantidadPedidos);
                        int volumenTotal = reporteData.Sum(r => r.VolumenTransportado);

                        // Tabla de resumen
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                            });

                            // Encabezados de la tabla de resumen
                            table.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
                                .Padding(5)
                                .Text("Descripción")
                                .Bold();

                            table.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
                                .Padding(5)
                                .Text("Valor")
                                .Bold();

                            // Filas de datos del resumen
                            table.Cell().Padding(5).Text("Total de Servicios");
                            table.Cell().Padding(5).Text(totalServicios.ToString());

                            table.Cell().Padding(5).Text("Total de Pedidos");
                            table.Cell().Padding(5).Text(totalPedidos.ToString());

                            table.Cell().Padding(5).Text("Volumen Total Transportado");
                            table.Cell().Padding(5).Text($"{volumenTotal:N0} L");
                        });

                        column.Item().PaddingTop(20);

                        // Título de detalle
                        column.Item().Text("Detalle por Servicio")
                            .FontSize(14)
                            .Bold();

                        column.Item().PaddingTop(10);

                        // Tabla de detalle
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);   // Tipo de Servicio
                                columns.RelativeColumn(1.5f); // Pedidos
                                columns.RelativeColumn(2);   // Vol. Solicitado
                                columns.RelativeColumn(2);   // Vol. Transportado
                                columns.RelativeColumn(1.5f); // % Cumplimiento
                            });

                            // Encabezados de la tabla de detalle
                            table.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
                                .Padding(5)
                                .Text("Tipo de Servicio")
                                .Bold();

                            table.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
                                .Padding(5)
                                .AlignCenter()
                                .Text("Pedidos")
                                .Bold();

                            table.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
                                .Padding(5)
                                .AlignCenter()
                                .Text("Vol. Solicitado")
                                .Bold();

                            table.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
                                .Padding(5)
                                .AlignCenter()
                                .Text("Vol. Transportado")
                                .Bold();

                            table.Cell().Background(QuestPDF.Helpers.Colors.Grey.Lighten3)
                                .Padding(5)
                                .AlignCenter()
                                .Text("% Cumplimiento")
                                .Bold();

                            // Filas de datos
                            bool colorAlternado = false;
                            foreach (var item in reporteData)
                            {
                                var backgroundColor = colorAlternado ? QuestPDF.Helpers.Colors.Grey.Lighten4 : QuestPDF.Helpers.Colors.White;
                                colorAlternado = !colorAlternado;

                                // Tipo de Servicio
                                table.Cell().Background(backgroundColor)
                                    .Padding(5)
                                    .Text(item.TipoServicio);

                                // Cantidad de Pedidos
                                table.Cell().Background(backgroundColor)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text(item.CantidadPedidos.ToString());

                                // Volumen Solicitado
                                table.Cell().Background(backgroundColor)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text($"{item.VolumenSolicitado:N0}");

                                // Volumen Transportado
                                table.Cell().Background(backgroundColor)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text($"{item.VolumenTransportado:N0}");

                                // Porcentaje de Cumplimiento
                                table.Cell().Background(backgroundColor)
                                    .Padding(5)
                                    .AlignCenter()
                                    .Text($"{item.PorcentajeCumplimiento:N2}%");
                            }
                        });
                    });

                    // Pie de página
                    page.Footer().AlignRight().Text($"Reporte generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                        .FontSize(10)
                        .FontColor(QuestPDF.Helpers.Colors.Grey.Medium);
                });
            });

            return document.GeneratePdf();
        }
        */
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
            canvas.StrokeColor = Microsoft.Maui.Graphics.Colors.Gray;
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
                    canvas.FillColor = Microsoft.Maui.Graphics.Colors.Green;
                else if (porcentaje >= 0.5f)
                    canvas.FillColor = Microsoft.Maui.Graphics.Colors.Orange;
                else
                    canvas.FillColor = Microsoft.Maui.Graphics.Colors.Red;

                // Dibujar barra
                canvas.FillRectangle(x, y, anchoBarras, altoBarra);

                // Dibujar etiqueta del servicio
                canvas.FontColor = Microsoft.Maui.Graphics.Colors.Black;
                canvas.FontSize = 10;

                // Etiqueta de servicio abreviada
                string etiqueta = servicio.TipoServicio;
                if (etiqueta.Length > 10)
                    etiqueta = etiqueta.Substring(0, 7) + "...";

                // Rotar texto para etiquetas en eje X
                canvas.SaveState();
                canvas.Rotate(-45, x + anchoBarras / 2, height - margenInferior + 5);
                canvas.DrawString(etiqueta, x, height - margenInferior + 5, Microsoft.Maui.Graphics.HorizontalAlignment.Left);
                canvas.RestoreState();

                // Porcentaje encima de la barra
                canvas.DrawString($"{servicio.PorcentajeCumplimiento:N0}%", x + anchoBarras / 2, y - 15, Microsoft.Maui.Graphics.HorizontalAlignment.Center);
            }

            // Dibujar título del eje Y
            canvas.SaveState();
            canvas.Rotate(-90, margenIzquierdo - 25, height / 2);
            canvas.DrawString("% Cumplimiento", margenIzquierdo - 25, height / 2, Microsoft.Maui.Graphics.HorizontalAlignment.Center);
            canvas.RestoreState();

            // Escala en eje Y (0%, 25%, 50%, 75%, 100%)
            for (int i = 0; i <= 4; i++)
            {
                float porcentaje = i * 25;
                float y = height - margenInferior - (areaGraficoAlto * (porcentaje / 100f));

                // Línea de guía
                canvas.StrokeColor = Microsoft.Maui.Graphics.Colors.LightGray;
                canvas.StrokeSize = 1;
                canvas.DrawLine(margenIzquierdo - 5, y, width - margenDerecho, y);

                // Texto del porcentaje
                canvas.FontColor = Microsoft.Maui.Graphics.Colors.Gray;
                canvas.FontSize = 10;
                canvas.DrawString($"{porcentaje}%", margenIzquierdo - 25, y, Microsoft.Maui.Graphics.HorizontalAlignment.Center);
            }
        }
    }
}