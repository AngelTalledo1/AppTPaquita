using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class ViajeInfo
    {
            public int IdViaje { get; set; }
            public string Placa { get; set; }
            public int CantidadPlanificada { get; set; }
            public int CantidadRecogida { get; set; }
            public string Destino { get; set; }
            public EstadoViaje EstadoActual { get; set; }
      
    }
}
