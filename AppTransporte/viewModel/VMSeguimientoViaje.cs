using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AppTransporte.model;


namespace AppTransporte.viewModel
{
    public class VMSeguimientoViaje : INotifyPropertyChanged
    {


        private bool _isBusy;
        public ObservableCollection<Seguimiento> seguimiento { get; set; } = new();
        public ObservableCollection<Seguimiento> seguimientoFiltrados { get; set; } = new();
        private int? _idViajeSeleccionado;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public int? IdViajeSeleccionado
        {
            get => _idViajeSeleccionado;
            set
            {
                if (_idViajeSeleccionado != value)
                {
                    _idViajeSeleccionado = value;
                    OnPropertyChanged(nameof(IdViajeSeleccionado));
                    FiltrarViajes(); // Filtra los viajes automáticamente al cambiar el IdPedido
                }
            }
        }
        public VMSeguimientoViaje()
        {
            InicializarViajes();
        }
        public VMSeguimientoViaje(int idViaje)
        {
            this.IdViajeSeleccionado = idViaje;
            InicializarViajes(); // Carga los viajes y aplica el filtro automáticamente
        }


        private async void InicializarViajes()
        {
            IsBusy = true;

            try
            {
                var seguimientoBD = await App.Database.ObtenerEstadosViaje();

                this.seguimiento.Clear();

                foreach (var segui in seguimientoBD)
                {
                    this.seguimiento.Add(segui);
                }

                FiltrarViajes(); // Aplica el filtro después de cargar los datos
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los estados: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FiltrarViajes()
        {
            if (seguimiento == null || !seguimiento.Any())
            {
                seguimientoFiltrados.Clear();
                return;
            }

            var seguimientoFiltradosTemp = seguimiento.AsEnumerable();

            if (IdViajeSeleccionado.HasValue)
            {
                seguimientoFiltradosTemp = seguimientoFiltradosTemp.Where(v => v.IdViaje == IdViajeSeleccionado.Value);
            }

            seguimientoFiltrados.Clear();

            foreach (var segui in seguimientoFiltradosTemp)
            {
                seguimientoFiltrados.Add(segui);
            }
        }

        

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
