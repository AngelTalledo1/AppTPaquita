using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using AppTransporte.model;

namespace AppTransporte.Interfaces
{
    // Clases auxiliares para los pickers
    public class PickerItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public override string ToString() => Nombre;
    }

    public partial class PedidoAuto : ContentPage
    {
        private int idUsuario;
        private int idtipousuario;
        private SqlServerService _sqlService;
        private PedidoAutomatico _pedidoEditar; // Para cuando se está editando

        // Listas para los pickers nuevos
        private List<PickerItem> _transportistas = new();
        private List<PickerItem> _ayudantes = new();
        private List<PickerItem> _tractos = new();
        private List<PickerItem> _cisternas = new();

        public PedidoAuto(int idUsuario, int idTipoUsuario)
        {
            InitializeComponent();
            this.idUsuario = idUsuario;
            this.idtipousuario = idTipoUsuario;

            try
            {
                // Usar el mismo connection string que ya tienes
                var existingService = new SqlServerService("Data Source=SQL8011.site4now.net;Initial Catalog=db_aaecc9_paquitaappdb;User Id=db_aaecc9_paquitaappdb_admin;Password=paquita123;Connection Timeout=60");
                _sqlService = existingService;
            }
            catch
            {
                _sqlService = null;
            }

            InicializarFechas();
            CargarDatosAsync();
        }

        // Constructor para editar pedido existente
        public PedidoAuto(int idUsuario, int idTipoUsuario, PedidoAutomatico pedidoEditar) : this(idUsuario, idTipoUsuario)
        {
            _pedidoEditar = pedidoEditar;
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

        private async void CargarDatosAsync()
        {
            try
            {
                if (_sqlService != null)
                {
                    // Cargar servicios
                    await CargarServicios();

                    // Cargar transportistas, ayudantes, tractos y cisternas
                    await CargarTransportistas();
                    await CargarAyudantes();
                    await CargarTractos();
                    await CargarCisternas();

                    // Si es edición, cargar los datos después de que se carguen las listas
                    if (_pedidoEditar != null)
                    {
                        CargarDatosParaEdicion();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos: {ex.Message}");
                await DisplayAlert("Error", "Error al cargar datos. Algunos campos pueden no estar disponibles.", "OK");
            }
        }

        private async Task CargarServicios()
        {
            try
            {
                var servicios = await _sqlService.ObtenerServiciosAsync();
                TipoServicioPicker.ItemsSource = servicios.Select(s => s.Descripcion).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar servicios: {ex.Message}");
            }
        }

        private async Task CargarTransportistas()
        {
            try
            {
                var trabajadores = await _sqlService.ObtenerTrabajadoresAsync("Transportista");
                _transportistas.Clear();
                _transportistas.Add(new PickerItem { Id = 0, Nombre = "Transportista (Asignar después)" });

                foreach (var trabajador in trabajadores)
                {
                    _transportistas.Add(new PickerItem
                    {
                        Id = trabajador.IdTrabajador,
                        Nombre = $"{trabajador.Nombre} {trabajador.apePaterno} {trabajador.apeMaterno}".Trim()
                    });
                }

                TransportistaPicker.ItemsSource = _transportistas;
                TransportistaPicker.SelectedIndex = 0; // Seleccionar "Ninguno" por defecto
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar transportistas: {ex.Message}");
            }
        }

        private async Task CargarAyudantes()
        {
            try
            {
                var trabajadores = await _sqlService.ObtenerTrabajadoresAsync("Ayudante");
                _ayudantes.Clear();
                _ayudantes.Add(new PickerItem { Id = 0, Nombre = "Ayudante (Asignar después)" });

                foreach (var trabajador in trabajadores)
                {
                    _ayudantes.Add(new PickerItem
                    {
                        Id = trabajador.IdTrabajador,
                        Nombre = $"{trabajador.Nombre} {trabajador.apePaterno} {trabajador.apeMaterno}".Trim()
                    });
                }

                AyudantePicker.ItemsSource = _ayudantes;
                AyudantePicker.SelectedIndex = 0; // Seleccionar "Ninguno" por defecto
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar ayudantes: {ex.Message}");
            }
        }

        private async Task CargarTractos()
        {
            try
            {
                var tractos = await _sqlService.ObtenerTractosDisponiblesAsync();
                _tractos.Clear();
                _tractos.Add(new PickerItem { Id = 0, Nombre = "Tracto (Asignar después)" });

                foreach (var tracto in tractos)
                {
                    _tractos.Add(new PickerItem
                    {
                        Id = tracto.IdVehiculo,
                        Nombre = $"{tracto.Placa} - {tracto.Modelo}".Trim()
                    });
                }

                TractosPicker.ItemsSource = _tractos;
                TractosPicker.SelectedIndex = 0; // Seleccionar "Ninguno" por defecto
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar tractos: {ex.Message}");
            }
        }

        private async Task CargarCisternas()
        {
            try
            {
                var cisternas = await _sqlService.ObtenerCisternasDisponiblesAsync();
                _cisternas.Clear();
                _cisternas.Add(new PickerItem { Id = 0, Nombre = "Cisterna (Asignar después)" });

                foreach (var cisterna in cisternas)
                {
                    _cisternas.Add(new PickerItem
                    {
                        Id = cisterna.IdVehiculo,
                        Nombre = $"{cisterna.Placa} - {cisterna.AñoFabricacion}".Trim()
                    });
                }

                CisternaPicker.ItemsSource = _cisternas;
                CisternaPicker.SelectedIndex = 0; // Seleccionar "Ninguna" por defecto
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar cisternas: {ex.Message}");
            }
        }

        private void CargarDatosParaEdicion()
        {
            if (_pedidoEditar == null) return;

            try
            {
                // Cambiar título
                TituloLabel.Text = "Editar Pedido Automático";

                // Cargar datos en los controles
                TipoServicioPicker.SelectedItem = _pedidoEditar.TipoServicio;
                descripcionEntry.Text = _pedidoEditar.Descripcion;

                // Cargar selecciones de los nuevos campos
                if (_pedidoEditar.IdTransportista.HasValue && _pedidoEditar.IdTransportista > 0)
                {
                    var transportista = _transportistas.FirstOrDefault(t => t.Id == _pedidoEditar.IdTransportista);
                    if (transportista != null)
                        TransportistaPicker.SelectedItem = transportista;
                }

                if (_pedidoEditar.IdAyudante.HasValue && _pedidoEditar.IdAyudante > 0)
                {
                    var ayudante = _ayudantes.FirstOrDefault(a => a.Id == _pedidoEditar.IdAyudante);
                    if (ayudante != null)
                        AyudantePicker.SelectedItem = ayudante;
                }

                if (_pedidoEditar.IdTracto.HasValue && _pedidoEditar.IdTracto > 0)
                {
                    var tracto = _tractos.FirstOrDefault(t => t.Id == _pedidoEditar.IdTracto);
                    if (tracto != null)
                        TractosPicker.SelectedItem = tracto;
                }

                if (_pedidoEditar.IdCisterna.HasValue && _pedidoEditar.IdCisterna > 0)
                {
                    var cisterna = _cisternas.FirstOrDefault(c => c.Id == _pedidoEditar.IdCisterna);
                    if (cisterna != null)
                        CisternaPicker.SelectedItem = cisterna;
                }

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos para edición: {ex.Message}");
            }
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

                // Obtener IDs de las selecciones (si están seleccionadas)
                int? idTransportista = null;
                int? idAyudante = null;
                int? idTracto = null;
                int? idCisterna = null;

                if (TransportistaPicker.SelectedItem is PickerItem transportistaSeleccionado && transportistaSeleccionado.Id > 0)
                    idTransportista = transportistaSeleccionado.Id;

                if (AyudantePicker.SelectedItem is PickerItem ayudanteSeleccionado && ayudanteSeleccionado.Id > 0)
                    idAyudante = ayudanteSeleccionado.Id;

                if (TractosPicker.SelectedItem is PickerItem tractoSeleccionado && tractoSeleccionado.Id > 0)
                    idTracto = tractoSeleccionado.Id;

                if (CisternaPicker.SelectedItem is PickerItem cisternaSeleccionada && cisternaSeleccionada.Id > 0)
                    idCisterna = cisternaSeleccionada.Id;

                // Crear el objeto pedido automático
                var pedidoAutomatico = new PedidoAutomatico
                {
                    IdUsuario = idUsuario,
                    IdTipoServicio = idTipoServicio,
                    DiasSemana = diasSeleccionados,
                    HoraProgramada = HoraPicker.Time,
                    FechaInicio = FechaInicioPicker.Date,
                    FechaFin = FechaFinPicker.Date,
                    Descripcion = string.IsNullOrWhiteSpace(descripcionEntry.Text) ? null : descripcionEntry.Text,
                    // Nuevos campos
                    IdTransportista = idTransportista,
                    IdAyudante = idAyudante,
                    IdTracto = idTracto,
                    IdCisterna = idCisterna
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