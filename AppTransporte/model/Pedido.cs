using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Pedido : INotifyPropertyChanged
    {
        public int IdPedido { get; set; }
        public int IdSolicitud { get; set; }
        public int IdUsuario { get; set; }
        public int Cantidad { get; set; }
        public int Viajes { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string? OrigSector { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Servicios { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public string? DestSector { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaEntrega { get; set; }

        // Propiedad EstadoPedido con notificación de cambios
        private string _estadoPedido = string.Empty;
        public string EstadoPedido
        {
            get => _estadoPedido;
            set
            {
                if (_estadoPedido != value)
                {
                    _estadoPedido = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ultEstado)); // Notificar también ultEstado
                }
            }
        }

        // Propiedad ultEstado que usa el mismo valor que EstadoPedido
        // Para mantener consistencia con VEProcesoPedido
        public string ultEstado
        {
            get => _estadoPedido;
            set
            {
                if (_estadoPedido != value)
                {
                    _estadoPedido = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EstadoPedido)); // Notificar también EstadoPedido
                }
            }
        }

        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}