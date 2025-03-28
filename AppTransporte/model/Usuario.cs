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
        public int? IdEmpresa { get; set; } 
        public string TipoUsuario { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string NombreEmpresa { get; set; } = string.Empty; 
    }

}
