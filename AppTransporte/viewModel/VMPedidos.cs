using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppTransporte.model;

namespace AppTransporte.viewModel
{
    public class VMPedidos : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand VerDetallesCommand { get; set; }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        
        private List<Pedidos> _pedido;

        public List<Pedidos> Pedido
        {
            get
            {
                return _pedido;
            }
            set
            {
                _pedido = value;
                OnPropertyChanged(nameof(Pedido));

            }
        }

        public List<string> Estados { get; set; }

        private List<Pedidos> _pedidosFiltrados;
        public List<Pedidos> PedidosFiltrados
        {
            get
            {
                return _pedidosFiltrados;
            }
            set
            {
                _pedidosFiltrados = value;
                OnPropertyChanged(nameof(PedidosFiltrados));
            }
        }

        private string _estadoSeleccionado;
        public string EstadoSeleccionado
        {
            get
            {
                return _estadoSeleccionado;
            }
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

        private string _Numero;
        public string Numero
        {
            get
            {
                return _Numero;
            }
            set
            {
                if (_Numero != value)
                {
                    _Numero = value;
                    FiltrarPedidos();
                    OnPropertyChanged(nameof(Numero));
                }
            }
        }

        public VMPedidos()
        {
            Pedido = new List<Pedidos>
            {
                new Pedidos { Numero = "#0001", imagen = "barril.png", cantidad = 25.5, estado = "Pendiente"},
                new Pedidos { Numero = "#0002", imagen = "barril.png", cantidad = 10.5, estado = "En curso" },
                new Pedidos { Numero = "#0003", imagen = "barril.png", cantidad = 20.5, estado = "Entregado" },
                new Pedidos { Numero = "#0004", imagen = "barril.png", cantidad = 85.5, estado = "Pendiente" },
                new Pedidos { Numero = "#0005", imagen = "barril.png", cantidad = 5.5, estado = "En curso" },
                new Pedidos { Numero = "#0006", imagen = "barril.png", cantidad = 24.5, estado = "En curso" },
                new Pedidos { Numero = "#0007", imagen = "barril.png", cantidad = 98.5, estado = "Pendiente" },
                new Pedidos { Numero = "#0008", imagen = "barril.png", cantidad = 100.5, estado = "Entregado" },
                new Pedidos { Numero = "#0009", imagen = "barril.png", cantidad = 201.5, estado = "En curso" },
            };


            Estados = new List<String>
            {
                "Todos",
                "Pendiente",
                "Asignado",
                "En curso",
                "Entregado"
            };

            PedidosFiltrados = Pedido;
            
        }

        


        private void FiltrarPedidos()
        {
            var pedidosFiltrados = _pedido.AsEnumerable();
            if (!string.IsNullOrEmpty(EstadoSeleccionado) && EstadoSeleccionado != "Todos")
            {
                pedidosFiltrados = pedidosFiltrados.Where(p => p.estado == EstadoSeleccionado);
            }

            // Filtrar por número de pedido
            if (!string.IsNullOrEmpty(Numero))
            {
                pedidosFiltrados = pedidosFiltrados.Where(p => p.Numero.Contains(Numero, System.StringComparison.OrdinalIgnoreCase));
            }

            // Actualizar la lista filtrada
            PedidosFiltrados = pedidosFiltrados.ToList();
        }

    }
}
