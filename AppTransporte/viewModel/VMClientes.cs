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

        public List<Clientes> clientes
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
                new Clientes { nombre = "Juan Perez", telefono = 959698988 },
                new Clientes { nombre = "luana sanchez", telefono = 959698988 },
                new Clientes { nombre = "angel Perez", telefono = 959698988 },
                new Clientes { nombre = "alexander Perez", telefono = 959698988 }
            };
        }
    }
}
