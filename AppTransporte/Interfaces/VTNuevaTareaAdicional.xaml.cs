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

            string connectionString = ("Data Source=SQL8011.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123;Connection Timeout=60");
            _sqlService = new SqlServerService(connectionString);
            this._idUsuario = idUsuario;
            this._idTipoUsuario = idTipoUsuario;
            DatePickerTarea.Date = DateTime.Today;
            TimePickerInicio.Time = TimeSpan.Zero;
            TimePickerFin.Time = TimeSpan.Zero;
        }
        public void setUserData(int idUsuario, int idTipoUsuario)
        {
            _idUsuario = idUsuario;
            _idTipoUsuario = idTipoUsuario;
        }

        private async void Btn_AtrasTareas(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TareasAdicionales(_idUsuario , _idTipoUsuario));
        }

        private void Btn_LimpiarTarea(object sender, EventArgs e)
        {
            DatePickerTarea.Date = DateTime.Today;
            TimePickerInicio.Time = TimeSpan.Zero;
            TimePickerFin.Time = TimeSpan.Zero;
            EditorDescripcion.Text = string.Empty;
        }

        private async void Btn_GuardarTarea(object sender, EventArgs e)
        {
            if (!ValidarFormulario())
                return;

            try
            {
                BtnGuardar.IsEnabled = false;
                BtnGuardar.Text = "Guardando...";

                var nuevaTarea = new TareaAdicional
                {
                    fecha_tarea = DatePickerTarea.Date,
                    hora_inicio = TimePickerInicio.Time,
                    hora_fin = TimePickerFin.Time,
                    descripcion = EditorDescripcion.Text.Trim(),
                    id_usuario = ObtenerIdUsuarioActual(),
                    estado = true
                };

                var resultado = await _sqlService.InsertarTareaAsync(nuevaTarea);

                if (resultado.EsExitoso)
                {
                    await DisplayAlert("Éxito", resultado.Mensaje, "OK");
                    Btn_LimpiarTarea(sender, e);
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", resultado.Mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
            finally
            {
                BtnGuardar.IsEnabled = true;
                BtnGuardar.Text = "Guardar Tarea";
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(EditorDescripcion.Text))
            {
                DisplayAlert("Error", "La descripción es obligatoria", "OK");
                return false;
            }

            if (EditorDescripcion.Text.Trim().Length > 500)
            {
                DisplayAlert("Error", "La descripción no puede exceder 500 caracteres", "OK");
                return false;
            }

            if (TimePickerFin.Time <= TimePickerInicio.Time)
            {
                DisplayAlert("Error", "La hora de finalización debe ser mayor que la de inicio", "OK");
                return false;
            }

            var duracion = TimePickerFin.Time - TimePickerInicio.Time;
            if (duracion.TotalMinutes < 15)
            {
                DisplayAlert("Error", "La tarea debe tener una duración mínima de 15 minutos", "OK");
                return false;
            }

            if (duracion.TotalHours > 16)
            {
                DisplayAlert("Error", "La tarea no puede exceder 16 horas de duración", "OK");
                return false;
            }

            return true;
        }

        private int ObtenerIdUsuarioActual()
        {
            return 1;
        }
    }
}