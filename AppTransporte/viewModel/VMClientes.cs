using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppTransporte.model;

namespace AppTransporte.viewModel
{
    public class VMClientes : ContentPage
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<Clientes> _clientes;

        public List<Clientes>   clientes
        {
            get
            {
                return _clientes;
            }
            set
            {
                _clientes = value;
                OnPropertyChanged(nameof(clientes));

            }
        }

        public VMClientes()
        {
            clientes = new List<Clientes>
            {
                new Clientes { nombre = "S.A ECOSAC", telefono = 959698988, DNI= 73376698 },
                new Clientes { nombre = "Luana Sanchez Pizarro", telefono = 959698988, DNI= 7345655 },
                new Clientes { nombre = "Angel Jair Garcia Perez", telefono = 959698988, DNI= 73372342 },
                new Clientes { nombre = "Alexander Talledo Perez", telefono = 959698988 , DNI= 733766324}
            };
        }
    }
}
