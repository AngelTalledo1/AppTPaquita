using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    /// <summary>
    /// ViewModel for managing and displaying a list of trips (Viajes).
    /// This class handles loading trips from the database, filtered by order ID or user ID,
    /// and provides further client-side filtering by trip status.
    /// It also calculates and exposes summary data, like barrel counts.
    /// </summary>
    public class VMViajes : BaseViewModel
    {
        private bool _isBusy;
        private readonly ObservableCollection<Viaje> _allViajes = new();
        private ObservableCollection<Viaje> _viajesFiltrados = new();
        private int? _idPedidoSeleccionado;
        private int? _idUsuario;
        private string _estadoSeleccionado;

        /// <summary>
        /// Gets the list of available trip statuses for filtering.
        /// </summary>
        public List<string> Estados { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is busy loading data.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Gets the collection of filtered trips to be displayed in the view.
        /// </summary>
        public ObservableCollection<Viaje> ViajesFiltrados
        {
            get => _viajesFiltrados;
            private set => SetProperty(ref _viajesFiltrados, value);
        }

        /// <summary>
        /// Gets or sets the ID of the order to filter by.
        /// Setting this property triggers a reload of trip data from the database.
        /// </summary>
        public int? IdPedidoSeleccionado
        {
            get => _idPedidoSeleccionado;
            set
            {
                if (SetProperty(ref _idPedidoSeleccionado, value))
                {
                    CargarViajesAsync();
                }
            }
        }

        /// <summary>
        /// Gets or sets the ID of the user to filter by.
        /// Setting this property triggers a reload of trip data from the database.
        /// </summary>
        public int? IdUsuario
        {
            get => _idUsuario;
            set
            {
                if (SetProperty(ref _idUsuario, value))
                {
                    CargarViajesAsync();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected status to filter the trip list on the client side.
        /// </summary>
        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (SetProperty(ref _estadoSeleccionado, value))
                {
                    FiltrarViajes();
                }
            }
        }

        /// <summary>
        /// Calculates the total quantity (e.g., barrels) for all completed trips in the filtered list.
        /// </summary>
        public int TotalBarrilesFinalizados => ViajesFiltrados.Where(v => v.ultEstado == "Finalizado").Sum(v => v.Cantidad ?? 0);

        /// <summary>
        /// Gets a display string showing the ratio of completed barrels to the total barrels in the filtered list.
        /// </summary>
        public string BarrilesMostrados => $"{TotalBarrilesFinalizados} / {ViajesFiltrados.Sum(v => v.Cantidad ?? 0)}";

        /// <summary>
        /// Initializes a new instance of the <see cref="VMViajes"/> class.
        /// </summary>
        public VMViajes()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMViajes"/> class for a specific order.
        /// </summary>
        /// <param name="idPedido">The ID of the order to show trips for.</param>
        public VMViajes(int idPedido)
        {
            _idPedidoSeleccionado = idPedido;
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMViajes"/> class for a specific user.
        /// </summary>
        /// <param name="idUsuario">The ID of the user to show trips for.</param>
        public VMViajes(int? idUsuario)
        {
            _idUsuario = idUsuario;
            Initialize();
        }

        /// <summary>
        /// Initializes common properties and triggers the initial data load.
        /// </summary>
        private void Initialize()
        {
            Estados = new List<string> { "Todos", "Pendiente", "En el punto de Carga", "En camino al destino", "Finalizado" };
            CargarViajesAsync();
        }

        /// <summary>
        /// Asynchronously loads trip data from the database based on the current
        /// <see cref="IdPedidoSeleccionado"/> and <see cref="IdUsuario"/>.
        /// </summary>
        private async void CargarViajesAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var viajesDesdeBD = await App.Database.ObtenerViajesModAsync(IdPedidoSeleccionado, IdUsuario);
                _allViajes.Clear();
                foreach (var viaje in viajesDesdeBD)
                {
                    _allViajes.Add(viaje);
                }
                FiltrarViajes();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los viajes: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Filters the displayed list of trips based on the <see cref="EstadoSeleccionado"/>.
        /// </summary>
        private void FiltrarViajes()
        {
            var tempFiltered = _allViajes.AsEnumerable();

            if (!string.IsNullOrEmpty(EstadoSeleccionado) && EstadoSeleccionado != "Todos")
            {
                tempFiltered = tempFiltered.Where(p => p.ultEstado == EstadoSeleccionado);
            }

            ViajesFiltrados = new ObservableCollection<Viaje>(tempFiltered);
            // Notify that calculated properties may have changed.
            OnPropertyChanged(nameof(TotalBarrilesFinalizados));
            OnPropertyChanged(nameof(BarrilesMostrados));
        }
    }
}