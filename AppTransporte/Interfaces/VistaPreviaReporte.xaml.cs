using AppTransporte.model;
using System.Collections.ObjectModel;
using System.Globalization;

namespace AppTransporte.Interfaces;

public partial class VistaPreviaReporte : ContentPage
{
    private List<ReporteTrabajador> reporteData;
    private DateTime fechaInicio;
    private DateTime fechaFin;
    private string tipoReporte;

    public VistaPreviaReporte(List<ReporteTrabajador> data, DateTime fechaIni, DateTime fechaFin, string tipoReporte)
    {
        InitializeComponent();
        this.reporteData = data;
        this.fechaInicio = fechaIni;
        this.fechaFin = fechaFin;
        this.tipoReporte = tipoReporte;

        // Configurar la vista
        ConfigurarVista();

        // Cargar los datos
        CargarDatosReporte();
    }

    private void ConfigurarVista()
    {
        // Configurar título según el tipo de reporte
        TituloReporte.Text = $"Reporte de Trabajador - {tipoReporte}";

        // Configurar fechas
        FechasReporte.Text = $"Periodo: {fechaInicio.ToString("dd/MM/yyyy")} - {fechaFin.ToString("dd/MM/yyyy")}";

        // Configurar tipo
        TipoReporte.Text = $"Reporte {tipoReporte}";
    }

    private void CargarDatosReporte()
    {
        // Validar que tenemos datos
        if (reporteData == null || !reporteData.Any())
        {
            TotalTrabajadoresLabel.Text = "0";
            TotalViajesLabel.Text = "0";
            VolumenTotalLabel.Text = "0 L";
            reporteCollectionView.ItemsSource = new List<ReporteTrabajadorView>();
            return;
        }

        // Preparar datos para la vista
        var reporteViewData = new ObservableCollection<ReporteTrabajadorView>();
        bool alternarColor = false;

        foreach (var item in reporteData)
        {
            reporteViewData.Add(new ReporteTrabajadorView
            {
                IdTrabajador = item.IdTrabajador,
                NombreCompleto = item.NombreCompleto,
                Categoria = item.Categoria,
                TotalViajes = item.TotalViajes,
                TotalSeguimientos = item.TotalSeguimientos,
                // CORREGIDO: NO dividir por 1000 - usar valor directo
                VolumenTransportado = item.VolumenTransportado,
                Periodo = item.Periodo,
                Row = alternarColor
            });

            alternarColor = !alternarColor;
        }

        // Asignar datos al CollectionView
        reporteCollectionView.ItemsSource = reporteViewData;

        // CALCULAR TOTALES CORRECTAMENTE - SIN DIVISIONES
        // 1. Total trabajadores únicos
        int totalTrabajadores = reporteData
            .Select(r => r.IdTrabajador)
            .Distinct()
            .Count();

        // 2. Total viajes - SOLO CONTAR TRANSPORTISTAS (no ayudantes)
        int totalViajes = reporteData
            .Where(r => r.Categoria?.ToLower().Contains("transportista") == true)
            .Sum(r => r.TotalViajes);

        // 3. CORREGIDO: Volumen total transportado - SIN DIVISIÓN
        decimal totalVolumen = reporteData
            .Sum(r => (decimal)r.VolumenTransportado);

        // Actualizar labels con formato correcto
        TotalTrabajadoresLabel.Text = totalTrabajadores.ToString();
        TotalViajesLabel.Text = totalViajes.ToString("N0");
        VolumenTotalLabel.Text = $"{totalVolumen:N0} L";

        // Debug para verificar los cálculos
        System.Diagnostics.Debug.WriteLine($"=== RESUMEN CORREGIDO (SIN DIVISIÓN) ===");
        System.Diagnostics.Debug.WriteLine($"Total trabajadores únicos: {totalTrabajadores}");
        System.Diagnostics.Debug.WriteLine($"Total viajes (solo transportistas): {totalViajes:N0}");
        System.Diagnostics.Debug.WriteLine($"Volumen total transportado: {totalVolumen:N0} L (SIN división)");
        System.Diagnostics.Debug.WriteLine($"Registros totales: {reporteData.Count}");

        // Debug detallado por trabajador
        foreach (var trabajador in reporteData)
        {
            System.Diagnostics.Debug.WriteLine($"- {trabajador.NombreCompleto} ({trabajador.Categoria}): {trabajador.TotalViajes} viajes, {trabajador.VolumenTransportado:N0} L (valor directo)");
        }
    }

    private async void Btn_atras(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void ExportarPDF_Clicked(object sender, EventArgs e)
    {
        try
        {
            LoadingOverlay.IsVisible = true;

            // Generar el PDF
            byte[] pdfBytes = await App.Database.GenerarReporteTrabajadorPDF(
                reporteData, fechaInicio, fechaFin, tipoReporte);

            // Guardar en cache para poder abrirlo
            string fileName = $"Reporte_Trabajador_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            File.WriteAllBytes(filePath, pdfBytes);

            // Abrir el PDF usando el sistema de archivos del dispositivo
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath),
                Title = "Abrir reporte de trabajador"
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo exportar el PDF: {ex.Message}", "OK");
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }

    private async void Compartir_Clicked(object sender, EventArgs e)
    {
        try
        {
            LoadingOverlay.IsVisible = true;

            // Generar el PDF para compartir
            byte[] pdfBytes = await App.Database.GenerarReporteTrabajadorPDF(
                reporteData, fechaInicio, fechaFin, tipoReporte);

            // Nombre del archivo
            string fileName = $"Reporte_Trabajador_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            // Guardar en archivo temporal
            string tempPath = Path.Combine(FileSystem.CacheDirectory, fileName);
            File.WriteAllBytes(tempPath, pdfBytes);

            // Compartir el archivo
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Compartir reporte de trabajador",
                File = new ShareFile(tempPath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo compartir el reporte: {ex.Message}", "OK");
        }
        finally
        {
            LoadingOverlay.IsVisible = false;
        }
    }
}
public class ReporteTrabajadorView : ReporteTrabajador
{
    public bool Row { get; set; } // Para alternar colores de filas

    // Propiedad adicional para mostrar el volumen formateado correctamente
    public string VolumenFormateado => $"{VolumenTransportado:N0} L";

    // Propiedad para debug - ver el valor exacto
    public string DebugVolumen => $"Valor exacto: {VolumenTransportado}";
}

// Clase auxiliar para visualización
