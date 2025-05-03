using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
namespace AppTransporte.viewModel
#pragma warning disable CS8612, CS8602, CS8604, CS8601, CS4014, CS8625, CS8616, CS8618
{
    public class VMTrabajadores : INotifyPropertyChanged
    {
        public ObservableCollection<Trabajador> Trabajadores { get; set; } = new();
        private ObservableCollection<Trabajador> _allTrabajadores = new();
        public ObservableCollection<Trabajador> Ayudantes { get; set; } = new();
        public ObservableCollection<Trabajador> Transportistas { get; set; } = new();

        // Lista de categorías para el Picker
        public ObservableCollection<string> Categorias { get; set; } = new ObservableCollection<string>();

        // Propiedad para la categoría seleccionada en el Picker
        private string _categoriaSeleccionada;
        public string CategoriaSeleccionada
        {
            get => _categoriaSeleccionada;
            set
            {
                if (_categoriaSeleccionada != value)
                {
                    _categoriaSeleccionada = value;
                    OnPropertyChanged(nameof(CategoriaSeleccionada));
                    // Actualizar el filtro cuando cambia la selección
                    CategoriaFiltro = value;
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    FiltrarTrabajadores();
                }
            }
        }
        // Nueva propiedad para filtrar por categoría
        private string _categoriaFiltro;
        public string CategoriaFiltro
        {
            get => _categoriaFiltro;
            set
            {
                if (_categoriaFiltro != value)
                {
                    _categoriaFiltro = value?.Length > 20 ? value.Substring(0, 20) : value; // Validación de longitud
                    OnPropertyChanged(nameof(CategoriaFiltro));
                    CargarTrabajadores(_categoriaFiltro); // Recargar con filtro
                }
            }
        }

        public VMTrabajadores()
        {
            // Inicializar las categorías
            CargarCategorias();
            CargarTrabajadores("Ayudante");
            CargarTrabajadores("Transportista");
        }

        private void CargarCategorias()
        {
            // Añadir las categorías disponibles
            Categorias.Clear();
            // Añadir una opción para mostrar todos
            Categorias.Add("Todos");
            Categorias.Add("Ayudante");
            Categorias.Add("Transportista");
            // Añade más categorías según necesites

            // Establecer un valor predeterminado
            CategoriaSeleccionada = "Todos";
            OnPropertyChanged(nameof(Categorias));
        }

        public async Task ActualizarDatos()
        {
            _allTrabajadores.Clear();
            Trabajadores.Clear();
            await CargarTrabajadores(CategoriaFiltro);
        }

        private async Task CargarTrabajadores(string categoria = null)
        {
            IsBusy = true;
            var trabajadores = await App.Database.ObtenerTrabajadoresAsync(categoria == "Todos" ? null : categoria);

            if (categoria == "Ayudante")
            {
                Ayudantes.Clear();
                foreach (var t in trabajadores) Ayudantes.Add(t);
            }
            else if (categoria == "Transportista")
            {
                Transportistas.Clear();
                foreach (var t in trabajadores) Transportistas.Add(t);
            }
            else if (categoria == "Todos" || string.IsNullOrEmpty(categoria))
            {
                // Si es "Todos" o null, cargar ambas categorías
                var ayudantes = await App.Database.ObtenerTrabajadoresAsync("Ayudante");
                var transportistas = await App.Database.ObtenerTrabajadoresAsync("Transportista");

                Ayudantes.Clear();
                foreach (var t in ayudantes) Ayudantes.Add(t);

                Transportistas.Clear();
                foreach (var t in transportistas) Transportistas.Add(t);
            }

            _allTrabajadores.Clear();
            foreach (var trabajador in trabajadores)
            {
                _allTrabajadores.Add(trabajador);
            }

            OnPropertyChanged(nameof(Ayudantes));
            OnPropertyChanged(nameof(Transportistas));
            FiltrarTrabajadores();
            IsBusy = false;
        }

        private void FiltrarTrabajadores()
        {
            var filtered = _allTrabajadores.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(t =>
                    t.NombreCompleto.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // Filtrar por categoría si hay una seleccionada que no sea "Todos"
            if (!string.IsNullOrWhiteSpace(CategoriaFiltro) && CategoriaFiltro != "Todos")
            {
                filtered = filtered.Where(t => t.Categoria == CategoriaFiltro);
            }

            Trabajadores = new ObservableCollection<Trabajador>(filtered);
            OnPropertyChanged(nameof(Trabajadores));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
