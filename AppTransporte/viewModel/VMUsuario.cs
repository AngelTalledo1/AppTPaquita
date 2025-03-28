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
    internal class VMUsuario : INotifyPropertyChanged
    {
        public ObservableCollection<Usuario> Usuarios { get; set; } = new();
        public event PropertyChangedEventHandler? PropertyChanged;
        private bool isBusy;
        private bool mostrarSoloActivos = true; // Por defecto, solo mostrar usuarios activos
        private string textoBusqueda;

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public bool MostrarSoloActivos
        {
            get => mostrarSoloActivos;
            set
            {
                mostrarSoloActivos = value;
                OnPropertyChanged(nameof(MostrarSoloActivos));
                CargarUsuarios();
            }
        }

        public string TextoBusqueda
        {
            get => textoBusqueda;
            set
            {
                textoBusqueda = value;
                OnPropertyChanged(nameof(TextoBusqueda));
                // Aquí podrías implementar la búsqueda
            }
        }

        public VMUsuario()
        {
            CargarUsuarios();
        }

        private async void CargarUsuarios()
        {
            IsBusy = true;

            // Pasar el filtro de estado (null para todos, true para activos)
            var usuarios = await App.Database.ObtenerUsuariosAsync(MostrarSoloActivos ? true : (bool?)null);

            Usuarios.Clear();
            foreach (var usuario in usuarios)
            {
                Usuarios.Add(usuario);
            }

            IsBusy = false;
        }

        public async Task ActualizarDatos()
        {
            Usuarios.Clear();
            CargarUsuarios();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
