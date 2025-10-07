using AppTransporte.model;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AppTransporte.viewModel
{
    /// <summary>
    /// ViewModel for managing and displaying the tracking history (Seguimiento) of a specific trip.
    /// This class loads all tracking events and filters them for a selected trip ID.
    /// </summary>
    public class VMSeguimientoViaje : BaseViewModel
    {
        private bool _isBusy;
        private readonly ObservableCollection<Seguimiento> _allSeguimiento = new();
        private ObservableCollection<Seguimiento> _seguimientoFiltrados = new();
        private int? _idViajeSeleccionado;

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is busy loading data.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        /// <summary>
        /// Gets or sets the collection of filtered tracking events to be displayed in the view.
        /// </summary>
        public ObservableCollection<Seguimiento> SeguimientoFiltrados
        {
            get => _seguimientoFiltrados;
            set => SetProperty(ref _seguimientoFiltrados, value);
        }

        /// <summary>
        /// Gets or sets the ID of the trip for which to display tracking events.
        /// When set, it triggers the filtering logic.
        /// </summary>
        public int? IdViajeSeleccionado
        {
            get => _idViajeSeleccionado;
            set
            {
                if (SetProperty(ref _idViajeSeleccionado, value))
                {
                    FiltrarSeguimiento();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMSeguimientoViaje"/> class.
        /// </summary>
        public VMSeguimientoViaje()
        {
            InicializarSeguimiento();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VMSeguimientoViaje"/> class for a specific trip.
        /// </summary>
        /// <param name="idViaje">The ID of the trip to display tracking for.</param>
        public VMSeguimientoViaje(int idViaje)
        {
            IdViajeSeleccionado = idViaje;
            InicializarSeguimiento();
        }

        /// <summary>
        /// Asynchronously loads all tracking events from the database into memory.
        /// </summary>
        private async void InicializarSeguimiento()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var seguimientoBD = await App.Database.ObtenerEstadosViaje();
                _allSeguimiento.Clear();
                foreach (var segui in seguimientoBD)
                {
                    _allSeguimiento.Add(segui);
                }
                FiltrarSeguimiento();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los estados: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Filters the displayed list of tracking events based on the <see cref="IdViajeSeleccionado"/>.
        /// </summary>
        private void FiltrarSeguimiento()
        {
            var tempFiltered = _allSeguimiento.AsEnumerable();

            if (IdViajeSeleccionado.HasValue)
            {
                tempFiltered = tempFiltered.Where(v => v.IdViaje == IdViajeSeleccionado.Value);
            }

            SeguimientoFiltrados = new ObservableCollection<Seguimiento>(tempFiltered);
        }
    }
}