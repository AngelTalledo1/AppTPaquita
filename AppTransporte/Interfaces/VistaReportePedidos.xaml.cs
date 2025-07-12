using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppTransporte.model;
// Importaciones para QuestPDF
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AppTransporte.Interfaces
{
    public partial class VistaReportePedidos : ContentPage
    {
        private List<ResumenPedido> _datosResumen;
        private List<DetallePedido> _datosDetalle;

        public VistaReportePedidos()
        {
            // Registrar licencia QuestPDF (necesario para evitar watermarks)
            QuestPDF.Settings.License = LicenseType.Community;

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
            // Implementar el dibujo del gráfico utilizando Microsoft.Maui.Graphics
            graficoView.Drawable = new GraficoPedidos(_datosResumen);
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
    }

    // Clase para el gráfico de pedidos por estado corregida para MAUI.Graphics
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
            float radio = Math.Min(centroY - margenSuperior, (width - margenIzquierdo - margenDerecho) / 2) - 10;

            // Colores para el gráfico usando Microsoft.Maui.Graphics.Colors
            Microsoft.Maui.Graphics.Color[] colores = new Microsoft.Maui.Graphics.Color[]
            {
                Microsoft.Maui.Graphics.Colors.DodgerBlue,
                Microsoft.Maui.Graphics.Colors.OrangeRed,
                Microsoft.Maui.Graphics.Colors.Green,
                Microsoft.Maui.Graphics.Colors.Purple,
                Microsoft.Maui.Graphics.Colors.Orange,
                Microsoft.Maui.Graphics.Colors.DeepPink,
                Microsoft.Maui.Graphics.Colors.Teal
            };

            // Total de pedidos
            int totalPedidos = _datos.Sum(d => d.CantidadPedidos);

            if (totalPedidos == 0) return;

            // Dibujar gráfico de torta
            float anguloInicial = 0;

            for (int i = 0; i < _datos.Count; i++)
            {
                var estado = _datos[i];
                float porcentaje = (float)estado.CantidadPedidos / totalPedidos;
                float angulo = porcentaje * 360f;

                // Seleccionar color
                canvas.FillColor = colores[i % colores.Length];

                // Dibujar sector usando Path para mejor compatibilidad
                var path = new PathF();
                path.MoveTo(centroX, centroY);

                // Convertir ángulos a radianes
                double anguloInicialRad = anguloInicial * Math.PI / 180;
                double anguloFinalRad = (anguloInicial + angulo) * Math.PI / 180;

                // Calcular puntos del arco
                float x1 = centroX + radio * (float)Math.Cos(anguloInicialRad);
                float y1 = centroY + radio * (float)Math.Sin(anguloInicialRad);
                float x2 = centroX + radio * (float)Math.Cos(anguloFinalRad);
                float y2 = centroY + radio * (float)Math.Sin(anguloFinalRad);

                path.LineTo(x1, y1);
                path.AddArc(centroX - radio, centroY - radio, centroX + radio, centroY + radio, anguloInicial, anguloInicial + angulo, false);
                path.LineTo(centroX, centroY);
                path.Close();

                canvas.FillPath(path);

                // Calcular la posición para la leyenda
                float posX = margenIzquierdo + (i % 3) * ((width - margenIzquierdo - margenDerecho) / 3);
                float posY = height - margenInferior + 15 + (i / 3) * 20;

                // Dibujar cuadrado de color para la leyenda
                canvas.FillColor = colores[i % colores.Length];
                canvas.FillRectangle(posX, posY, 10, 10);

                // Dibujar texto de la leyenda
                canvas.FontColor = Microsoft.Maui.Graphics.Colors.Black;
                canvas.FontSize = 10;

                // Abreviar texto si es muy largo
                string etiqueta = estado.EstadoPedido;
                if (etiqueta.Length > 15)
                    etiqueta = etiqueta.Substring(0, 12) + "...";

                canvas.DrawString($"{etiqueta} ({estado.CantidadPedidos}, {porcentaje:P0})", posX + 15, posY + 7, Microsoft.Maui.Graphics.HorizontalAlignment.Left);

                // Actualizar ángulo inicial para el siguiente sector
                anguloInicial += angulo;
            }

            // Dibujar título del gráfico
            canvas.FontColor = Microsoft.Maui.Graphics.Colors.Black;
            canvas.FontSize = 12;
            canvas.DrawString("Distribución de Pedidos por Estado", width / 2, margenSuperior - 5, Microsoft.Maui.Graphics.HorizontalAlignment.Center);
        }
    }
}