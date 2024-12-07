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
    internal class VMUbicacion : INotifyPropertyChanged
    {
        private bool _isBusy;
        public ObservableCollection<Ubicacion> Ubicaciones { get; set; } = new();
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public VMUbicacion()
        {
            CargarUbicaciones();
        }
        private async void CargarUbicaciones()
        {
            IsBusy = true;

            var ubicaciones = await App.Database.ObtenerUbicacionesAsync();

            Ubicaciones.Clear();
            foreach (var ubicacion in ubicaciones)
            {
                Ubicaciones.Add(ubicacion);
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
