using AppTransporte.model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    /// <summary>
    /// ViewModel for managing and displaying a list of locations (Ubicaciones).
    /// This class handles loading location data from the database, provides search functionality,
    /// and exposes the filtered list for data binding in the UI.
    /// </summary>
    public class VMUbicacion : BaseViewModel
    {
        private string _textoBusqueda;
        private bool _isBusy;
        private readonly ObservableCollection<Ubicacion> _allUbicaciones = new();
        private ObservableCollection<Ubicacion> _ubicacionesFiltradas = new();

        /// <summary>
        /// Gets or sets the text used to filter the locations list.
        /// When set, it triggers the filtering logic.
        /// </summary>
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (SetProperty(ref _textoBusqueda, value))
                {
                    FiltrarUbicaciones();
                }
            }
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
        /// Gets or sets the filtered collection of locations to be displayed in the view.
        /// </summary>
        public ObservableCollection<Ubicacion> UbicacionesFiltradas
        {
            get => _ubicacionesFiltradas;
            set => SetProperty(ref _ubicacionesFiltradas, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMUbicacion"/> class.
        /// It triggers the initial loading of locations.
        /// </summary>
        public VMUbicacion()
        {
            CargarUbicaciones();
        }

        /// <summary>
        /// Asynchronously loads all locations from the database into memory.
        /// </summary>
        private async void CargarUbicaciones()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var ubicaciones = await App.Database.ObtenerUbicacionesAsync();
                _allUbicaciones.Clear();
                foreach (var ubicacion in ubicaciones)
                {
                    _allUbicaciones.Add(ubicacion);
                }
                FiltrarUbicaciones();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading locations: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Filters the displayed list of locations based on the <see cref="TextoBusqueda"/>.
        /// The filter is case-insensitive and checks the location's description.
        /// </summary>
        private void FiltrarUbicaciones()
        {
            var tempFiltered = string.IsNullOrWhiteSpace(TextoBusqueda)
                ? _allUbicaciones
                : _allUbicaciones.Where(u => u.Descripcion.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase));

            UbicacionesFiltradas = new ObservableCollection<Ubicacion>(tempFiltered);
        }
    }
}