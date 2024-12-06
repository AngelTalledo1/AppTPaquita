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

        public VMTrabajadores()
        {
            CargarTrabajadores();
        }
        public async Task ActualizarDatos()
        {
            Trabajadores.Clear();
            CargarTrabajadores();
        }
            private async void CargarTrabajadores()
        {
            IsBusy = true;

            var trabajadores = await App.Database.ObtenerTrabajadoresAsync();

            Trabajadores.Clear();

            foreach (var cliente in trabajadores)
            {
                Trabajadores.Add(cliente);
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
