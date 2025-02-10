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

            public int? IdPedidoSeleccionado
            {
                get => _idPedidoSeleccionado;
                set
                {
                    if (_idPedidoSeleccionado != value)
                    {
                        _idPedidoSeleccionado = value;
                        OnPropertyChanged(nameof(IdPedidoSeleccionado));
                        FiltrarViajes(); // Filtra los viajes automáticamente al cambiar el IdPedido
                    }
                }
            }

        public int TotalBarrilesFinalizados
        {
            get => viajesFiltrados
                .Where(v => v.ultEstado == "Finalizado")
                .Sum(v => v.Cantidad ?? 0); // Si v.Cantidad es null, tomará 0
        }
        public string BarrilesMostrados
        {
            get
            {
                var totalBarrilesFinalizados = TotalBarrilesFinalizados;
                var totalBarrilesPedido = viajesFiltrados.Sum(v => v.Cantidad ?? 0); // Total de barriles en todos los viajes
                return $"{totalBarrilesFinalizados} / {totalBarrilesPedido}";
            }
        }
        public VMViajes()
        {
            InicializarViajes();
        }
        public VMViajes(int idPedido)
        {
            this.IdPedidoSeleccionado = idPedido;
            InicializarViajes(); // Carga los viajes y aplica el filtro automáticamente
        }


        private async void InicializarViajes()
        {
            IsBusy = true;

            try
            {
                var viajesDesdeBD = await App.Database.ObtenerViajesAsync();

                this.viajes.Clear();

                foreach (var viaje in viajesDesdeBD)
                {
                    this.viajes.Add(viaje);
                }

                FiltrarViajes(); // Aplica el filtro después de cargar los datos
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
                OnPropertyChanged(nameof(TotalBarrilesFinalizados));
                OnPropertyChanged(nameof(BarrilesMostrados));
                return;
            }

            var viajesFiltradosTemp = viajes.AsEnumerable();

            if (IdPedidoSeleccionado.HasValue)
            {
                viajesFiltradosTemp = viajesFiltradosTemp.Where(v => v.IdPedido == IdPedidoSeleccionado.Value);
            }

            viajesFiltrados.Clear();

            foreach (var viaje in viajesFiltradosTemp)
            {
                viajesFiltrados.Add(viaje);
            }
            OnPropertyChanged(nameof(TotalBarrilesFinalizados));
            OnPropertyChanged(nameof(BarrilesMostrados));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    }
