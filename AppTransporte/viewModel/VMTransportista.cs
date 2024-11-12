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
    public class VMTransportista : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<Trabajador> _transportistas;

        public List<Trabajador> transportista
        {
            get
            {
                return _transportistas;
            }
            set
            {
                _transportistas = value;
                OnPropertyChanged(nameof(transportista));

            }
        }

        private List<Trabajador> _transportistaFiltrados;
        public List<Trabajador> TransportistaFiltrados
        {
            get
            {
                return _transportistaFiltrados;
            }
            set
            {
                _transportistaFiltrados = value;
                OnPropertyChanged(nameof(TransportistaFiltrados));
            }
        }
        public List<string> Categoria { get; set; }

        private string _categoriaSeleccionada;
        public string CategoriaSeleccionada
        {
            get
            {
                return _categoriaSeleccionada;
            }
            set
            {
                if (_categoriaSeleccionada != value)
                {
                    _categoriaSeleccionada = value;
                    FiltrarTransportista();
                    OnPropertyChanged(nameof(CategoriaSeleccionada));
                }
            }
        }

        public VMTransportista()
        {
            transportista = new List<Trabajador>
            {
                new Trabajador { nombre = "S.A ECOSAC", telefono = 959698988, DNI= 73376698, categoria="Transp."},
                new Trabajador { nombre = "Luan pizarro", telefono = 959698988, DNI= 7345655, categoria="Transp." },
                new Trabajador { nombre = "AngelGarcia Perez", telefono = 959698988, DNI= 73372342, categoria="Admin." },
                new Trabajador { nombre = "Alexander Talledo ", telefono = 959698988 , DNI= 733766324, categoria="Transp."}
            };

            Categoria = new List<string>
            {
                "Admin.",
                "Transp.",
            };

            TransportistaFiltrados =  transportista;


        }

        private void FiltrarTransportista()
        {
            var transportistaFiltrados = _transportistas.AsEnumerable();
            if (!string.IsNullOrEmpty(CategoriaSeleccionada) && CategoriaSeleccionada != "Todos")
            {
                transportistaFiltrados = transportistaFiltrados.Where(t => t.categoria == CategoriaSeleccionada);
            }

            // Actualizar la lista filtrada
            TransportistaFiltrados = transportistaFiltrados.ToList();
        }
    }
}
