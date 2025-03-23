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

            if (!DiasSeleccionadosLayout.IsVisible)
                foreach (var check in new[] { LunesCheck, MartesCheck, MiercolesCheck, JuevesCheck, ViernesCheck, SabadoCheck })
                    check.IsChecked = false;
        }
        private void FechaInicioPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
        }
        private void FechaFinPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
        }

        private async void Btn_crear(object sender, EventArgs e)
        {
            try
            {
                var frecuencia = FrecuenciaPicker.SelectedItem?.ToString();
                var fechaInicio = FechaInicioPicker.Date;
                var fechaFin = FechaFinPicker.Date;
                var hora = HoraPicker.Time;
                var cantidad = cantidadEntry.Text;

                if (string.IsNullOrWhiteSpace(frecuencia) ||
                    string.IsNullOrWhiteSpace(cantidadEntry.Text) ||
                    fechaInicio == default ||
                    fechaFin == default ||
                    cantidad == default ||
                    hora == default)
                {
                    await DisplayAlert("Error", "Todos los campos deben estar llenos.", "OK");
                    return;
                }
                if (!int.TryParse(cantidadEntry.Text, out int cantidadLbl))
                {
                    await DisplayAlert("Error", "Ingrese una cantidad válida en barriles.", "OK");
                    return;
                }

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