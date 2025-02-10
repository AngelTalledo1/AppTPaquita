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
    public class VMUbicacion : INotifyPropertyChanged
    {
        private string _textoBusqueda;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (_textoBusqueda != value)
                {
                    _textoBusqueda = value;
                    OnPropertyChanged(nameof(TextoBusqueda));
                    FiltrarUbicaciones(); // Llama al método para filtrar las ubicaciones
                }
            }
        }
        private bool _isBusy;
        public ObservableCollection<Ubicacion> Ubicaciones { get; set; } = new();
        public ObservableCollection<Ubicacion> UbicacionesFiltradas { get; set; } = new();
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

            // Inicializa las ubicaciones filtradas con todas las ubicaciones
            FiltrarUbicaciones();

            IsBusy = false;
        }

        private void FiltrarUbicaciones()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                // Si no hay texto en el buscador, muestra todas las ubicaciones
                UbicacionesFiltradas.Clear();
                foreach (var ubicacion in Ubicaciones)
                {
                    UbicacionesFiltradas.Add(ubicacion);
                }
            }
            else
            {
                // Aplica el filtro basado en el texto ingresado
                var resultado = Ubicaciones.Where(u =>
                    u.Descripcion.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase));

                UbicacionesFiltradas.Clear();
                foreach (var ubicacion in resultado)
                {
                    UbicacionesFiltradas.Add(ubicacion);
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
