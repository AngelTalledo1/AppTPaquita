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
        public string Username { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public int IdTipoUsuario { get; set; }
        public bool Estado { get; set; }
        public int IdPersona { get; set; }

        public Persona Persona { get; set; } = new Persona();  // Relación con Persona
        public TipoUsuario TipoUsuario { get; set; } = new TipoUsuario(); // Relación con TipoUsuario
    }
}
