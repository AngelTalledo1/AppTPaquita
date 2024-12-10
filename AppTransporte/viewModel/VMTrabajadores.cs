using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    public class VMTrabajadores : INotifyPropertyChanged
    {
        public ObservableCollection<Trabajador> Trabajadores { get; set; } = new();
        private ObservableCollection<Trabajador> _allTrabajadores = new(); // Lista completa de trabajadores

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
                    FiltrarTrabajadores(); // Aplicar el filtro al cambiar el texto
                }
            }
        }

        public VMTrabajadores()
        {
            CargarTrabajadores();
        }

        public async Task ActualizarDatos()
        {
            _allTrabajadores.Clear();
            Trabajadores.Clear();
            CargarTrabajadores();
        }

        private async void CargarTrabajadores()
        {
            IsBusy = true;

            var trabajadores = await App.Database.ObtenerTrabajadoresAsync();

            _allTrabajadores.Clear();
            foreach (var trabajador in trabajadores)
            {
                _allTrabajadores.Add(trabajador); // Guardar en la lista completa
            }

            FiltrarTrabajadores(); // Mostrar todos inicialmente
            IsBusy = false;
        }

        private void FiltrarTrabajadores()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Mostrar todos si no hay texto de búsqueda
                Trabajadores = new ObservableCollection<Trabajador>(_allTrabajadores);
            }
            else
            {
                // Filtrar trabajadores por nombre
                var filtered = _allTrabajadores
                    .Where(t => t.NombreCompleto.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);

                Trabajadores = new ObservableCollection<Trabajador>(filtered);
            }

            OnPropertyChanged(nameof(Trabajadores));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
