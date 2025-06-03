using AppTransporte.model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppTransporte.viewModel
{
    public class VMAsignarViaje : INotifyPropertyChanged
    {
        private ObservableCollection<Trabajador> _transportistas = new();
        private ObservableCollection<Trabajador> _ayudantes = new();
        private ObservableCollection<Vehiculo> _cisternas = new();
        private ObservableCollection<Vehiculo> _tractos = new();
        private bool _isBusy = false;

        public ObservableCollection<Trabajador> Transportistas
        {
            get => _transportistas;
            set
            {
                _transportistas = value;
                OnPropertyChanged(nameof(Transportistas));
            }
        }

        public ObservableCollection<Trabajador> Ayudantes
        {
            get => _ayudantes;
            set
            {
                _ayudantes = value;
                OnPropertyChanged(nameof(Ayudantes));
            }
        }

        public ObservableCollection<Vehiculo> Cisternas
        {
            get => _cisternas;
            set
            {
                _cisternas = value;
                OnPropertyChanged(nameof(Cisternas));
            }
        }

        public ObservableCollection<Vehiculo> Tractos
        {
            get => _tractos;
            set
            {
                _tractos = value;
                OnPropertyChanged(nameof(Tractos));
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public VMAsignarViaje()
        {
            // Constructor vacío
        }

        public async Task CargarDatosAsync()
        {
            try
            {
                IsBusy = true;

                // Cargar trabajadores disponibles (no asignados a viajes activos)
                var transportistasDisponibles = await App.Database.ObtenerTrabajadoresDisponiblesAsync("Transportista");
                var ayudantesDisponibles = await App.Database.ObtenerTrabajadoresDisponiblesAsync("Ayudante");

                Transportistas.Clear();
                Ayudantes.Clear();

                foreach (var transportista in transportistasDisponibles)
                {
                    Transportistas.Add(transportista);
                }

                foreach (var ayudante in ayudantesDisponibles)
                {
                    Ayudantes.Add(ayudante);
                }

                // Cargar vehículos disponibles (no asignados a viajes activos)
                var cisternasDisponibles = await App.Database.ObtenerCisternasDisponiblesAsync();
                var tractosDisponibles = await App.Database.ObtenerTractosDisponiblesAsync();

                Cisternas.Clear();
                Tractos.Clear();

                foreach (var cisterna in cisternasDisponibles)
                {
                    Cisternas.Add(cisterna);
                }

                foreach (var tracto in tractosDisponibles)
                {
                    Tractos.Add(tracto);
                }

                // DEBUG
                System.Diagnostics.Debug.WriteLine($"=== RECURSOS DISPONIBLES ===");
                System.Diagnostics.Debug.WriteLine($"Transportistas disponibles: {Transportistas.Count}");
                System.Diagnostics.Debug.WriteLine($"Ayudantes disponibles: {Ayudantes.Count}");
                System.Diagnostics.Debug.WriteLine($"Cisternas disponibles: {Cisternas.Count}");
                System.Diagnostics.Debug.WriteLine($"Tractos disponibles: {Tractos.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}