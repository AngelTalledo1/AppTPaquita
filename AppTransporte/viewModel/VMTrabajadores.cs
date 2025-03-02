using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    public class VMTrabajadores : INotifyPropertyChanged
    {
        public ObservableCollection<Trabajador> Trabajadores { get; set; } = new();
        private ObservableCollection<Trabajador> _allTrabajadores = new();
        public ObservableCollection<Trabajador> Ayudantes { get; set; } = new();
        public ObservableCollection<Trabajador> Transportistas { get; set; } = new();

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
            CargarTrabajadores("Ayudante");
            CargarTrabajadores("Transportista");
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

            var trabajadores = await App.Database.ObtenerTrabajadoresAsync(categoria);
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