using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; }
        public string Contraseña { get; set; }
        public int IdTipoUsuario { get; set; }
        public bool Estado { get; set; }
        public int IdPersona { get; set; }

        public Persona Persona { get; set; } // Relación con Persona
        public TipoUsuario TipoUsuario { get; set; } // Relación con TipoUsuario
    }
}
