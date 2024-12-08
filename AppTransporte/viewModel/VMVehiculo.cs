    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using AppTransporte.model;
    using System.Threading.Tasks;
    using AppTransporte.Interfaces;
    using System.Windows.Input;

    namespace AppTransporte.viewModel
    {
        public class VMVehiculo : INotifyPropertyChanged
        {
        public ObservableCollection<Vehiculo> Tractos { get; set; } = new ObservableCollection<Vehiculo>();
        public ObservableCollection<Vehiculo> Cisternas { get; set; } = new ObservableCollection<Vehiculo>();

        // Propiedades de filtrado
        public List<string> TipoVehiculo { get; set; }
        private string _filtroSeleccionado;
        public List<string> Vencimiento { get; set; }
        private string _ordenSeleccionado;

        public bool IsBusy { get; set; }

        private ObservableCollection<Vehiculo> _vehiculosFiltrados = new ObservableCollection<Vehiculo>();
        public ObservableCollection<Vehiculo> VehiculosFiltrados
        {
            get => _vehiculosFiltrados;
            set
            {
                _vehiculosFiltrados = value;
                OnPropertyChanged(nameof(VehiculosFiltrados));
            }
        }

        // Propiedad que va a manejar la opción seleccionada del Picker (Cisterna o Tracto)
        public string FiltroSeleccionado
        {
            get => _filtroSeleccionado;
            set
            {
                if (_filtroSeleccionado != value)
                {
                    _filtroSeleccionado = value;
                    FiltrarVehiculos();
                    OnPropertyChanged(nameof(FiltroSeleccionado));
                }
            }
        }
        public string OrdenSeleccionado
        {
            get => _ordenSeleccionado;
            set
            {
                if (_ordenSeleccionado != value)
                {
                    _ordenSeleccionado = value;
                    FiltrarVehiculos();
                    OnPropertyChanged(nameof(OrdenSeleccionado));
                }
            }
        }

        // Propiedad para el filtro de búsqueda con el SearchBar
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    FiltrarVehiculos(); // Se vuelve a filtrar cuando cambia el texto de búsqueda
                    OnPropertyChanged(nameof(SearchText));
                }
            }
        }

        public VMVehiculo()
        {
            TipoVehiculo = new List<string>
            {
                "Cisterna",
                "Tracto"
            };

            Vencimiento = new List<string>
            {
                "Poliza",
                "CITV",
                "Cubicacion"
            };
            CargarVehiculos();
        }

        // Método para cargar los vehículos (tractos y cisternas)
        private async void CargarVehiculos()
        {
            IsBusy = true;

            // Llamada para obtener vehículos según placa y orden
            var tractos = await App.Database.ObtenerTractoAsync(placa: SearchText, ordenarPor: OrdenSeleccionado);
            var cisternas = await App.Database.ObtenerCisternaAsync(placa: SearchText, ordenarPor: OrdenSeleccionado);

            // Limpiar las listas y agregar los nuevos
            Tractos.Clear();
            Cisternas.Clear();

            foreach (var tracto in tractos)
            {
                Tractos.Add(tracto);
            }

            foreach (var cisterna in cisternas)
            {
                Cisternas.Add(cisterna);
            }

            IsBusy = false;
            FiltrarVehiculos(); // Filtrar después de cargar
        }

        // Método para filtrar los vehículos en base al filtro seleccionado y el texto de búsqueda
        private void FiltrarVehiculos()
        {
            var vehiculosFiltrados = new List<Vehiculo>();

            // Filtrar según el tipo de vehículo seleccionado (tracto o cisterna)
            if (FiltroSeleccionado == "Tracto")
            {
                vehiculosFiltrados.AddRange(Tractos);
            }
            else if (FiltroSeleccionado == "Cisterna")
            {
                vehiculosFiltrados.AddRange(Cisternas);
            }

            // Filtrar por texto de búsqueda si es necesario
            if (!string.IsNullOrEmpty(SearchText))
            {
                vehiculosFiltrados = vehiculosFiltrados
                    .Where(v => v.Placa.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Asignar los vehículos filtrados a la propiedad ObservableCollection
            VehiculosFiltrados = new ObservableCollection<Vehiculo>(vehiculosFiltrados);
        }

        // Evento para notificar cambios en las propiedades
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
    }
