using AppTransporte.model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;


namespace AppTransporte.viewModel
{
    internal class VMServicio : INotifyPropertyChanged
    {
        public ObservableCollection<Servicio> Servicios { get; set; } = new();
        public event PropertyChangedEventHandler? PropertyChanged;
        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        private string textoBusqueda;
        public string TextoBusqueda
        {
            get => textoBusqueda;
            set
            {
                textoBusqueda = value;
                OnPropertyChanged(nameof(TextoBusqueda));
                CargarServicios(textoBusqueda);
            }
        }

        public VMServicio()
        {
            CargarServicios();
        }

        private async void CargarServicios(string filtro = null)
        {
            IsBusy = true;

            var servicios = await App.Database.ObtenerServiciosAsync(filtro);

            Servicios.Clear();
            foreach (var servicio in servicios)
            {
                Servicios.Add(servicio);
            }

            IsBusy = false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
