using AppTransporte.model;
using AppTransporte.viewModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace AppTransporte.Interfaces
{
    public partial class VTNuevaTareaAdicional : ContentPage
    {
        private int _idUsuario;
        private int _idTipoUsuario;
        private readonly SqlServerService _sqlService;

        public VTNuevaTareaAdicional(int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            this._idUsuario = idUsuario;
            this._idTipoUsuario = idTipoUsuario;

            // INICIALIZAR EL SERVICIO SQL (ESTO FALTABA)
            string connectionString = "Data Source=SQL8011.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123;Connection Timeout=60";
            _sqlService = new SqlServerService(connectionString);

            // Inicializar controles
            DatePickerTarea.Date = DateTime.Today;
            TimePickerInicio.Time = TimeSpan.Zero;
            TimePickerFin.Time = TimeSpan.Zero;

            // Log para debugging
            System.Diagnostics.Debug.WriteLine($"VTNuevaTareaAdicional inicializada para usuario: {_idUsuario}");
        }

        public void setUserData(int idUsuario, int idTipoUsuario)
        {
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;
            System.Diagnostics.Debug.WriteLine($"Datos de usuario actualizados: {_idUsuario}, {_idTipoUsuario}");
        }

        private async void Btn_AtrasTareas(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TareasAdicionales(_idTipoUsuario, _idUsuario));
        }

        private void Btn_LimpiarTarea(object sender, EventArgs e)
        {
            DatePickerTarea.Date = DateTime.Today;
            TimePickerInicio.Time = TimeSpan.Zero;
            TimePickerFin.Time = TimeSpan.Zero;
            EditorDescripcion.Text = string.Empty;

            System.Diagnostics.Debug.WriteLine("Formulario limpiado");
        }

        private async void Btn_GuardarTarea(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Iniciando guardado de tarea...");

            if (!ValidarFormulario())
            {
                System.Diagnostics.Debug.WriteLine("Validación de formulario falló");
                return;
            }

            try
            {
                // Mostrar estado de carga
                BtnGuardar.IsEnabled = false;
                BtnGuardar.Text = "Guardando...";

                System.Diagnostics.Debug.WriteLine($"Guardando tarea para usuario: {_idUsuario}");

                var nuevaTarea = new TareaAdicional
                {
                    fecha_tarea = DatePickerTarea.Date,
                    hora_inicio = TimePickerInicio.Time,
                    hora_fin = TimePickerFin.Time,
                    descripcion = EditorDescripcion.Text.Trim(),
                    id_usuario = _idUsuario,
                    estado = true
                };

                // Log de la tarea que se va a crear
                System.Diagnostics.Debug.WriteLine($"Tarea a crear:");
                System.Diagnostics.Debug.WriteLine($"  - Fecha: {nuevaTarea.fecha_tarea:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"  - Hora inicio: {nuevaTarea.hora_inicio}");
                System.Diagnostics.Debug.WriteLine($"  - Hora fin: {nuevaTarea.hora_fin}");
                System.Diagnostics.Debug.WriteLine($"  - Descripción: {nuevaTarea.descripcion}");
                System.Diagnostics.Debug.WriteLine($"  - ID Usuario: {nuevaTarea.id_usuario}");

                // USAR EL SERVICIO SQL EN LUGAR DE App.Database
                var resultado = await _sqlService.InsertarTareaAsync(nuevaTarea);

                System.Diagnostics.Debug.WriteLine($"Resultado del guardado: {resultado?.Mensaje ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"Éxito: {resultado?.EsExitoso ?? false}");

                if (resultado != null && resultado.EsExitoso)
                {
                    await DisplayAlert("Éxito", resultado.Mensaje, "OK");

                    // Limpiar formulario
                    Btn_LimpiarTarea(sender, e);

                    // Navegar de vuelta a la lista de tareas
                    System.Diagnostics.Debug.WriteLine("Navegando de vuelta a TareasAdicionales...");
                    await Navigation.PushAsync(new TareasAdicionales(_idTipoUsuario, _idUsuario));
                }
                else
                {
                    string mensajeError = resultado?.Mensaje ?? "Error desconocido al guardar la tarea";
                    await DisplayAlert("Error", mensajeError, "OK");
                    System.Diagnostics.Debug.WriteLine($"Error al guardar: {mensajeError}");
                }
            }
            catch (Exception ex)
            {
                string mensajeError = $"Ocurrió un error: {ex.Message}";
                await DisplayAlert("Error", mensajeError, "OK");
                System.Diagnostics.Debug.WriteLine($"Excepción al guardar tarea: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // Restaurar estado del botón
                BtnGuardar.IsEnabled = true;
                BtnGuardar.Text = "Guardar Tarea";
            }
        }

        private bool ValidarFormulario()
        {
            System.Diagnostics.Debug.WriteLine("Validando formulario...");

            if (string.IsNullOrWhiteSpace(EditorDescripcion.Text))
            {
                DisplayAlert("Error", "La descripción es obligatoria", "OK");
                System.Diagnostics.Debug.WriteLine("Validación falló: descripción vacía");
                return false;
            }

            if (EditorDescripcion.Text.Trim().Length > 500)
            {
                DisplayAlert("Error", "La descripción no puede exceder 500 caracteres", "OK");
                System.Diagnostics.Debug.WriteLine("Validación falló: descripción muy larga");
                return false;
            }

            if (TimePickerFin.Time <= TimePickerInicio.Time)
            {
                DisplayAlert("Error", "La hora de finalización debe ser mayor que la de inicio", "OK");
                System.Diagnostics.Debug.WriteLine("Validación falló: hora fin menor o igual a hora inicio");
                return false;
            }

            var duracion = TimePickerFin.Time - TimePickerInicio.Time;
            if (duracion.TotalMinutes < 15)
            {
                DisplayAlert("Error", "La tarea debe tener una duración mínima de 15 minutos", "OK");
                System.Diagnostics.Debug.WriteLine("Validación falló: duración menor a 15 minutos");
                return false;
            }

            if (duracion.TotalHours > 16)
            {
                DisplayAlert("Error", "La tarea no puede exceder 16 horas de duración", "OK");
                System.Diagnostics.Debug.WriteLine("Validación falló: duración mayor a 16 horas");
                return false;
            }

            System.Diagnostics.Debug.WriteLine("Validación exitosa");
            return true;
        }

        // Este método ya no se usa, pero lo dejamos por si acaso
        private int ObtenerIdUsuarioActual()
        {
            return _idUsuario; // Cambiado para usar el ID real en lugar de 1 fijo
        }
    }
}