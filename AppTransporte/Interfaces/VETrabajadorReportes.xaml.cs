using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class VETrabajadorReportes : ContentPage
    {
        private int _idUsuario;
        private int _idTipoUsuario;
        private int _idTrabajador;
        private Trabajador _trabajador;

        public VETrabajadorReportes(int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;
            CargarDatosTrabajador();
        }

        private async void CargarDatosTrabajador()
        {
            LoadingOverlay.IsVisible = true;

            try
            {
                // Obtener el trabajador directamente por el ID de usuario
                _trabajador = await App.Database.ObtenerTrabajadorPorUsuarioAsync(_idUsuario);

                if (_trabajador != null)
                {
                    _idTrabajador = _trabajador.IdTrabajador;

                    // Actualizar la información del trabajador en la UI
                    lblNombre.Text = $"{_trabajador.Nombre} {_trabajador.apePaterno} {_trabajador.apeMaterno}".Trim();
                    lblCategoria.Text = _trabajador.categoria ?? "No especificada";
                    lblIdTrabajador.Text = _idTrabajador.ToString();

                    Console.WriteLine($"Trabajador encontrado: ID={_idTrabajador}, Nombre={lblNombre.Text}");
                }
                else
                {
                    Console.WriteLine("No se encontró trabajador para el usuario actual");
                    await DisplayAlert("Error", "No se pudo encontrar la información del trabajador asociado a su cuenta", "OK");

                    // Deshabilitar los botones de reportes

                    // Ocultar información del trabajador
                    FrameInfoTrabajador.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CargarDatosTrabajador: {ex.Message}");
                await DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");

                // Ocultar información del trabajador
                FrameInfoTrabajador.IsVisible = false;
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void Btn_atras(object sender, EventArgs e)
        {
            // Preguntamos si está en la raíz de la navegación
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
            else
            {
                // Si está en la raíz, volvemos al menú principal
                await Navigation.PushAsync(new MenuTransportista(_idUsuario, _idTipoUsuario));
            }
        }

        private async void ReporteActividadDiaria_Clicked(object sender, EventArgs e)
        {
            if (!ValidarTrabajador())
                return;

            await MostrarReporte(() => new VistaReporteActividadDiaria(_idTrabajador, _idUsuario, _idTipoUsuario));
        }

        private async void ReporteTareasAdicionales_Clicked(object sender, EventArgs e)
        {
            if (!ValidarTrabajador())
                return;

            await MostrarReporte(() => new VistaReporteTareasAdicionales(_idTrabajador, _idUsuario, _idTipoUsuario));
        }

        private async void ReportePeriodoCompleto_Clicked(object sender, EventArgs e)
        {
            if (!ValidarTrabajador())
                return;

            // Aquí se navega al reporte de período
            await DisplayAlert("En desarrollo", "Esta funcionalidad estará disponible próximamente", "OK");
        }

        private async void ReporteEstadisticas_Clicked(object sender, EventArgs e)
        {
            if (!ValidarTrabajador())
                return;

            // Aquí se navega al reporte de estadísticas
            await DisplayAlert("En desarrollo", "Esta funcionalidad estará disponible próximamente", "OK");
        }

        // Método auxiliar para validar que el trabajador esté cargado
        private bool ValidarTrabajador()
        {
            if (_trabajador == null)
            {
                DisplayAlert("Error", "No se ha podido cargar la información del trabajador", "OK").ConfigureAwait(false);
                return false;
            }
            return true;
        }

        // Método auxiliar para mostrar reportes con manejo de errores unificado
        private async Task MostrarReporte(Func<Page> crearPagina)
        {
            LoadingOverlay.IsVisible = true;
            try
            {
                var pagina = crearPagina();
                await Navigation.PushAsync(pagina);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir el reporte: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }
    }
}
