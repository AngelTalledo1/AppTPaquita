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

        public ClienteViewModel()
        {
            CargarClientes();
        }

        private async void CargarClientes()
        {
            IsBusy = true;

            var clientes = await App.Database.ObtenerClientesAsync();
            
            Clientes.Clear();

            foreach (var cliente in clientes)
            {
                Clientes.Add(cliente);
            }

            IsBusy = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
