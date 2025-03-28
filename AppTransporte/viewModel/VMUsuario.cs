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
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public VMUsuario()
        {
            CargarUsuarios();
        }
        private async void CargarUsuarios()
        {
            IsBusy = true;

            var usuarios = await App.Database.ObtenerUsuariosAsync();

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
