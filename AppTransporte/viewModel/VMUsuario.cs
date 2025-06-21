using AppTransporte.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS8602, CS1998, CS8618

namespace AppTransporte.viewModel
{
    internal class VMUsuario : INotifyPropertyChanged
    {
        public ObservableCollection<Usuario> Usuarios { get; set; } = new();
        public ObservableCollection<Usuario> UsuariosFiltrados { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        private bool isBusy;
        private bool mostrarSoloActivos = true;
        private string textoBusqueda = "";

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
                FiltrarUsuarios();
            }
        }

        public VMUsuario()
        {
            CargarUsuarios();
        }

        private async void CargarUsuarios()
        {
            IsBusy = true;
            try
            {
                // Pasar el filtro de estado (null para todos, true para activos)
                var usuarios = await App.Database.ObtenerUsuariosAsync(MostrarSoloActivos ? true : (bool?)null);

                Usuarios.Clear();
                foreach (var usuario in usuarios)
                {
                    Usuarios.Add(usuario);
                }

                FiltrarUsuarios();
            }
            catch (Exception ex)
            {
                // Manejo de errores si es necesario
                System.Diagnostics.Debug.WriteLine($"Error al cargar usuarios: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FiltrarUsuarios()
        {
            UsuariosFiltrados.Clear();

            var usuariosFiltrados = Usuarios.AsEnumerable();

            // Aplicar filtro de búsqueda si hay texto
            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                string busqueda = TextoBusqueda.ToLower().Trim();
                usuariosFiltrados = usuariosFiltrados.Where(u =>
                    (u.Username?.ToLower().Contains(busqueda) ?? false) ||
                    (u.Nombres?.ToLower().Contains(busqueda) ?? false) ||
                    (u.Apellidos?.ToLower().Contains(busqueda) ?? false) ||
                    ($"{u.Nombres} {u.Apellidos}".ToLower().Contains(busqueda))
                );
            }

            foreach (var usuario in usuariosFiltrados)
            {
                UsuariosFiltrados.Add(usuario);
            }
        }

        public async Task ActualizarDatos()
        {
            Usuarios.Clear();
            UsuariosFiltrados.Clear();
            CargarUsuarios();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}