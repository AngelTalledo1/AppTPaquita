using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTransporte.model;

namespace AppTransporte.viewModel
{
    internal class ClienteViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Cliente> Clientes { get; set; } = new();

        private ObservableCollection<Cliente> _allClientes = new(); // Lista completa sin filtros
        private string _searchText;
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

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    FiltrarClientes(); // Aplicar filtro
                }
            }
        }

        public ClienteViewModel()
        {
            CargarClientes();
        }
        public async Task ActualizarDatos()
        {
            _allClientes.Clear();
            Clientes.Clear();
            CargarClientes();
        }


        private async void CargarClientes()
        {
            IsBusy = true;

            var clientes = await App.Database.ObtenerClientesAsync();

            _allClientes.Clear();
            foreach (var cliente in clientes)
            {
                _allClientes.Add(cliente); // Guardar en la lista completa
            }

            FiltrarClientes(); // Mostrar inicialmente todos los clientes
            IsBusy = false;
        }

        private void FiltrarClientes()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Mostrar todos los clientes si no hay texto de búsqueda
                Clientes = new ObservableCollection<Cliente>(_allClientes);
            }
            else
            {
                // Filtrar clientes por nombre
                var filtered = _allClientes
                    .Where(c => c.NombreCompleto.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);

                Clientes = new ObservableCollection<Cliente>(filtered);
            }

            OnPropertyChanged(nameof(Clientes));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
