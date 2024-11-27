using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Trabajador
    {
        public int IdTrabajador { get; set; }
        public int IdPersona { get; set; }
        public int IdCategoria { get; set; }
        public string Licencia { get; set; }
        public bool Estado { get; set; }

        public Persona Persona { get; set; } // Relación con Persona
        public Categoria Categoria { get; set; } // Relación con Categoria





    }

}
