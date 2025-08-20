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
                VolumenTransportado = item.VolumenTransportado,
                Periodo = item.Periodo,
                Row = alternarColor
            });

            alternarColor = !alternarColor;
        }

        // Asignar datos al CollectionView
        reporteCollectionView.ItemsSource = reporteViewData;

        // Actualizar resumen
        int totalTrabajadores = reporteData.Select(r => r.IdTrabajador).Distinct().Count();
        int totalViajes = reporteData.Sum(r => r.TotalViajes);
        decimal totalVolumen = reporteData.Sum(r => r.VolumenTransportado);

        TotalTrabajadoresLabel.Text = totalTrabajadores.ToString();
        TotalViajesLabel.Text = totalViajes.ToString();
        VolumenTotalLabel.Text = $"{totalVolumen:N2} L";
    }

    private void Btn_atras(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    // REEMPLAZA el método ExportarPDF_Clicked en VistaPreviaReporte por este:

    private async void ExportarPDF_Clicked(object sender, EventArgs e)
    {
        LoadingOverlay.IsVisible = true;

        try
        {
            // Verificar que tengamos datos para exportar
            if (reporteData == null || reporteData.Count == 0)
            {
                await DisplayAlert("Sin datos",
                    "No hay datos disponibles para exportar. Actualice el reporte primero.", "OK");
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
            byte[] pdfBytes = await Task.Run(() => App.Database.GenerarReporteTrabajadorPDF(
                reporteData, fechaInicio, fechaFin, tipoReporte));

            // Crear el nombre del archivo
            string fileName = $"Reporte_Trabajador_{DateTime.Now:yyyyMMddHHmmss}.pdf";

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

    // AGREGAR estos métodos auxiliares a la clase VistaPreviaReporte:

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
                Title = "Compartir Reporte de Trabajador",
                File = new ShareFile(filePath)
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al compartir el archivo: {ex.Message}");
        }
    }

    // TAMBIÉN REEMPLAZA el método Compartir_Clicked por este:

    private async void Compartir_Clicked(object sender, EventArgs e)
    {
        try
        {
            LoadingOverlay.IsVisible = true;

            // Verificar que tengamos datos para compartir
            if (reporteData == null || reporteData.Count == 0)
            {
                await DisplayAlert("Sin datos",
                    "No hay datos disponibles para compartir. Actualice el reporte primero.", "OK");
                return;
            }

            // Generar el PDF
            byte[] pdfBytes = await Task.Run(() => App.Database.GenerarReporteTrabajadorPDF(
                reporteData, fechaInicio, fechaFin, tipoReporte));

            // Crear el nombre del archivo
            string fileName = $"Reporte_Trabajador_{DateTime.Now:yyyyMMddHHmmss}.pdf";

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
    
}

// Clase auxiliar para visualización
public class ReporteTrabajadorView : ReporteTrabajador
{
    public bool Row { get; set; } // Para alternar colores de filas
}
