using AppTransporte.model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    /// <summary>
    /// ViewModel for managing and displaying lists of workers (Trabajadores).
    /// This class handles loading workers from the database and provides separate collections
    /// for all workers, assistants (Ayudantes), and drivers (Transportistas).
    /// It also includes search functionality.
    /// </summary>
    public class VMTrabajadores : BaseViewModel
    {
        private ObservableCollection<Trabajador> _trabajadores = new();
        private readonly ObservableCollection<Trabajador> _allTrabajadores = new();
        private ObservableCollection<Trabajador> _ayudantes = new();
        private ObservableCollection<Trabajador> _transportistas = new();
        private bool _isBusy;
        private string _searchText;

        /// <summary>
        /// Gets or sets the main collection of workers displayed in the UI.
        /// This list is filtered by the <see cref="SearchText"/>.
        /// </summary>
        public ObservableCollection<Trabajador> Trabajadores
        {
            get => _trabajadores;
            set => SetProperty(ref _trabajadores, value);
        }

        /// <summary>
        /// Gets or sets a collection containing only workers categorized as 'Ayudante'.
        /// </summary>
        public ObservableCollection<Trabajador> Ayudantes
        {
            get => _ayudantes;
            set => SetProperty(ref _ayudantes, value);
        }

        /// <summary>
        /// Gets or sets a collection containing only workers categorized as 'Transportista' (Driver).
        /// </summary>
        public ObservableCollection<Trabajador> Transportistas
        {
            get => _transportistas;
            set => SetProperty(ref _transportistas, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is busy loading data.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Gets or sets the text used to filter the main <see cref="Trabajadores"/> list.
        /// The filter is case-insensitive and checks the worker's full name.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FiltrarTrabajadores();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMTrabajadores"/> class.
        /// </summary>
        public VMTrabajadores()
        {
            InitializeAsync();
        }

        /// <summary>
        /// Asynchronously refreshes all worker data from the database.
        /// </summary>
        public async Task ActualizarDatos()
        {
            await InitializeAsync();
        }

        /// <summary>
        /// Asynchronously loads and categorizes all workers from the database.
        /// This method populates the main worker list as well as the specialized
        /// 'Ayudantes' and 'Transportistas' collections.
        /// </summary>
        private async Task InitializeAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var allWorkers = await App.Database.ObtenerTrabajadoresAsync();

                _allTrabajadores.Clear();
                Ayudantes.Clear();
                Transportistas.Clear();

                foreach (var worker in allWorkers)
                {
                    _allTrabajadores.Add(worker);
                    if (worker.categoria.Equals("Ayudante", StringComparison.OrdinalIgnoreCase))
                    {
                        Ayudantes.Add(worker);
                    }
                    else if (worker.categoria.Equals("Transportista", StringComparison.OrdinalIgnoreCase))
                    {
                        Transportistas.Add(worker);
                    }
                }

                FiltrarTrabajadores();
            }
            catch (Exception ex)
            {
                // In a real application, consider a more robust logging mechanism.
                Console.WriteLine($"Error loading workers: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Filters the <see cref="Trabajadores"/> collection based on the <see cref="SearchText"/>.
        /// If the search text is empty, all workers are displayed.
        /// </summary>
        private void FiltrarTrabajadores()
        {
            var tempFiltered = string.IsNullOrWhiteSpace(SearchText)
                ? _allTrabajadores
                : _allTrabajadores.Where(t => t.NombreCompleto.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);

            Trabajadores = new ObservableCollection<Trabajador>(tempFiltered);
        }
    }
}