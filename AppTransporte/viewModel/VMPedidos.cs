using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    /// <summary>
    /// ViewModel for managing and displaying a list of orders (Pedidos).
    /// This class handles loading orders for either an admin or a specific user,
    /// provides filtering by status and a general search term, and exposes the
    /// order list for data binding.
    /// </summary>
    public class VMPedidos : BaseViewModel
    {
        private bool _isBusy;
        private ObservableCollection<Pedido> _allPedidos = new();
        private ObservableCollection<Pedido> _pedidosFiltrados = new();
        private string _estadoSeleccionado;
        private string _numero;

        /// <summary>
        /// Gets a list of possible order statuses for filtering.
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
        /// Gets or sets the filtered collection of orders to be displayed in the view.
        /// </summary>
        public ObservableCollection<Pedido> PedidosFiltrados
        {
            get => _pedidosFiltrados;
            set => SetProperty(ref _pedidosFiltrados, value);
        }

        /// <summary>
        /// Gets or sets the selected status for filtering the order list.
        /// When set, it triggers the filtering logic.
        /// </summary>
        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (SetProperty(ref _estadoSeleccionado, value))
                {
                    FiltrarPedidos();
                }
            }
        }

        /// <summary>
        /// Gets or sets the search term used for filtering orders.
        /// The filter checks the Order ID, Origin, and Destination fields.
        /// </summary>
        public string Numero
        {
            get => _numero;
            set
            {
                if (SetProperty(ref _numero, value))
                {
                    FiltrarPedidos();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMPedidos"/> class for an admin context.
        /// It loads all orders from the database.
        /// </summary>
        public VMPedidos()
        {
            InitializeEstados();
            CargarPedidos();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMPedidos"/> class for a specific user context.
        /// It loads only the orders associated with the given user ID.
        /// </summary>
        /// <param name="idUsuario">The ID of the user whose orders are to be loaded.</param>
        public VMPedidos(int idUsuario)
        {
            InitializeEstados();
            CargarPedidosPorUsuario(idUsuario);
        }

        /// <summary>
        /// Initializes the list of filterable statuses.
        /// </summary>
        private void InitializeEstados()
        {
            Estados = new List<string>
            {
                "Todos",
                "Pendiente",
                "En el punto de Carga",
                "En camino al destino",
                "Finalizado"
            };
        }

        /// <summary>
        /// Asynchronously loads orders for a specific user from the database.
        /// </summary>
        /// <param name="idUsuario">The user's ID.</param>
        public async void CargarPedidosPorUsuario(int idUsuario)
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var pedidos = await App.Database.ListarPedidosPorUsuario(idUsuario);
                _allPedidos = new ObservableCollection<Pedido>(pedidos);
                FiltrarPedidos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los pedidos: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Asynchronously loads all orders from the database (admin view).
        /// </summary>
        public async void CargarPedidos()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var pedidos = await App.Database.ListarPedidosAdminAsync();
                _allPedidos = new ObservableCollection<Pedido>(pedidos);
                FiltrarPedidos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los pedidos: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Filters the displayed list of orders based on the selected status and search number.
        /// </summary>
        private void FiltrarPedidos()
        {
            IEnumerable<Pedido> tempFiltered = _allPedidos;

            if (!string.IsNullOrEmpty(EstadoSeleccionado) && EstadoSeleccionado != "Todos")
            {
                tempFiltered = tempFiltered.Where(p => p.EstadoPedido == EstadoSeleccionado);
            }

            if (!string.IsNullOrEmpty(Numero))
            {
                tempFiltered = tempFiltered.Where(p =>
                    p.IdPedido.ToString().Contains(Numero, StringComparison.OrdinalIgnoreCase) ||
                    p.Destino.Contains(Numero, StringComparison.OrdinalIgnoreCase) ||
                    p.Origen.Contains(Numero, StringComparison.OrdinalIgnoreCase));
            }

            PedidosFiltrados = new ObservableCollection<Pedido>(tempFiltered);
        }
    }
}