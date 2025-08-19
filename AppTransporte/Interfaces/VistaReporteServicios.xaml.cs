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
            LoadingOverlay.IsVisible = true;

            try
            {
                // Verificar que tengamos datos para exportar
                if (_datosReporte == null || !_datosReporte.Any())
                {
                    await DisplayAlert("Sin datos",
                        "No hay datos disponibles para exportar. Actualice el reporte primero.", "OK");
                    return;
                }

                // Validar fechas
                if (FechaInicio.Date > FechaFin.Date)
                {
                    await DisplayAlert("Error de fechas", "La fecha de inicio no puede ser posterior a la fecha fin", "OK");
                    return;
                }

                // Mostrar opciones al usuario
                string action = await DisplayActionSheet(
                    "¿Qué desea hacer con el PDF?",
                    "Cancelar",
                    null,
                    "Guardar y abrir",
                    "Solo guardar",
                    "Compartir");

                if (action == "Cancelar")
                    return;

                // Generar el PDF
                byte[] pdfBytes = await Task.Run(async () =>
                    await App.Database.GenerarReporteServiciosPDF(
                        _datosReporte,
                        FechaInicio.Date,
                        FechaFin.Date));

                // Crear el nombre del archivo
                string fileName = $"Reporte_Servicios_{FechaInicio.Date:yyyyMMdd}_{FechaFin.Date:yyyyMMdd}.pdf";

                switch (action)
                {
                    case "Guardar y abrir":
                        await GuardarYAbrirPDFAsync(pdfBytes, fileName);
                        await DisplayAlert("Éxito", "El reporte PDF se ha generado y abierto correctamente.", "OK");
                        break;

                    case "Solo guardar":
                        await GuardarEnCarpetaDescargas(pdfBytes, fileName);
                        await DisplayAlert("Éxito", "El reporte PDF se ha guardado en la carpeta de descargas.", "OK");
                        break;

                    case "Compartir":
                        await CompartirPDFAsync(pdfBytes, fileName);
                        break;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al procesar el PDF: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error completo: {ex}");
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
                if (_datosReporte == null || !_datosReporte.Any())
                {
                    await DisplayAlert("Sin datos",
                        "No hay datos disponibles para compartir. Actualice el reporte primero.", "OK");
                    return;
                }

                // Generar el PDF
                byte[] pdfBytes = await Task.Run(async () =>
                    await App.Database.GenerarReporteServiciosPDF(
                        _datosReporte,
                        FechaInicio.Date,
                        FechaFin.Date));

                // Crear el nombre del archivo
                string fileName = $"Reporte_Servicios_{FechaInicio.Date:yyyyMMdd}_{FechaFin.Date:yyyyMMdd}.pdf";

                // Compartir el archivo
                await CompartirPDFAsync(pdfBytes, fileName);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al compartir el PDF: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al compartir PDF: {ex.Message}");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        // AGREGAR estos métodos auxiliares si no los tienes ya en la clase:

        private async Task GuardarYAbrirPDFAsync(byte[] pdfBytes, string fileName)
        {
            try
            {
                // Crear un archivo temporal
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                // Escribir los bytes al archivo
                await File.WriteAllBytesAsync(filePath, pdfBytes);

                // Abrir el archivo con la aplicación predeterminada
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath),
                    Title = "Abrir Reporte PDF"
                });
            }
            catch (Exception ex)
            {
                // Si no se puede abrir, al menos guardarlo en Downloads
                await GuardarEnCarpetaDescargas(pdfBytes, fileName);
                throw new Exception($"PDF guardado pero no se pudo abrir automáticamente: {ex.Message}");
            }
        }

        private async Task GuardarEnCarpetaDescargas(byte[] pdfBytes, string fileName)
        {
            try
            {
#if ANDROID
        // Android: Guardar en la carpeta Downloads
        var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
            Android.OS.Environment.DirectoryDownloads)?.AbsolutePath;
        
        if (!string.IsNullOrEmpty(downloadsPath))
        {
            string filePath = Path.Combine(downloadsPath, fileName);
            await File.WriteAllBytesAsync(filePath, pdfBytes);
            
            // Notificar al sistema que se agregó un archivo
            var mediaScanIntent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
            mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(filePath)));
            Platform.CurrentActivity?.SendBroadcast(mediaScanIntent);
        }
#elif IOS
        // iOS: Guardar en Documents
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string filePath = Path.Combine(documentsPath, fileName);
        await File.WriteAllBytesAsync(filePath, pdfBytes);
#else
                // Otras plataformas: usar carpeta de documentos
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(documentsPath, fileName);
                await File.WriteAllBytesAsync(filePath, pdfBytes);
#endif
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar en carpeta de descargas: {ex.Message}");
                throw;
            }
        }

        private async Task CompartirPDFAsync(byte[] pdfBytes, string fileName)
        {
            try
            {
                // Crear un archivo temporal
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                // Escribir los bytes al archivo
                await File.WriteAllBytesAsync(filePath, pdfBytes);

                // Compartir el archivo
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Reporte de Servicios",
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al compartir el archivo: {ex.Message}");
            }
        }

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
}