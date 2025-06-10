using System;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using AppTransporte.model;
using AppTransporte.viewModel;

namespace AppTransporte.Interfaces
{
    public partial class PedidoAuto : ContentPage
    {
        private int _idUsuario;
        private int _idTipoUsuario;

        public PedidoAuto(int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();

            // CORRECCIÓN: Asignar correctamente los parámetros
            this._idUsuario = idUsuario;
            this._idTipoUsuario = idTipoUsuario;

            // Inicializar fechas por defecto
            InicializarFechasPorDefecto();
        }

        private void InicializarFechasPorDefecto()
        {
            // Fecha de inicio: hoy
            FechaInicioPicker.Date = DateTime.Today;

            // Fecha de fin: una semana después
            FechaFinPicker.Date = DateTime.Today.AddDays(7);

            // Hora por defecto: 8:00 AM
            HoraPicker.Time = new TimeSpan(8, 0, 0);
        }

        private async void Btn_atras(object sender, EventArgs e)
        {
            // CORRECCIÓN: Usar las variables correctas
            await Navigation.PushAsync(new MenuPrincipal(_idUsuario, _idTipoUsuario));
        }

        private void OnFrecuenciaChanged(object sender, EventArgs e)
        {
            // Mostrar selección de días solo si es "Personalizada"
            bool esPersonalizada = FrecuenciaPicker.SelectedItem?.ToString() == "Personalizada";
            DiasSeleccionadosLayout.IsVisible = esPersonalizada;

            // Limpiar selecciones si no es personalizada
            if (!esPersonalizada)
            {
                foreach (var check in new[] { LunesCheck, MartesCheck, MiercolesCheck, JuevesCheck, ViernesCheck, SabadoCheck })
                    check.IsChecked = false;
            }
        }

        private void FechaInicioPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            // Validar que la fecha de inicio no sea anterior a hoy
            if (e.NewDate < DateTime.Today)
            {
                DisplayAlert("Advertencia", "La fecha de inicio no puede ser anterior a hoy", "OK");
                FechaInicioPicker.Date = DateTime.Today;
                return;
            }

            // Ajustar fecha de fin si es necesario
            if (FechaFinPicker.Date <= e.NewDate)
            {
                FechaFinPicker.Date = e.NewDate.AddDays(1);
            }
        }

