using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class ServicioSeleccionable
    {
        public int IdServicio { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
        public bool IsSelected { get; set; } = false; // Propiedad para la selección
    }
}
