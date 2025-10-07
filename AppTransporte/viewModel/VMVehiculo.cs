using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    /// <summary>
    /// ViewModel for managing and displaying lists of vehicles (Vehiculos).
    /// This class handles loading Tractos and Cisternas, providing UI collections,
    /// and filtering them based on type, search text, and sorting criteria.
    /// </summary>
    public class VMVehiculo : BaseViewModel
    {
        private bool _isBusy;
        private string _filtroSeleccionado = "Tracto"; // Default filter
        private string _ordenSeleccionado;
        private string _searchText;

        /// <summary>
        /// Gets a collection of all loaded Tracto vehicles from the database.
        /// </summary>
        public ObservableCollection<Vehiculo> Tractos { get; } = new();

        /// <summary>
        /// Gets a collection of all loaded Cisterna vehicles from the database.
        /// </summary>
        public ObservableCollection<Vehiculo> Cisternas { get; } = new();

        /// <summary>
        /// Gets the collection of vehicles currently displayed in the UI,
        /// based on the selected filters.
        /// </summary>
        public ObservableCollection<Vehiculo> VehiculosFiltrados { get; private set; } = new();

        /// <summary>
        /// Gets the list of vehicle types available for filtering.
        /// </summary>
        public List<string> TiposVehiculo { get; }

        /// <summary>
        /// Gets the list of properties available for sorting vehicles.
        /// </summary>
        public List<string> CriteriosOrden { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is busy loading data.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Gets or sets the selected vehicle type filter ("Cisterna" or "Tracto").
        /// Changing this value updates the displayed vehicle list.
        /// </summary>
        public string FiltroSeleccionado
        {
            get => _filtroSeleccionado;
            set
            {
                if (SetProperty(ref _filtroSeleccionado, value))
                {
                    UpdateVehiculosFiltrados();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected sorting criteria (e.g., "Poliza", "CITV").
        /// Changing this value triggers a reload of data from the database.
        /// </summary>
        public string OrdenSeleccionado
        {
            get => _ordenSeleccionado;
            set
            {
                if (SetProperty(ref _ordenSeleccionado, value))
                {
                    CargarVehiculosAsync();
                }
            }
        }

        /// <summary>
        /// Gets or sets the search text for filtering vehicles by license plate.
        /// Changing this value triggers a reload of data from the database.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    CargarVehiculosAsync();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMVehiculo"/> class.
        /// </summary>
        public VMVehiculo()
        {
            TiposVehiculo = new List<string> { "Tracto", "Cisterna" };
            CriteriosOrden = new List<string> { "Poliza", "CITV", "Cubicacion" };
            CargarVehiculosAsync();
        }

        /// <summary>
        /// Asynchronously loads vehicle data from the database based on current filter and sort settings.
        /// </summary>
        private async void CargarVehiculosAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                // Fetch both lists in parallel for efficiency
                var tractosTask = App.Database.ObtenerTractoAsync(SearchText, OrdenSeleccionado);
                var cisternasTask = App.Database.ObtenerCisternaAsync(SearchText, OrdenSeleccionado);
                await Task.WhenAll(tractosTask, cisternasTask);

                Tractos.Clear();
                foreach (var tracto in await tractosTask)
                {
                    Tractos.Add(tracto);
                }

                Cisternas.Clear();
                foreach (var cisterna in await cisternasTask)
                {
                    Cisternas.Add(cisterna);
                }

                UpdateVehiculosFiltrados();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading vehicles: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Updates the <see cref="VehiculosFiltrados"/> collection based on the selected vehicle type.
        /// </summary>
        private void UpdateVehiculosFiltrados()
        {
            ObservableCollection<Vehiculo> source = FiltroSeleccionado == "Tracto" ? Tractos : Cisternas;
            VehiculosFiltrados = new ObservableCollection<Vehiculo>(source);
            OnPropertyChanged(nameof(VehiculosFiltrados));
        }
    }
}