        private void FechaFinPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            // Validar que la fecha de fin sea posterior a la de inicio
            if (e.NewDate <= FechaInicioPicker.Date)
            {
                DisplayAlert("Advertencia", "La fecha de fin debe ser posterior a la fecha de inicio", "OK");
                FechaFinPicker.Date = FechaInicioPicker.Date.AddDays(1);
            }
        }

        private async void Btn_crear(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                // Deshabilitar botón mientras se procesa
                button.IsEnabled = false;
                button.Text = "Creando...";

                // Validar datos
                if (!ValidarFormulario())
                    return;

                // Obtener datos del formulario
                var tipoServicio = TipoServicioPicker.SelectedItem?.ToString();
                var frecuencia = FrecuenciaPicker.SelectedItem?.ToString();
                var fechaInicio = FechaInicioPicker.Date;
                var fechaFin = FechaFinPicker.Date;
                var hora = HoraPicker.Time;
                var cantidadBarriles = int.Parse(cantidadEntry.Text);

                // Obtener días seleccionados si es personalizada
                string diasSeleccionados = "";
                if (frecuencia == "Personalizada")
                {
                    diasSeleccionados = ObtenerDiasSeleccionados();
                    if (string.IsNullOrWhiteSpace(diasSeleccionados))
                    {
                        await DisplayAlert("Error", "Debe seleccionar al menos un día para frecuencia personalizada.", "OK");
                        return;
                    }
                }

                // Crear descripción automática
                string descripcion = $"Pedido automatizado - {tipoServicio} - {frecuencia}";
                if (!string.IsNullOrEmpty(diasSeleccionados))
                {
                    descripcion += $" ({diasSeleccionados})";
                }

                System.Diagnostics.Debug.WriteLine($"Creando pedido programado: {tipoServicio}, {frecuencia}, {cantidadBarriles} barriles");

                // Llamar al método de base de datos
                var resultado = await App.Database.CrearPedidoProgramadoAsync(
                    idUsuario: _idUsuario,
                    idCliente: null, // Por ahora null, puedes agregar lógica para seleccionar cliente
                    tipoServicio: tipoServicio,
                    frecuencia: frecuencia,
                    diasSeleccionados: diasSeleccionados,
                    fechaInicio: fechaInicio,
                    fechaFin: fechaFin,
                    horaProgramada: hora,
                    cantidadBarriles: cantidadBarriles,
                    descripcion: descripcion
                );

                if (resultado.IdPedido > 0)
                {
                    await DisplayAlert("? Éxito",
                        $"Pedido programado creado exitosamente.\n\n" +
                        $"ID: {resultado.IdPedido}\n" +
                        $"Tipo: {tipoServicio}\n" +
                        $"Frecuencia: {frecuencia}\n" +
                        $"Cantidad: {cantidadBarriles} barriles\n" +
                        $"Período: {fechaInicio:dd/MM/yyyy} - {fechaFin:dd/MM/yyyy}", "OK");

                    // Limpiar formulario
                    LimpiarFormulario();
                }
                else
                {
                    await DisplayAlert("? Error", resultado.Mensaje ?? "No se pudo crear el pedido programado", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("? Error", $"Ocurrió un error al crear el pedido programado:\n{ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al crear pedido programado: {ex.Message}");
            }
            finally
            {
                // Rehabilitar botón
                if (button != null)
                {
                    button.IsEnabled = true;
                    button.Text = "Programar Pedido";
                }
            }
        }

        private bool ValidarFormulario()
        {
            // Validar tipo de servicio
            if (TipoServicioPicker.SelectedItem == null)
            {
                DisplayAlert("Error", "Debe seleccionar un tipo de servicio.", "OK");
                return false;
            }

            // Validar frecuencia
            if (FrecuenciaPicker.SelectedItem == null)
            {
                DisplayAlert("Error", "Debe seleccionar una frecuencia.", "OK");
                return false;
            }

            // Validar cantidad
            if (string.IsNullOrWhiteSpace(cantidadEntry.Text))
            {
                DisplayAlert("Error", "Debe ingresar la cantidad en barriles.", "OK");
                return false;
            }

            if (!int.TryParse(cantidadEntry.Text, out int cantidad) || cantidad <= 0)
            {
                DisplayAlert("Error", "Ingrese una cantidad válida en barriles (mayor a 0).", "OK");
                return false;
            }

            // Validar fechas
            if (FechaInicioPicker.Date < DateTime.Today)
            {
                DisplayAlert("Error", "La fecha de inicio no puede ser anterior a hoy.", "OK");
                return false;
            }

            if (FechaFinPicker.Date <= FechaInicioPicker.Date)
            {
                DisplayAlert("Error", "La fecha de fin debe ser posterior a la fecha de inicio.", "OK");
                return false;
            }

            // Validar días personalizados si aplica
            if (FrecuenciaPicker.SelectedItem?.ToString() == "Personalizada")
            {
                var diasSeleccionados = ObtenerDiasSeleccionados();
                if (string.IsNullOrWhiteSpace(diasSeleccionados))
                {
                    DisplayAlert("Error", "Debe seleccionar al menos un día para frecuencia personalizada.", "OK");
                    return false;
                }
            }

            return true;
        }

        private string ObtenerDiasSeleccionados()
        {
            var diasSeleccionados = new List<string>();

            if (LunesCheck.IsChecked) diasSeleccionados.Add("Lunes");
            if (MartesCheck.IsChecked) diasSeleccionados.Add("Martes");
            if (MiercolesCheck.IsChecked) diasSeleccionados.Add("Miércoles");
            if (JuevesCheck.IsChecked) diasSeleccionados.Add("Jueves");
            if (ViernesCheck.IsChecked) diasSeleccionados.Add("Viernes");
            if (SabadoCheck.IsChecked) diasSeleccionados.Add("Sábado");

            return string.Join(", ", diasSeleccionados);
        }

        private void LimpiarFormulario()
        {
            // Limpiar selecciones
            TipoServicioPicker.SelectedItem = null;
            FrecuenciaPicker.SelectedItem = null;
            cantidadEntry.Text = "";

            // Resetear fechas
            InicializarFechasPorDefecto();

            // Limpiar días seleccionados
            foreach (var check in new[] { LunesCheck, MartesCheck, MiercolesCheck, JuevesCheck, ViernesCheck, SabadoCheck })
                check.IsChecked = false;

            // Ocultar layout de días
            DiasSeleccionadosLayout.IsVisible = false;
        }

        // NUEVO: Evento para el botón "Ver Pedidos Programados"
        private async void Btn_verPedidosProgramados(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ListaPedidosProgramados(_idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al navegar: {ex.Message}", "OK");
            }
        }

        // NUEVO: Mostrar información adicional cuando cambia la cantidad
        private void cantidadEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out int barriles) && barriles > 0)
            {
                double litros = barriles * 159.0; // 1 barril = 159 litros
                int viajes = (int)Math.Ceiling(litros / 25000.0); // Asumiendo cisternas de 25,000 L

                Viajeslbl.Text = $"˜ {litros:N0} litros | Viajes estimados: {viajes}";
                Viajeslbl.TextColor = Colors.DarkBlue;
                Viajeslbl.FontAttributes = FontAttributes.Italic;
            }
            else
            {
                Viajeslbl.Text = "";
            }
        }
    }
}