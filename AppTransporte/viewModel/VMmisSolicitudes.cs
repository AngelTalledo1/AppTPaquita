using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AppTransporte.model;
using System.Threading.Tasks;

namespace AppTransporte.viewModel
{
    public class VMmisSolicitudes : INotifyPropertyChanged
    {


        public ObservableCollection<Solicitud> Solicitudes { get; set; }

        public bool IsBusy { get; set; }

        
        
        public VMmisSolicitudes()
        {
            // Crear algunas solicitudes de ejemplo
            Solicitudes = new ObservableCollection<Solicitud>
            {
                new Solicitud { IdSolicitud = 1, Descripcion = "Solicitud de transporte urgente olicitud de transporte urgenteolicitud de transporte urgenteolicitud de transporte urgenteolicitud de transporte urgenteolicitud de transporte urgenteolicitud de transporte urgente", IdEstadoSolicitud = "Pendiente" , Comentario = "Se necesita transporte para mañana", Fecha =DateTime.Today },


                new Solicitud { IdSolicitud = 2, Descripcion = "Solicitud para carga pesada", IdEstadoSolicitud = "Pedido Creado", Comentario = "Carga de maquinaria pesada para la construcción", Fecha =DateTime.Today },
                new Solicitud { IdSolicitud = 3, Descripcion = "Solicitud estándar", IdEstadoSolicitud = "Cancelada", Comentario = "Su pedido no cumple nuestras reglas", Fecha =DateTime.Today }
            };
        }


        public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
        

    }
}
