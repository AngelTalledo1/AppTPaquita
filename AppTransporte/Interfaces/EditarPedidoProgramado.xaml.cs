using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class EditarPedidoProgramado : ContentPage
    {
        private PedidoProgramado _pedidoOriginal;
        private int _idUsuario;
        private int _idTipoUsuario;
        private bool _cambiosRealizados = false;

        public EditarPedidoProgramado(PedidoProgramado pedido, int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();

            _pedidoOriginal = pedido;
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;

            CargarDatosPedido();
        }

        private void CargarDatosPedido()
        {
            try
            {
                // Información del encabezado
                InfoPedidoLabel.Text = $"ID: {_pedidoOriginal.IdPedidoProgramado} | {_pedidoOriginal.TipoServicio}";
                EstadoActualLabel.Text = $"Estado: {_pedidoOriginal.Estado} | Creado: {_pedidoOriginal.FechaCreacion:dd/MM/yyyy HH:mm}";

                // Cargar datos en controles
                TipoServicioPicker.SelectedItem = _pedidoOriginal.TipoServicio;
                FrecuenciaPicker.SelectedItem = _pedidoOriginal.Frecuencia;
                FechaInicioPicker.Date = _pedidoOriginal.FechaInicio;
                FechaFinPicker.Date = _pedidoOriginal.FechaFin;
                HoraPicker.Time = _pedidoOriginal.HoraProgramada;
                CantidadEntry.Text = _pedidoOriginal.CantidadBarriles.ToString();
                DescripcionEntry.Text = _pedidoOriginal.Descripcion ?? "";

                // Cargar días seleccionados si es personalizada
                if (_pedidoOriginal.Frecuencia == "Personalizada" && !string.IsNullOrEmpty(_pedidoOriginal.DiasSeleccionados))
                {
                    CargarDiasSeleccionados(_pedidoOriginal.DiasSeleccionados);
                }

                // Mostrar/ocultar layout de días
                DiasSeleccionadosLayout.IsVisible = _pedidoOriginal.Frecuencia == "Personalizada";

                // Información de ejecuciones
                ActualizarInfoEjecuciones();

                // Configurar botón pausar según estado
                ConfigurarBotonPausar();

                // Mostrar advertencias si aplica
                MostrarAdvertencias();

                System.Diagnostics.Debug.WriteLine($"Datos cargados para pedido ID: {_pedidoOriginal.IdPedidoProgramado}");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", $"Error al cargar datos del pedido: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos: {ex.Message}");
            }
        }

        private void CargarDiasSeleccionados(string diasSeleccionados)
        {
            var dias = diasSeleccionados.Split(',').Select(d => d.Trim()).ToList();

            LunesCheck.IsChecked = dias.Contains("Lunes");
            MartesCheck.IsChecked = dias.Contains("Martes");
            MiercolesCheck.IsChecked = dias.Contains("Miércoles");
            JuevesCheck.IsChecked = dias.Contains("Jueves");
            ViernesCheck.IsChecked = dias.Contains("Viernes");
            SabadoCheck.IsChecked = dias.Contains("Sábado");
        }

        private void ActualizarInfoEjecuciones()
        {
            var info = $"Ejecutado {_pedidoOriginal.TotalEjecuciones} veces | " +
                      $"Última: {_pedidoOriginal.UltimaEjecucionTexto} | " +
                      $"Próxima: {_pedidoOriginal.ProximaEjecucionTexto}";

            EjecucionesInfoLabel.Text = info;
        }

        private void ConfigurarBotonPausar()
        {
            if (_pedidoOriginal.Estado == "Activo")
            {
                PausarButton.Text = "?? Pausar";
                PausarButton.BackgroundColor = Colors.Orange;
            }
            else if (_pedidoOriginal.Estado == "Pausado")
            {
                PausarButton.Text = "?? Reanudar";
                PausarButton.BackgroundColor = Colors.Green;
            }
            else
            {
                PausarButton.IsVisible = false;
            }
        }

        private void MostrarAdvertencias()
        {
            var advertencias = new List<string>();

            // Verificar si está vencido
            if (_pedidoOriginal.FechaFin < DateTime.Today)
            {
                advertencias.Add("?? Este pedido ya ha vencido");
            }

            // Verificar si está próximo a vencer
            if (_pedidoOriginal.FechaFin <= DateTime.Today.AddDays(7) && _pedidoOriginal.FechaFin >= DateTime.Today)
            {
                var diasRestantes = (_pedidoOriginal.FechaFin - DateTime.Today).Days;
                advertencias.Add($"? Vence en {diasRestantes} día(s)");
            }

            // Verificar si tiene ejecuciones pendientes
            if (_pedidoOriginal.DiasHastaEjecucion.HasValue && _pedidoOriginal.DiasHastaEjecucion <= 0 && _pedidoOriginal.Estado == "Activo")
            {
                advertencias.Add("?? Tiene ejecuciones pendientes");
            }

            if (advertencias.Any())
            {
                AdvertenciaFrame.IsVisible = true;
                AdvertenciaLabel.Text = string.Join(" | ", advertencias);
            }
        }

        #region Eventos de Validación

        private void OnFrecuenciaChanged(object sender, EventArgs e)
        {
            bool esPersonalizada = FrecuenciaPicker.SelectedItem?.ToString() == "Personalizada";
            DiasSeleccionadosLayout.IsVisible = esPersonalizada;

            if (!esPersonalizada)
            {
                // Limpiar selecciones
                foreach (var check in new[] { LunesCheck, MartesCheck, MiercolesCheck, JuevesCheck, ViernesCheck, SabadoCheck })
                    check.IsChecked = false;
            }

            _cambiosRealizados = true;
        }

        private void FechaInicioPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (e.NewDate < DateTime.Today)
            {
                DisplayAlert("Advertencia", "La fecha de inicio no puede ser anterior a hoy", "OK");
                FechaInicioPicker.Date = DateTime.Today;
                return;
            }

            if (FechaFinPicker.Date <= e.NewDate)
            {
                FechaFinPicker.Date = e.NewDate.AddDays(1);
            }

            _cambiosRealizados = true;
        }

        private void FechaFinPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (e.NewDate <= FechaInicioPicker.Date)
            {
                DisplayAlert("Advertencia", "La fecha de fin debe ser posterior a la fecha de inicio", "OK");
                FechaFinPicker.Date = FechaInicioPicker.Date.AddDays(1);
                return;
            }

            _cambiosRealizados = true;
        }

        private void CantidadEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out int barriles) && barriles > 0)
            {
                double litros = barriles * 159.0;
                int viajes = (int)Math.Ceiling(litros / 25000.0);
                ConversionLabel.Text = $"˜ {litros:N0} litros | Viajes estimados: {viajes}";
            }
            else
            {
                ConversionLabel.Text = "";
            }

            _cambiosRealizados = true;
        }

        #endregion

        #region Métodos de Negocio

        private bool ValidarFormulario()
        {
            if (TipoServicioPicker.SelectedItem == null)
            {
                DisplayAlert("Error", "Debe seleccionar un tipo de servicio", "OK");
                return false;
            }

            if (FrecuenciaPicker.SelectedItem == null)
            {
                DisplayAlert("Error", "Debe seleccionar una frecuencia", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(CantidadEntry.Text))
            {
                DisplayAlert("Error", "Debe ingresar la cantidad en barriles", "OK");
                return false;
            }

            if (!int.TryParse(CantidadEntry.Text, out int cantidad) || cantidad <= 0)
            {
                DisplayAlert("Error", "Ingrese una cantidad válida mayor a 0", "OK");
                return false;
            }

            if (FechaFinPicker.Date <= FechaInicioPicker.Date)
            {
                DisplayAlert("Error", "La fecha de fin debe ser posterior a la fecha de inicio", "OK");
                return false;
            }

            if (FrecuenciaPicker.SelectedItem?.ToString() == "Personalizada")
            {
                var diasSeleccionados = ObtenerDiasSeleccionados();
                if (string.IsNullOrWhiteSpace(diasSeleccionados))
                {
                    DisplayAlert("Error", "Debe seleccionar al menos un día para frecuencia personalizada", "OK");
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

        #endregion

        #region Eventos de Botones

        private async void Btn_atras(object sender, EventArgs e)
        {
            if (_cambiosRealizados)
            {
                bool salir = await DisplayAlert(
                    "Cambios no guardados",
                    "Tiene cambios sin guardar. ¿Está seguro que desea salir?",
                    "Salir sin guardar", "Cancelar");

                if (!salir) return;
            }

            await Navigation.PushAsync(new ListaPedidosProgramados(_idUsuario, _idTipoUsuario));
        }

        private async void Btn_guardarCambios(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                if (!ValidarFormulario()) return;

                button.IsEnabled = false;
                button.Text = "?? Guardando...";

                var tipoServicio = TipoServicioPicker.SelectedItem?.ToString();
                var frecuencia = FrecuenciaPicker.SelectedItem?.ToString();
                var diasSeleccionados = frecuencia == "Personalizada" ? ObtenerDiasSeleccionados() : null;
                var fechaInicio = FechaInicioPicker.Date;
                var fechaFin = FechaFinPicker.Date;
                var hora = HoraPicker.Time;
                var cantidad = int.Parse(CantidadEntry.Text);
                var descripcion = DescripcionEntry.Text;

                // Verificar si realmente hubo cambios
                bool huboChangios =
                    tipoServicio != _pedidoOriginal.TipoServicio ||
                    frecuencia != _pedidoOriginal.Frecuencia ||
                    diasSeleccionados != _pedidoOriginal.DiasSeleccionados ||
                    fechaInicio != _pedidoOriginal.FechaInicio ||
                    fechaFin != _pedidoOriginal.FechaFin ||
                    hora != _pedidoOriginal.HoraProgramada ||
                    cantidad != _pedidoOriginal.CantidadBarriles ||
                    descripcion != _pedidoOriginal.Descripcion;

                if (!huboChangios)
                {
                    await DisplayAlert("Info", "No se detectaron cambios para guardar", "OK");
                    return;
                }

                var mensaje = await App.Database.ActualizarPedidoProgramadoAsync(
                    _pedidoOriginal.IdPedidoProgramado,
                    tipoServicio,
                    frecuencia,
                    diasSeleccionados,
                    fechaInicio,
                    fechaFin,
                    hora,
                    cantidad,
                    descripcion,
                    recalcularProximaEjecucion: true
                );

                await DisplayAlert("? Éxito", "Cambios guardados correctamente", "OK");
                _cambiosRealizados = false;

                // Actualizar el objeto original
                _pedidoOriginal.TipoServicio = tipoServicio;
                _pedidoOriginal.Frecuencia = frecuencia;
                _pedidoOriginal.DiasSeleccionados = diasSeleccionados;
                _pedidoOriginal.FechaInicio = fechaInicio;
                _pedidoOriginal.FechaFin = fechaFin;
                _pedidoOriginal.HoraProgramada = hora;
                _pedidoOriginal.CantidadBarriles = cantidad;
                _pedidoOriginal.Descripcion = descripcion;

                // Actualizar UI
                InfoPedidoLabel.Text = $"ID: {_pedidoOriginal.IdPedidoProgramado} | {tipoServicio}";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al guardar cambios: {ex.Message}", "OK");
            }
            finally
            {
                button.IsEnabled = true;
                button.Text = "?? Guardar Cambios";
            }
        }

        private async void Btn_recalcularEjecucion(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.IsEnabled = false;
                button.Text = "?? Recalculando...";

                bool confirmar = await DisplayAlert(
                    "Recalcular Ejecución",
                    "¿Está seguro que desea recalcular la próxima ejecución?\n\n" +
                    "Esto actualizará la fecha y hora de la próxima ejecución según la configuración actual.",
                    "Recalcular", "Cancelar");

                if (!confirmar) return;

                var mensaje = await App.Database.ActualizarPedidoProgramadoAsync(
                    _pedidoOriginal.IdPedidoProgramado,
                    recalcularProximaEjecucion: true
                );

                await DisplayAlert("? Éxito", "Próxima ejecución recalculada correctamente", "OK");
                ActualizarInfoEjecuciones();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al recalcular: {ex.Message}", "OK");
            }
            finally
            {
                button.IsEnabled = true;
                button.Text = "?? Recalcular Próxima Ejecución";
            }
        }

        private async void Btn_pausarPedido(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.IsEnabled = false;

                string nuevoEstado = _pedidoOriginal.Estado == "Activo" ? "Pausado" : "Activo";
                string accion = nuevoEstado == "Pausado" ? "pausar" : "reanudar";

                bool confirmar = await DisplayAlert(
                    $"{char.ToUpper(accion[0])}{accion.Substring(1)} Pedido",
                    $"¿Está seguro que desea {accion} este pedido programado?",
                    char.ToUpper(accion[0]) + accion.Substring(1), "Cancelar");

                if (!confirmar) return;

                var mensaje = await App.Database.CambiarEstadoPedidoProgramadoAsync(
                    _pedidoOriginal.IdPedidoProgramado,
                    nuevoEstado,
                    $"Estado cambiado desde editor a {nuevoEstado}"
                );

                await DisplayAlert("? Éxito", mensaje, "OK");

                // Actualizar estado local
                _pedidoOriginal.Estado = nuevoEstado;
                EstadoActualLabel.Text = $"Estado: {_pedidoOriginal.Estado} | Creado: {_pedidoOriginal.FechaCreacion:dd/MM/yyyy HH:mm}";
                ConfigurarBotonPausar();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cambiar estado: {ex.Message}", "OK");
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        private async void Btn_eliminarPedido(object sender, EventArgs e)
        {
            var button = sender as Button;

            try
            {
                button.IsEnabled = false;

                bool confirmar = await DisplayAlert(
                    "?? Eliminar Pedido",
                    $"¿Está seguro que desea eliminar este pedido programado?\n\n" +
                    $"Esta acción NO se puede deshacer.",
                    "??? Eliminar", "Cancelar");

                if (!confirmar) return;

                string motivo = await DisplayPromptAsync(
                    "Motivo de Eliminación",
                    "Ingrese el motivo de la eliminación:",
                    "OK", "Cancelar",
                    "Ejemplo: Ya no se requiere el servicio",
                    maxLength: 200);

                if (string.IsNullOrWhiteSpace(motivo)) return;

                var mensaje = await App.Database.EliminarPedidoProgramadoAsync(
                    _pedidoOriginal.IdPedidoProgramado,
                    motivo
                );

                await DisplayAlert("? Eliminado", mensaje, "OK");
                await Navigation.PushAsync(new ListaPedidosProgramados(_idUsuario, _idTipoUsuario));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al eliminar: {ex.Message}", "OK");
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        #endregion
    }
}