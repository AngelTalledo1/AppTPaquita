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
        public string Licencia { get; set; } = string.Empty;
        public bool Estado { get; set; }

        public Persona Persona { get; set; } = new Persona();// Relación con Persona
        public Categoria Categoria { get; set; } = new Categoria(); // Relación con Categoria





    }

}
