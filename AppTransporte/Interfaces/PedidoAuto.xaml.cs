using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    public partial class PedidoAuto : ContentPage
    {
        private int idUsuario;
        private int idtipousuario;
        private SqlServerService _sqlService;
        private PedidoAutomatico _pedidoEditar; // Para cuando se está editando

        public PedidoAuto(int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            this.idUsuario = idUsuario;
            this.idtipousuario = idTipoUsuario;

            // Usar el mismo connection string que ya tienes en tu SqlServerService existente
            // Busca en tu código donde ya inicializas SqlServerService y usa la misma cadena
            try
            {
                // Intenta usar el mismo connection string que en otras partes de tu app
                var existingService = new SqlServerService("Data Source=SQL8011.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123;Connection Timeout=60"); // Esto usará el que ya tienes configurado
                _sqlService = existingService;
            }
            catch
            {
                // Si no funciona, necesitarás poner tu connection string real aquí
                _sqlService = null;
            }

            InicializarFechas();
            CargarServicios();
        }

        // Constructor para editar pedido existente
        public PedidoAuto(int idUsuario, int idTipoUsuario, PedidoAutomatico pedidoEditar) : this(idUsuario, idTipoUsuario)
        {
            _pedidoEditar = pedidoEditar;
            CargarDatosParaEdicion();
        }

        private void InicializarFechas()
        {
            // Establecer fecha mínima como hoy
            FechaInicioPicker.MinimumDate = DateTime.Today;
            FechaFinPicker.MinimumDate = DateTime.Today;

            // Establecer fechas por defecto
            FechaInicioPicker.Date = DateTime.Today;
            FechaFinPicker.Date = DateTime.Today.AddDays(7); // Una semana por defecto
        }

        private async void CargarServicios()
        {
            try
            {
                if (_sqlService != null)
                {
                    var servicios = await _sqlService.ObtenerServiciosAsync();
                    TipoServicioPicker.ItemsSource = servicios.Select(s => s.Descripcion).ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar servicios: {ex.Message}");
                // Si no puede cargar de la BD, mantener los valores hardcodeados del XAML
            }
        }

        private void CargarDatosParaEdicion()
        {
            if (_pedidoEditar == null) return;

            // Cambiar título
            TituloLabel.Text = "Editar Pedido Automático";

            // Cargar datos en los controles
            TipoServicioPicker.SelectedItem = _pedidoEditar.TipoServicio;
            descripcionEntry.Text = _pedidoEditar.Descripcion;

            // Cargar días seleccionados
            var dias = _pedidoEditar.DiasSemana.Split(',').Select(d => d.Trim()).ToList();
            LunesCheck.IsChecked = dias.Contains("Lunes");
            MartesCheck.IsChecked = dias.Contains("Martes");
            MiercolesCheck.IsChecked = dias.Contains("Miércoles");
            JuevesCheck.IsChecked = dias.Contains("Jueves");
            ViernesCheck.IsChecked = dias.Contains("Viernes");
            SabadoCheck.IsChecked = dias.Contains("Sábado");
            DomingoCheck.IsChecked = dias.Contains("Domingo");

            FechaInicioPicker.Date = _pedidoEditar.FechaInicio;
            FechaFinPicker.Date = _pedidoEditar.FechaFin;
            HoraPicker.Time = _pedidoEditar.HoraProgramada;
        }

        private async void Btn_atras(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaPedidosAutomaticos(idUsuario, idtipousuario));
        }

        private async void Btn_verPedidos(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaPedidosAutomaticos(idUsuario, idtipousuario));
        }

        private void FechaInicioPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            // Asegurar que la fecha de fin no sea anterior a la de inicio
            if (FechaFinPicker.Date < e.NewDate)
            {
                FechaFinPicker.Date = e.NewDate.AddDays(1);
            }
            FechaFinPicker.MinimumDate = e.NewDate;
        }

        private void FechaFinPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            // Validación adicional si es necesaria
        }

        private async void Btn_crear(object sender, EventArgs e)
        {
            try
            {
                // Validaciones
                if (TipoServicioPicker.SelectedItem == null)
                {
                    await DisplayAlert("Error", "Debe seleccionar un tipo de servicio.", "OK");
                    return;
                }

                // Verificar que al menos un día esté seleccionado
                string diasSeleccionados = "";
                var dias = new[] {
                    LunesCheck.IsChecked ? "Lunes" : "",
                    MartesCheck.IsChecked ? "Martes" : "",
                    MiercolesCheck.IsChecked ? "Miércoles" : "",
                    JuevesCheck.IsChecked ? "Jueves" : "",
                    ViernesCheck.IsChecked ? "Viernes" : "",
                    SabadoCheck.IsChecked ? "Sábado" : "",
                    DomingoCheck.IsChecked ? "Domingo" : ""
                };
                diasSeleccionados = string.Join(", ", dias.Where(d => !string.IsNullOrEmpty(d)));

                if (string.IsNullOrWhiteSpace(diasSeleccionados))
                {
                    await DisplayAlert("Error", "Debe seleccionar al menos un día.", "OK");
                    return;
                }

                if (FechaInicioPicker.Date > FechaFinPicker.Date)
                {
                    await DisplayAlert("Error", "La fecha de inicio no puede ser mayor a la fecha de fin.", "OK");
                    return;
                }

                if (FechaInicioPicker.Date < DateTime.Today)
                {
                    await DisplayAlert("Error", "La fecha de inicio no puede ser anterior a hoy.", "OK");
                    return;
                }

                // Obtener el ID del servicio
                int idTipoServicio = await ObtenerIdServicioPorDescripcion(TipoServicioPicker.SelectedItem.ToString());

                if (idTipoServicio == 0)
                {
                    await DisplayAlert("Error", "No se pudo obtener el tipo de servicio seleccionado.", "OK");
                    return;
                }

                // Crear el objeto pedido automático
                var pedidoAutomatico = new PedidoAutomatico
                {
                    IdUsuario = idUsuario,
                    IdTipoServicio = idTipoServicio,
                    DiasSemana = diasSeleccionados,
                    HoraProgramada = HoraPicker.Time,
                    FechaInicio = FechaInicioPicker.Date,
                    FechaFin = FechaFinPicker.Date,
                    Descripcion = string.IsNullOrWhiteSpace(descripcionEntry.Text) ? null : descripcionEntry.Text
                };

                if (_sqlService == null)
                {
                    await DisplayAlert("Error", "Error de conexión a la base de datos. Revisa tu configuración.", "OK");
                    return;
                }

                RespuestaPedidoAutomatico respuesta;

                // Determinar si es creación o edición
                if (_pedidoEditar != null)
                {
                    // Edición
                    pedidoAutomatico.IdPedidoAutomatico = _pedidoEditar.IdPedidoAutomatico;
                    respuesta = await _sqlService.ActualizarPedidoAutomaticoAsync(pedidoAutomatico);
                }
                else
                {
                    // Creación
                    respuesta = await _sqlService.InsertarPedidoAutomaticoAsync(pedidoAutomatico);
                }

                if (respuesta.EsExitoso)
                {
                    string accion = _pedidoEditar != null ? "actualizado" : "creado";
                    await DisplayAlert("Éxito", $"Pedido automático {accion} exitosamente.", "OK");

                    // Navegar a la lista de pedidos automáticos
                    await Navigation.PushAsync(new ListaPedidosAutomaticos(idUsuario, idtipousuario));
                }
                else
                {
                    await DisplayAlert("Error", respuesta.Mensaje, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error inesperado: {ex.Message}", "OK");
            }
        }

        private async Task<int> ObtenerIdServicioPorDescripcion(string descripcion)
        {
            try
            {
                var servicios = await _sqlService.ObtenerServiciosAsync();
                var servicio = servicios.FirstOrDefault(s => s.Descripcion == descripcion);
                return servicio?.IdServicio ?? 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener ID del servicio: {ex.Message}");
                return 0;
            }
        }
    }
}