using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AppTransporte.model;
using Microsoft.Maui.Controls;

namespace AppTransporte.viewModel
{
    public class ActualizarEstadoViajeViewModel : INotifyPropertyChanged
    {
        private ViajeInfo _viajeInfo;
        private EstadoViaje _estadoSeleccionado;
        private string _comentario;
        private string _ubicacion;
        private int _cantidadRecogida;
        private string _mensajeValidacion;
        private string _mensajeCantidad;
        private ImageSource _fotoEvidencia;
        private string _fotoBytes;
        private int _idTrabajador;
        private string _resultadoOperacion;
        private bool _operacionExitosa;

        public ViajeInfo ViajeInfo
        {
            get => _viajeInfo;
            set => SetProperty(ref _viajeInfo, value);
        }

        public ObservableCollection<EstadoViaje> EstadosDisponibles { get; set; }

        public EstadoViaje EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (SetProperty(ref _estadoSeleccionado, value))
                {
                    ValidarEstadoSeleccionado();
                    OnPropertyChanged(nameof(EsEstadoRecogida));
                    OnPropertyChanged(nameof(RequiereUbicacion));
                    OnPropertyChanged(nameof(PuedeGuardar));
                }
            }
        }

        public string EstadoActual => ViajeInfo?.EstadoActual?.Descripcion ?? "";

        public string Comentario
        {
            get => _comentario;
            set => SetProperty(ref _comentario, value);
        }

        public string Ubicacion
        {
            get => _ubicacion;
            set
            {
                if (SetProperty(ref _ubicacion, value))
                    OnPropertyChanged(nameof(PuedeGuardar));
            }
        }

        public int CantidadRecogida
        {
            get => _cantidadRecogida;
            set
            {
                if (SetProperty(ref _cantidadRecogida, value))
                {
                    ValidarCantidad();
                    OnPropertyChanged(nameof(PuedeGuardar));
                }
            }
        }

        public string MensajeValidacion
        {
            get => _mensajeValidacion;
            set
            {
                SetProperty(ref _mensajeValidacion, value);
                OnPropertyChanged(nameof(TieneMensajeValidacion));
            }
        }

        public string MensajeCantidad
        {
            get => _mensajeCantidad;
            set
            {
                SetProperty(ref _mensajeCantidad, value);
                OnPropertyChanged(nameof(TieneMensajeCantidad));
            }
        }

        public ImageSource FotoEvidencia
        {
            get => _fotoEvidencia;
            set
            {
                SetProperty(ref _fotoEvidencia, value);
                OnPropertyChanged(nameof(TieneFoto));
            }
        }

        // Modificación en ActualizarEstadoViajeViewModel.cs - propiedad FotoBytes
        public string FotoBytes
        {
            get => _fotoBytes;
            set => SetProperty(ref _fotoBytes, value); // Ahora almacenará la URL, no la imagen en Base64
        }


        public int IdTrabajador
        {
            get => _idTrabajador;
            set => SetProperty(ref _idTrabajador, value);
        }

        public string ResultadoOperacion
        {
            get => _resultadoOperacion;
            set => SetProperty(ref _resultadoOperacion, value);
        }

        public bool OperacionExitosa
        {
            get => _operacionExitosa;
            set => SetProperty(ref _operacionExitosa, value);
        }

        // Propiedades computadas
        public bool EsEstadoRecogida => EstadoSeleccionado?.RequiereCantidad == true;
        public bool RequiereUbicacion => EstadoSeleccionado?.RequiereUbicacion == true;
        public bool TieneMensajeValidacion => !string.IsNullOrEmpty(MensajeValidacion);
        public bool TieneMensajeCantidad => !string.IsNullOrEmpty(MensajeCantidad);
        public bool TieneFoto => FotoEvidencia != null;
        public int CantidadDisponible => ViajeInfo?.CantidadPlanificada - ViajeInfo?.CantidadRecogida ?? 0;

        public bool PuedeGuardar
        {
            get
            {
                if (EstadoSeleccionado == null) return true; // Permitir guardar sin cambio de estado
                if (!string.IsNullOrEmpty(MensajeValidacion)) return false;
                if (EsEstadoRecogida && (CantidadRecogida <= 0 || !string.IsNullOrEmpty(MensajeCantidad))) return false;
                if (RequiereUbicacion && string.IsNullOrEmpty(Ubicacion)) return false;
                return true;
            }
        }

        public ActualizarEstadoViajeViewModel()
        {
            EstadosDisponibles = new ObservableCollection<EstadoViaje>();
        }

        public ActualizarEstadoViajeViewModel(ViajeInfo viajeInfo, int idTrabajador)
        {
            EstadosDisponibles = new ObservableCollection<EstadoViaje>();
            ViajeInfo = viajeInfo;
            IdTrabajador = idTrabajador;
        }

        // **CAMBIADO A ASYNC TASK EN LUGAR DE ASYNC VOID**
        public async Task CargarEstadosDisponiblesAsync()
        {
            try
            {
                var estados = await App.Database.ObtenerEstadosViajeAsync();

                // Solo mostrar estados posteriores al actual
                var estadosValidos = estados.Where(e => e.Orden > ViajeInfo?.EstadoActual?.Orden).ToList();

                EstadosDisponibles.Clear();
                foreach (var estado in estadosValidos)
                {
                    EstadosDisponibles.Add(estado);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar estados: {ex.Message}");
            }
        }

        private void ValidarEstadoSeleccionado()
        {
            if (EstadoSeleccionado == null)
            {
                MensajeValidacion = "";
                return;
            }

            // Validar que no sea un estado anterior
            if (EstadoSeleccionado.Orden <= ViajeInfo.EstadoActual.Orden)
            {
                MensajeValidacion = "No se puede retroceder a un estado anterior";
                return;
            }

            // Validar salto de estados (opcional)
            if (EstadoSeleccionado.Orden > ViajeInfo.EstadoActual.Orden + 1)
            {
                MensajeValidacion = "Advertencia: Está saltando estados intermedios";
                return;
            }

            MensajeValidacion = "";
        }

        private void ValidarCantidad()
        {
            if (!EsEstadoRecogida)
            {
                MensajeCantidad = "";
                return;
            }

            if (CantidadRecogida <= 0)
            {
                MensajeCantidad = "La cantidad debe ser mayor a cero";
                return;
            }

            if (CantidadRecogida > CantidadDisponible)
            {
                MensajeCantidad = $"La cantidad excede lo disponible ({CantidadDisponible} L)";
                return;
            }

            MensajeCantidad = "";
        }

        public async Task<bool> GuardarActualizacion()
        {
            try
            {
                if (EsEstadoRecogida)
                {
                    // **CASO ESPECIAL: Estado 4 "Carga Completa" - Registrar cantidad**
                    var resultado = await RegistrarRecogida();
                    return resultado;
                }
                else
                {
                    // **CASO NORMAL: Cambio de estado sin cantidad**
                    await ActualizarEstadoViaje();
                    return OperacionExitosa;
                }
            }
            catch (Exception ex)
            {
                ResultadoOperacion = $"Error: {ex.Message}";
                OperacionExitosa = false;
                return false;
            }
        }

        private async Task<bool> RegistrarRecogida()
        {
            try
            {
                // **USAR EL PA ESPECÍFICO PARA REGISTRAR CANTIDAD**
                var resultado = await App.Database.RegistrarRecogidaViajeAsync(
                    ViajeInfo.IdViaje,
                    CantidadRecogida,
                    IdTrabajador,
                    Ubicacion,
                    Comentario,
                    FotoBytes
                );

                ResultadoOperacion = resultado.Mensaje;
                OperacionExitosa = resultado.Exitoso;

                if (resultado.Exitoso)
                {
                    // Actualizar la información del viaje
                    ViajeInfo.CantidadRecogida = resultado.TotalRecogidoViaje;
                    // Actualizar el estado actual a "Carga Completa"
                    ViajeInfo.EstadoActual = EstadoSeleccionado;
                }

                return resultado.Exitoso;
            }
            catch (Exception ex)
            {
                ResultadoOperacion = $"Error al registrar la recogida: {ex.Message}";
                OperacionExitosa = false;
                return false;
            }
        }

        // En ActualizarEstadoViajeViewModel.cs - método ActualizarEstadoViaje
        private async Task ActualizarEstadoViaje()
        {
            try
            {
                // FotoBytes ahora contiene la URL de la imagen, no datos Base64
                var resultado = await App.Database.ActualizarEstadoSeguimientoAsync(
                    ViajeInfo.IdViaje,
                    EstadoSeleccionado.IdEstadoViaje,
                    Comentario,
                    FotoBytes  // URL de la imagen, no Base64
                );

                OperacionExitosa = resultado > 0;
                ResultadoOperacion = OperacionExitosa
                    ? $"Estado actualizado a: {EstadoSeleccionado.Descripcion}"
                    : "No se pudo actualizar el estado";

                if (OperacionExitosa)
                {
                    // Actualizar el estado actual del viaje
                    ViajeInfo.EstadoActual = EstadoSeleccionado;
                }
            }
            catch (Exception ex)
            {
                ResultadoOperacion = $"Error al actualizar el estado: {ex.Message}";
                OperacionExitosa = false;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}