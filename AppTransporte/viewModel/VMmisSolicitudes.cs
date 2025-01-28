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
    public class VMmisSolicitudes : INotifyPropertyChanged
    {


        public ObservableCollection<Solicitud> Solicitudes { get; set; } = new ObservableCollection<Solicitud>();
        private bool _isBusy;

        private string _textoBusquedaCliente;

        public string TextoBusquedaCliente
        {
            get => _textoBusquedaCliente;
            set
            {
                if (_textoBusquedaCliente != value)
                {
                    _textoBusquedaCliente = value;
                    Filtrar(); // Aplica el filtro al cambiar el texto de búsqueda
                    OnPropertyChanged(nameof(TextoBusquedaCliente));
                }
            }
        }
        public List<string> EstadoSolicitud { get; set; }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        private ObservableCollection<Solicitud> _solicitudesFiltradas = new ObservableCollection<Solicitud>();
        public ObservableCollection<Solicitud> SolicitudesFiltradas
        {
            get => _solicitudesFiltradas;
            set
            {
                _solicitudesFiltradas = value;
                OnPropertyChanged(nameof(SolicitudesFiltradas));
            }
        }

        private string _estadoSeleccionado;

        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (_estadoSeleccionado != value)
                {
                    _estadoSeleccionado = value;
                    Filtrar();
                    OnPropertyChanged(nameof(EstadoSeleccionado));
                }
            }
        }
        public VMmisSolicitudes()
        {
            EstadoSeleccionado = "Por revisar";
            TextoBusquedaCliente = string.Empty; // Inicializa el texto de búsqueda vacío
            CargarSolicitudes();
            EstadoSolicitud = new List<string>
        {
            "Todos",
            "Por revisar",
            "Pedido Creado",
            "Cancelada"
        };
        }

        private async void CargarSolicitudes()
        {
            IsBusy = true;

            var solicitudes = await App.Database.ObtenerSolicitudesAsync();

            // Limpiar las solicitudes previas y agregar las nuevas
            Solicitudes.Clear();

            foreach (var solicitud in solicitudes)
            {
                Solicitudes.Add(solicitud);
            }
            Filtrar();

            IsBusy = false;
        }


        private void Filtrar()
        {
            var solicitudesFiltradas = Solicitudes.AsEnumerable();

            // Filtro por estado seleccionado
            if (EstadoSeleccionado != "Todos")
            {
                solicitudesFiltradas = solicitudesFiltradas.Where(s => s.EstadoSolicitud == EstadoSeleccionado);
            }

            // Filtro por texto de cliente
            if (!string.IsNullOrWhiteSpace(TextoBusquedaCliente))
            {
                solicitudesFiltradas = solicitudesFiltradas.Where(s =>
                    s.Cliente.Contains(TextoBusquedaCliente, StringComparison.OrdinalIgnoreCase));
            }

            // Actualiza las solicitudes filtradas
            SolicitudesFiltradas = new ObservableCollection<Solicitud>(solicitudesFiltradas);
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
