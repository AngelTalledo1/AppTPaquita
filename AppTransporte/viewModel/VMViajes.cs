using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AppTransporte.model;

namespace AppTransporte.viewModel
{
    public class VMViajes : INotifyPropertyChanged
    {
        private bool _isBusy;
        public ObservableCollection<Viaje> viajes { get; set; } = new();
        public ObservableCollection<Viaje> viajesFiltrados { get; set; } = new();
        private int? _idPedidoSeleccionado;
        private int? _idUsuario; // Nuevo campo para el idUsuario

        public event PropertyChangedEventHandler? PropertyChanged;
        public List<string> Estados { get; set; }
        private string _estadoSeleccionado;
        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (_estadoSeleccionado != value)
                {
                    _estadoSeleccionado = value;
                    FiltrarViajes();
                    OnPropertyChanged(nameof(EstadoSeleccionado));
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        public int? IdPedidoSeleccionado
        {
            get => _idPedidoSeleccionado;
            set
            {
                if (_idPedidoSeleccionado != value)
                {
                    _idPedidoSeleccionado = value;
                    OnPropertyChanged(nameof(IdPedidoSeleccionado));
                    InicializarViajes(); // Ahora recarga desde la base de datos
                }
            }
        }

        // Nuevo propiedad para el idUsuario
        public int? IdUsuario
        {
            get => _idUsuario;
            set
            {
                if (_idUsuario != value)
                {
                    _idUsuario = value;
                    OnPropertyChanged(nameof(IdUsuario));
                    InicializarViajes(); // Recarga datos al cambiar
                }
            }
        }

        public int TotalBarrilesFinalizados
        {
            get => viajesFiltrados
                .Where(v => v.ultEstado == "Finalizado")
                .Sum(v => v.Cantidad ?? 0);
        }

        public string BarrilesMostrados
        {
            get
            {
                var totalBarrilesFinalizados = TotalBarrilesFinalizados;
                var totalBarrilesPedido = viajesFiltrados.Sum(v => v.Cantidad ?? 0);
                return $"{totalBarrilesFinalizados} / {totalBarrilesPedido}";
            }
        }

        public VMViajes()
        {
            InicializarPropiedades();
        }
        public VMViajes(int idPedido)
        {
            this.IdPedidoSeleccionado = idPedido;
            InicializarPropiedades();
        }

        // Nuevo constructor para recibir idUsuario
        public VMViajes(int? idUsuario)
        {
            this.IdUsuario = idUsuario;
            InicializarPropiedades();
        }
        private void InicializarPropiedades()
        {
            Estados = new List<string>
            {
                "Todos",
                "Pendiente",
                "En el punto de Carga",
                "En camino al destino",
                "Finalizado"
            };
            InicializarViajes();
        }
        private async void InicializarViajes()
        {
            IsBusy = true;

            try
            {
                // Pasa ambos parámetros al método de la base de datos
                var viajesDesdeBD = await App.Database.ObtenerViajesModAsync(IdPedidoSeleccionado, IdUsuario);

                this.viajes.Clear();

                foreach (var viaje in viajesDesdeBD)
                {
                    this.viajes.Add(viaje);
                }

                FiltrarViajes();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los viajes: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FiltrarViajes()
        {
            if (viajes == null || !viajes.Any())
            {
                viajesFiltrados.Clear();
                ActualizarPropiedades();
                return;
            }

            var viajesFiltradosTemp = viajes.AsEnumerable();

            // Solo filtra por estado (el filtro de pedido y usuario se aplica en la base de datos)
            if (!string.IsNullOrEmpty(EstadoSeleccionado) && EstadoSeleccionado != "Todos")
            {
                viajesFiltradosTemp = viajesFiltradosTemp.Where(p => p.ultEstado == EstadoSeleccionado);
            }

            viajesFiltrados.Clear();

            foreach (var viaje in viajesFiltradosTemp)
            {
                viajesFiltrados.Add(viaje);
            }

            ActualizarPropiedades();
        }

        private void ActualizarPropiedades()
        {
            OnPropertyChanged(nameof(TotalBarrilesFinalizados));
            OnPropertyChanged(nameof(BarrilesMostrados));
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}