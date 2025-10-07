using AppTransporte.model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    /// <summary>
    /// ViewModel for managing and displaying a list of clients.
    /// This class handles loading client data, provides search functionality,
    /// and exposes the client list for data binding in the UI.
    /// </summary>
    public class ClienteViewModel : BaseViewModel
    {
        private ObservableCollection<Cliente> _clientes = new();
        /// <summary>
        /// Gets or sets the collection of clients to be displayed in the view.
        /// This collection is filtered based on the <see cref="SearchText"/>.
        /// </summary>
        public ObservableCollection<Cliente> Clientes
        {
            get => _clientes;
            set => SetProperty(ref _clientes, value);
        }

        private readonly ObservableCollection<Cliente> _allClientes = new();
        private string _searchText;
        private bool _isBusy;

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is currently busy loading data.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Gets or sets the text used to filter the <see cref="Clientes"/> collection.
        /// When set, it triggers the <see cref="FiltrarClientes"/> method.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FiltrarClientes();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClienteViewModel"/> class.
        /// It triggers the initial loading of clients.
        /// </summary>
        public ClienteViewModel()
        {
            CargarClientes();
        }

        /// <summary>
        /// Asynchronously refreshes the client data from the database.
        /// </summary>
        public async Task ActualizarDatos()
        {
            await CargarClientes();
        }

        /// <summary>
        /// Asynchronously loads the list of all clients from the database into memory.
        /// It populates the internal full list and then applies the current filter.
        /// </summary>
        private async Task CargarClientes()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var clientes = await App.Database.ObtenerClientesAsync();
                _allClientes.Clear();
                foreach (var cliente in clientes)
                {
                    _allClientes.Add(cliente);
                }
                FiltrarClientes();
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Filters the <see cref="Clientes"/> collection based on the <see cref="SearchText"/>.
        /// If the search text is empty, all clients are displayed.
        /// The filter is case-insensitive and checks the client's full name.
        /// </summary>
        private void FiltrarClientes()
        {
            var tempFiltered = string.IsNullOrWhiteSpace(SearchText)
                ? _allClientes
                : _allClientes.Where(c => c.NombreCompleto.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);

            Clientes.Clear();
            foreach (var cliente in tempFiltered)
            {
                Clientes.Add(cliente);
            }
        }
    }
}