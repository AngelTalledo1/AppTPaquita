using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public int IdPersona { get; set; }
        public bool Estado { get; set; }

        public Persona Persona { get; set; } // Relación con Persona
    }
}
