using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppTransporte.model;

namespace AppTransporte.viewModel
{
    public class VMPedidos : INotifyPropertyChanged
    {

        private bool _isBusy;
        private List<Pedido> _pedidos;
        private List<Pedido> _pedidosFiltrados;
        private string _estadoSeleccionado;
        private string _numero;

        public List<string> Estados { get; set; }
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

        public List<Pedido> Pedidos
        {
            get => _pedidos;
            set
            {
                _pedidos = value;
                OnPropertyChanged(nameof(Pedidos));
            }
        }

        public List<Pedido> PedidosFiltrados
        {
            get => _pedidosFiltrados;
            set
            {
                _pedidosFiltrados = value;
                OnPropertyChanged(nameof(PedidosFiltrados));
            }
        }

        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (_estadoSeleccionado != value)
                {
                    _estadoSeleccionado = value;
                    FiltrarPedidos();
                    OnPropertyChanged(nameof(EstadoSeleccionado));
                }
            }
        }

        public string Numero
        {
            get => _numero;
            set
            {
                if (_numero != value)
                {
                    _numero = value;
                    FiltrarPedidos();
                    OnPropertyChanged(nameof(Numero));
                }
            }
        }
        public VMPedidos()
        {
            _pedidos = new List<Pedido>();
            Estados = new List<string>
            {
                "Todos",
                "Pendiente",
                "En el punto de Carga",
                "En camino al destino",
                "Finalizado"
            };

            CargarPedidos(); // Cargar los pedidos al inicializar el ViewModel
        }

        public async void CargarPedidos()
        {
            IsBusy = true; // Indicar que los datos están cargando

            try
            {
                // Llamar al método de la capa de datos
                var pedidos = await App.Database.ListarPedidosAdminAsync();

                // Asignar los pedidos obtenidos
                Pedidos = pedidos;

                // Aplicar los filtros iniciales
                FiltrarPedidos();
            }
            catch (Exception ex)
            {
                // Manejar cualquier error
                Console.WriteLine($"Error al cargar los pedidos: {ex.Message}");
            }
            finally
            {
                IsBusy = false; // Finalizar el proceso de carga
            }
        }

        private void FiltrarPedidos()
        {
            var pedidosFiltrados = Pedidos.AsEnumerable();
            if (!string.IsNullOrEmpty(EstadoSeleccionado) && EstadoSeleccionado != "Todos")
            {
                pedidosFiltrados = pedidosFiltrados.Where(p => p.EstadoPedido == EstadoSeleccionado);
            }

            if (!string.IsNullOrEmpty(Numero))
            {
                pedidosFiltrados = pedidosFiltrados.Where(p => p.IdPedido.ToString()
                    .Contains(Numero, StringComparison.OrdinalIgnoreCase));
            }

            // Actualizar la lista filtrada
            PedidosFiltrados = pedidosFiltrados.ToList();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

