using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    /// <summary>
    /// ViewModel for managing and displaying service requests (Solicitudes).
    /// This class loads requests for either an admin (all requests) or a specific client,
    /// and provides filtering by status, client name, and request description.
    /// </summary>
    public class VMmisSolicitudes : BaseViewModel
    {
        private bool _isBusy;
        private readonly ObservableCollection<Solicitud> _allSolicitudes = new();
        private ObservableCollection<Solicitud> _solicitudesFiltradas = new();
        private string _textoBusquedaDescripcion;
        private string _textoBusquedaCliente;
        private string _estadoSeleccionado;

        /// <summary>
        /// Gets the list of possible request statuses for filtering.
        /// </summary>
        public List<string> EstadosSolicitud { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is busy loading data.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Gets or sets the filtered collection of service requests to be displayed in the view.
        /// </summary>
        public ObservableCollection<Solicitud> SolicitudesFiltradas
        {
            get => _solicitudesFiltradas;
            set => SetProperty(ref _solicitudesFiltradas, value);
        }

        /// <summary>
        /// Gets or sets the search text for filtering requests by their description.
        /// </summary>
        public string TextoBusquedaDescripcion
        {
            get => _textoBusquedaDescripcion;
            set
            {
                if (SetProperty(ref _textoBusquedaDescripcion, value))
                {
                    Filtrar();
                }
            }
        }

        /// <summary>
        /// Gets or sets the search text for filtering requests by the client's name.
        /// </summary>
        public string TextoBusquedaCliente
        {
            get => _textoBusquedaCliente;
            set
            {
                if (SetProperty(ref _textoBusquedaCliente, value))
                {
                    Filtrar();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected status for filtering the request list.
        /// </summary>
        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (SetProperty(ref _estadoSeleccionado, value))
                {
                    Filtrar();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMmisSolicitudes"/> class for an admin context, loading all requests.
        /// </summary>
        public VMmisSolicitudes()
        {
            Initialize();
            CargarSolicitudesAsync(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMmisSolicitudes"/> class for a specific client context.
        /// </summary>
        /// <param name="idCliente">The ID of the client whose requests are to be loaded.</param>
        public VMmisSolicitudes(int idCliente)
        {
            Initialize();
            CargarSolicitudesAsync(idCliente);
        }

        /// <summary>
        /// Initializes common properties for the ViewModel.
        /// </summary>
        private void Initialize()
        {
            EstadosSolicitud = new List<string> { "Todos", "Por revisar", "Pedido Creado", "Cancelada" };
            EstadoSeleccionado = "Por revisar";
            TextoBusquedaCliente = string.Empty;
        }

        /// <summary>
        /// Asynchronously loads service requests from the database.
        /// </summary>
        /// <param name="idCliente">Optional. If provided, only requests for this client will be loaded.</param>
        private async void CargarSolicitudesAsync(int? idCliente)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var solicitudes = await App.Database.ObtenerSolicitudesAsync(idCliente);
                _allSolicitudes.Clear();
                foreach (var solicitud in solicitudes)
                {
                    _allSolicitudes.Add(solicitud);
                }
                Filtrar();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading requests: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Filters the displayed list of requests based on the current filter criteria.
        /// </summary>
        private void Filtrar()
        {
            IEnumerable<Solicitud> tempFiltered = _allSolicitudes;

            if (EstadoSeleccionado != "Todos")
            {
                tempFiltered = tempFiltered.Where(s => s.EstadoSolicitud == EstadoSeleccionado);
            }

            if (!string.IsNullOrWhiteSpace(TextoBusquedaCliente))
            {
                tempFiltered = tempFiltered.Where(s => s.Cliente.Contains(TextoBusquedaCliente, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(TextoBusquedaDescripcion))
            {
                tempFiltered = tempFiltered.Where(s => s.Descripcion.Contains(TextoBusquedaDescripcion, StringComparison.OrdinalIgnoreCase));
            }

            SolicitudesFiltradas = new ObservableCollection<Solicitud>(tempFiltered);
        }
    }
}