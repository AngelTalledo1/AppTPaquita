using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using SyncSizeF = Syncfusion.Drawing.SizeF;
using Microsoft.Maui.Storage;
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
            _ = CargarDatosReporte();
        }

        private async void Btn_atras(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
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

                // Actualizar la colección de datos en el hilo principal
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    resumenCollectionView.ItemsSource = _datosResumen;
                    detalleCollectionView.ItemsSource = _datosDetalle;

                    // Actualizar etiquetas de resumen
                    ActualizarResumen();

                    // Actualizar gráfico
                    ActualizarGrafico();
                });
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
                        EstadoPedido = fila["Estado del Pedido"]?.ToString() ?? "",
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
                        Cliente = fila["Cliente"]?.ToString() ?? "",
                        FechaSolicitud = Convert.ToDateTime(fila["fecha_solicitud"]),
                        FechaEntrega = fila["fecha_entrega"] != DBNull.Value ? Convert.ToDateTime(fila["fecha_entrega"]) : (DateTime?)null,
                        Estado = fila["Estado"]?.ToString() ?? "",
                        Origen = fila["Origen"]?.ToString() ?? "",
                        Destino = fila["Destino"]?.ToString() ?? "",
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
                PedidosEntregadosLabel.Text = totalPedidos > 0
                    ? $"{pedidosEntregados:N0} ({(double)pedidosEntregados / totalPedidos * 100:N0}%)"
                    : "0 (0%)";
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
            try
            {
                // Implementar el dibujo del gráfico utilizando Microsoft.Maui.Graphics
                if (_datosResumen != null && _datosResumen.Any())
                {
                    graficoView.Drawable = new GraficoPedidos(_datosResumen);
                    graficoView.IsVisible = true;
                }
                else
                {
                    // Si no hay datos, mostrar gráfico vacío con mensaje
                    graficoView.Drawable = new GraficoPedidos(new List<ResumenPedido>());
                    graficoView.IsVisible = true;
                }

                // Forzar redibujado
                graficoView.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar gráfico: {ex.Message}");
                // En caso de error, ocultar el gráfico
                graficoView.IsVisible = false;
            }
        }
        public class GraficoPedidos : IDrawable
        {
            private List<ResumenPedido> _datos;

            public GraficoPedidos(List<ResumenPedido> datos)
            {
                _datos = datos ?? new List<ResumenPedido>();
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                if (_datos == null || !_datos.Any())
                {
                    // Dibujar mensaje cuando no hay datos
                    canvas.FontColor = Colors.Gray;
                    canvas.FontSize = 14;
                    canvas.DrawString("No hay datos disponibles",
                        dirtyRect.Width / 2, dirtyRect.Height / 2,
                        HorizontalAlignment.Center);
                    return;
                }

                // Configuración del gráfico
                float width = dirtyRect.Width;
                float height = dirtyRect.Height;
                float margenIzquierdo = 20;
                float margenDerecho = 20;
                float margenSuperior = 30;
                float margenInferior = 80; // Más espacio para la leyenda

                float centroX = width / 2;
                float centroY = (height - margenInferior) / 2 + margenSuperior / 2;
                float radio = Math.Min(centroY - margenSuperior, (width - margenIzquierdo - margenDerecho) / 2) - 20;

                // Colores atractivos para el gráfico
                Microsoft.Maui.Graphics.Color[] colores = new Microsoft.Maui.Graphics.Color[]
                {
            Color.FromRgb(54, 162, 235),   // Azul
            Color.FromRgb(255, 99, 132),   // Rosa/Rojo
            Color.FromRgb(75, 192, 192),   // Verde agua
            Color.FromRgb(255, 205, 86),   // Amarillo
            Color.FromRgb(153, 102, 255),  // Morado
            Color.FromRgb(255, 159, 64),   // Naranja
            Color.FromRgb(199, 199, 199),  // Gris
            Color.FromRgb(83, 102, 255)    // Azul oscuro
                };

                // Total de pedidos
                int totalPedidos = _datos.Sum(d => d.CantidadPedidos);

                if (totalPedidos == 0)
                {
                    canvas.FontColor = Colors.Gray;
                    canvas.FontSize = 14;
                    canvas.DrawString("No hay pedidos en el período seleccionado",
                        dirtyRect.Width / 2, dirtyRect.Height / 2,
                        HorizontalAlignment.Center);
                    return;
                }

                // Dibujar título del gráfico
                canvas.FontColor = Color.FromRgb(203, 67, 53); // Color corporativo
                canvas.FontSize = 16;
                canvas.DrawString("Distribución de Pedidos por Estado",
                    width / 2, 15, HorizontalAlignment.Center);

                // Dibujar gráfico de torta
                float anguloInicial = -90; // Comenzar desde arriba

                for (int i = 0; i < _datos.Count; i++)
                {
                    var estado = _datos[i];
                    float porcentaje = (float)estado.CantidadPedidos / totalPedidos;
                    float angulo = porcentaje * 360f;

                    // Seleccionar color
                    canvas.FillColor = colores[i % colores.Length];

                    // Dibujar sector del gráfico de torta
                    canvas.FillArc(centroX - radio, centroY - radio, radio * 2, radio * 2,
                                  anguloInicial, angulo, true);

                    // Dibujar borde del sector
                    canvas.StrokeColor = Colors.White;
                    canvas.StrokeSize = 2;
                    canvas.DrawArc(centroX - radio, centroY - radio, radio * 2, radio * 2,
                                  anguloInicial, angulo, true, false);

                    // Actualizar ángulo inicial para el siguiente sector
                    anguloInicial += angulo;
                }

                // Dibujar leyenda
                float leyendaX = 20;
                float leyendaY = height - margenInferior + 10;
                float alturaLinea = 20;
                float anchoColumna = (width - 40) / 2; // Dos columnas

                canvas.FontSize = 11;

                for (int i = 0; i < _datos.Count; i++)
                {
                    var estado = _datos[i];
                    float porcentaje = (float)estado.CantidadPedidos / totalPedidos;

                    // Posición de la leyenda (dos columnas)
                    float posX = leyendaX + (i % 2) * anchoColumna;
                    float posY = leyendaY + (i / 2) * alturaLinea;

                    // Verificar que no se salga del área
                    if (posY > height - 10) break;

                    // Dibujar cuadrado de color
                    canvas.FillColor = colores[i % colores.Length];
                    canvas.FillRectangle(posX, posY, 12, 12);

                    // Dibujar borde del cuadrado
                    canvas.StrokeColor = Colors.Gray;
                    canvas.StrokeSize = 1;
                    canvas.DrawRectangle(posX, posY, 12, 12);

                    // Preparar texto de la leyenda
                    string etiqueta = estado.EstadoPedido;
                    if (etiqueta.Length > 12)
                        etiqueta = etiqueta.Substring(0, 9) + "...";

                    string textoLeyenda = $"{etiqueta} ({estado.CantidadPedidos}, {porcentaje:P0})";

                    // Dibujar texto de la leyenda
                    canvas.FontColor = Colors.Black;
                    canvas.DrawString(textoLeyenda, posX + 18, posY + 9, HorizontalAlignment.Left);
                }

                // Dibujar información adicional en el centro (opcional)
                if (radio > 40) // Solo si hay espacio suficiente
                {
                    canvas.FontColor = Color.FromRgb(203, 67, 53);
                    canvas.FontSize = 14;
                    canvas.DrawString("Total", centroX, centroY - 8, HorizontalAlignment.Center);

                    canvas.FontSize = 18;
                    canvas.DrawString(totalPedidos.ToString(), centroX, centroY + 8, HorizontalAlignment.Center);
                }
            }
        }
        private async void ExportarPDF_Clicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                // Verificar que tengamos datos para exportar
                if (_datosResumen == null || _datosDetalle == null)
                {
                    await DisplayAlert("Error", "No hay datos disponibles para exportar.", "OK");
                    return;
                }

                // Generar el PDF usando el método de SqlServerService
                byte[] pdfBytes = await Task.Run(async () => 
                    await App.Database.GenerarReportePedidosPDF(
                        _datosResumen,
                        _datosDetalle,
                        FechaInicio.Date,
                        FechaFin.Date));

                // Crear el nombre del archivo
                string fileName = $"Reporte_Pedidos_{FechaInicio.Date:yyyyMMdd}_{FechaFin.Date:yyyyMMdd}.pdf";

                // Guardar y abrir el archivo
                await GuardarYAbrirPDFAsync(pdfBytes, fileName);

                await DisplayAlert("Éxito", "El reporte PDF se ha generado correctamente.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al generar el PDF: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al generar PDF: {ex.Message}");
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
                if (_datosResumen == null || _datosDetalle == null)
                {
                    await DisplayAlert("Error", "No hay datos disponibles para compartir.", "OK");
                    return;
                }

                // Generar el PDF usando el método de SqlServerService
                byte[] pdfBytes = await Task.Run(async () => 
                    await App.Database.GenerarReportePedidosPDF(
                        _datosResumen,
                        _datosDetalle,
                        FechaInicio.Date,
                        FechaFin.Date));

                // Crear el nombre del archivo
                string fileName = $"Reporte_Pedidos_{FechaInicio.Date:yyyyMMdd}_{FechaFin.Date:yyyyMMdd}.pdf";

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
                // Si no se puede abrir, al menos guardarlo en Downloads (Android) o Documents (iOS)
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
                    Title = "Compartir Reporte de Pedidos",
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al compartir el archivo: {ex.Message}");
            }
        }

        // También puedes agregar este método para previsualizar el PDF antes de guardarlo
        private async void PrevisualizarPDF_Clicked(object sender, EventArgs e)
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                if (_datosResumen == null || _datosDetalle == null)
                {
                    await DisplayAlert("Error", "No hay datos disponibles para previsualizar.", "OK");
                    return;
                }

                // Generar el PDF
                byte[] pdfBytes = await App.Database.GenerarReportePedidosPDF(
                    _datosResumen,
                    _datosDetalle,
                    FechaInicio.Date,
                    FechaFin.Date);

                // Crear archivo temporal para previsualización
                string fileName = $"Preview_Reporte_Pedidos_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                await File.WriteAllBytesAsync(filePath, pdfBytes);

                // Abrir para previsualización
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath),
                    Title = "Previsualizar Reporte"
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al previsualizar el PDF: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }
    }
}