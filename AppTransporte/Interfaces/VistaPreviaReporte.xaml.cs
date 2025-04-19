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

    private async void ExportarPDF_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Exportar", "Funcionalidad de exportación a PDF en desarrollo", "OK");
    }

    private async void Compartir_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Compartir", "Funcionalidad para compartir reporte en desarrollo", "OK");
    }
}

// Clase auxiliar para visualización
public class ReporteTrabajadorView : ReporteTrabajador
{
    public bool Row { get; set; } // Para alternar colores de filas
}
