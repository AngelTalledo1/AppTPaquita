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
    public class VMEmpresas : INotifyPropertyChanged
    {
        public ObservableCollection<Empresa> Empresas { get; set; } = new();
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
                CargarEmpresas(textoBusqueda);
            }
        }

        public VMEmpresas()
        {
            CargarEmpresas();
        }

        public async Task ActualizarDatos()
        {
            Empresas.Clear();
            CargarEmpresas();
        }

        private async void CargarEmpresas(string filtro = null)
        {
            IsBusy = true;

            var empresas = await App.Database.ObtenerEmpresasAsync(filtro);

            Empresas.Clear();
            foreach (var empresa in empresas)
            {
                Empresas.Add(empresa);
            }

            IsBusy = false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
