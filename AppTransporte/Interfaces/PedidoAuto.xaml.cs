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
        public string Nombre { get; set; } = string.Empty;
        public override string ToString() => Nombre;
    }

    public partial class PedidoAuto : ContentPage
    {
        private int idUsuario;
        private int idtipousuario;
        private SqlServerService? _sqlService;
        private PedidoAutomatico? _pedidoEditar; // Para cuando se está editando

        // Nuevas listas para Cliente, Origen y Destino
        private List<PickerItem> _clientes = new();
        private List<PickerItem> _origenes = new();
        private List<PickerItem> _destinos = new();

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

        private async Task CargarClientes()
        {
            try
            {
                var clientes = await _sqlService!.ObtenerClientesAsync();
                _clientes.Clear();
                _clientes.Add(new PickerItem { Id = 0, Nombre = "Seleccionar Cliente" });

                foreach (var cliente in clientes)
                {
                    _clientes.Add(new PickerItem
                    {
                        Id = cliente.IdCliente,
                        Nombre = cliente.NombreCompleto
                    });
                }

                ClientePicker.ItemsSource = _clientes;
                ClientePicker.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar clientes: {ex.Message}");
            }
        }

        private async Task CargarUbicaciones()
        {
            try
            {
                var ubicaciones = await _sqlService!.ObtenerUbicacionesAsync();

                // Cargar orígenes
                _origenes.Clear();
                _origenes.Add(new PickerItem { Id = 0, Nombre = "Seleccionar Origen" });
                foreach (var ubicacion in ubicaciones)
                {
                    _origenes.Add(new PickerItem
                    {
                        Id = ubicacion.IdUbicacion,
                        Nombre = ubicacion.Descripcion
                    });
                }
                OrigenPicker.ItemsSource = _origenes;
                OrigenPicker.SelectedIndex = 0;

                // Cargar destinos
                _destinos.Clear();
                _destinos.Add(new PickerItem { Id = 0, Nombre = "Seleccionar Destino" });
                foreach (var ubicacion in ubicaciones)
                {
                    _destinos.Add(new PickerItem
                    {
                        Id = ubicacion.IdUbicacion,
                        Nombre = ubicacion.Descripcion
                    });
                }
                DestinoPicker.ItemsSource = _destinos;
                DestinoPicker.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar ubicaciones: {ex.Message}");
            }
        }

        private async Task CargarServicios()
        {
            try
            {
                var servicios = await _sqlService!.ObtenerServiciosAsync();
                TipoServicioPicker.ItemsSource = servicios.Select(s => s.Descripcion).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar servicios: {ex.Message}");
            }
        }
            */


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

                // Cargar selecciones de Cliente, Origen y Destino
                if (_pedidoEditar.IdCliente.HasValue && _pedidoEditar.IdCliente > 0)
                {
                    var cliente = _clientes.FirstOrDefault(c => c.Id == _pedidoEditar.IdCliente);
                    if (cliente != null)
                        ClientePicker.SelectedItem = cliente;
                }

                if (_pedidoEditar.IdOrigen.HasValue && _pedidoEditar.IdOrigen > 0)
                {
                    var origen = _origenes.FirstOrDefault(o => o.Id == _pedidoEditar.IdOrigen);
                    if (origen != null)
                        OrigenPicker.SelectedItem = origen;
                }

                if (_pedidoEditar.IdDestino.HasValue && _pedidoEditar.IdDestino > 0)
                {
                    var destino = _destinos.FirstOrDefault(d => d.Id == _pedidoEditar.IdDestino);
                    if (destino != null)
                        DestinoPicker.SelectedItem = destino;
                }
                */
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
                if (ClientePicker.SelectedItem == null || ((PickerItem)ClientePicker.SelectedItem).Id == 0)
                {
                    await DisplayAlert("Error", "Debe seleccionar un cliente.", "OK");
                    return;
                }

                if (TipoServicioPicker.SelectedItem == null)
                {
                    await DisplayAlert("Error", "Debe seleccionar un tipo de servicio.", "OK");
                    return;
                }

                if(OrigenPicker.SelectedItem == null)
                {
                    await DisplayAlert("Error", "Debe seleccionar un origen.", "OK");
                    return;
                }
                if(DestinoPicker.SelectedItem == null)
                {
                    await DisplayAlert("Error", "Debe seleccionar un destino.", "OK");
                    return;
                }

                if (OrigenPicker.SelectedItem == null || ((PickerItem)OrigenPicker.SelectedItem).Id == 0)
                {
                    await DisplayAlert("Error", "Debe seleccionar un origen.", "OK");
                    return;
                }

                if (DestinoPicker.SelectedItem == null || ((PickerItem)DestinoPicker.SelectedItem).Id == 0)
                {
                    await DisplayAlert("Error", "Debe seleccionar un destino.", "OK");
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
                int idTipoServicio = await ObtenerIdServicioPorDescripcion(TipoServicioPicker.SelectedItem?.ToString() ?? "");

                if (idTipoServicio == 0)
                {
                    await DisplayAlert("Error", "No se pudo obtener el tipo de servicio seleccionado.", "OK");
                    return;
                }

                // Obtener IDs de las selecciones principales
                int idCliente = ((PickerItem)ClientePicker.SelectedItem).Id;
                int idOrigen = ((PickerItem)OrigenPicker.SelectedItem).Id;
                int idDestino = ((PickerItem)DestinoPicker.SelectedItem).Id;

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
                //se crea un nuevo objeto Pedido
                var pedido = new Pedido
                {
                    /*
                    IdUsuario = idUsuario,
                    IdTipoServicio = idTipoServicio,
                    Origen = OrigenPicker.SelectedItem.ToString(),
                    Destino = DestinoPicker.SelectedItem.ToString(),
                    FechaCreacion = DateTime.Now,
                    Estado = "Pendiente"
                    */
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
                var servicios = await _sqlService!.ObtenerServiciosAsync();
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