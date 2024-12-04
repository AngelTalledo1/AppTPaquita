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
                        //FiltrarViajes(); // Filtra los viajes automáticamente al cambiar el IdPedido
                    }
                }
            }
            public VMViajes()
            {
                CargarViajes();
            }
            public VMViajes(int idPedido)
            {
                this.IdPedidoSeleccionado = idPedido;
                CargarViajes();
            }

            private async void CargarViajes()
            {
                IsBusy = true;

                try
                {
                    var Viaje = await App.Database.ObtenerViajesAsync();

                    // Si viajes es null, asignamos una lista vacía
                    viajes.Clear();

                    foreach (var viaje in Viaje)
                    {
                        viajes.Add(viaje);
                    }  // Filtrar después de asignar los viajes
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

            //private void FiltrarViajes()
            //{
            //    // Verificar si Viajes está vacío o null
            //    if (Viajes == null || !Viajes.Any())
            //    {
            //        ViajesFiltrados = new List<Viaje>();
            //        return;
            //    }

            //    var viajesFiltrados = Viajes.AsEnumerable();

            //    if (IdPedidoSeleccionado.HasValue)
            //    {
            //        viajesFiltrados = viajesFiltrados.Where(v => v.IdPedido == IdPedidoSeleccionado.Value);
            //    }

            //    ViajesFiltrados = viajesFiltrados.ToList();
            //}
            protected  void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }


        }
    }
