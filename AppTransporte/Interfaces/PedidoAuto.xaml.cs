using System;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class PedidoAuto : ContentPage
    {
        public PedidoAuto()
        {
            InitializeComponent();
        }

        private void OnFrecuenciaChanged(object sender, EventArgs e)
        {
            DiasSeleccionadosLayout.IsVisible = FrecuenciaPicker.SelectedItem?.ToString() == "Personalizada";
        }
        private void FechaInicioPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            FechaInicioLabel.IsVisible = false; 
        }
        private void FechaFinPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            FechaFinLabel.IsVisible = false; 
        }

        private async void Btn_crear(object sender, EventArgs e)
        {
            try
            {
                var frecuencia = FrecuenciaPicker.SelectedItem?.ToString();
                var fechaInicio = FechaInicioPicker.Date;
                var fechaFin = FechaFinPicker.Date;
                var hora = HoraPicker.Time;

                string diasSeleccionados = "";
                if (frecuencia == "Personalizada")
                {
                    var dias = new[] {
                        LunesCheck.IsChecked ? "Lunes" : "",
                        MartesCheck.IsChecked ? "Martes" : "",
                        MiercolesCheck.IsChecked ? "Miércoles" : "",
                        JuevesCheck.IsChecked ? "Jueves" : "",
                        ViernesCheck.IsChecked ? "Viernes" : "",
                        SabadoCheck.IsChecked ? "Sábado" : "",
                       // DomingoCheck.IsChecked ? "Domingo" : ""
                    };
                    diasSeleccionados = string.Join(", ", dias.Where(d => !string.IsNullOrEmpty(d)));

                    if (string.IsNullOrWhiteSpace(diasSeleccionados))
                    {
                        await DisplayAlert("Error", "Debe seleccionar al menos un día.", "OK");
                        return;
                    }
                }


                //await App.Database.GuardarPedidoProgramadoAsync(new PedidoProgramado
                //{
                 //   Frecuencia = frecuencia,
                   // FechaInicio = fechaInicio,
                    //FechaFin = fechaFin,
                    //Hora = hora,
                    //Cantidad = int.Parse(FluidoEntry.Text),
                    //DiasSeleccionados = diasSeleccionados
                //});

                await DisplayAlert("Éxito", "Pedido programado exitosamente.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}