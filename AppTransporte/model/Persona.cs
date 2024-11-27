using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Persona
    {
        public int IdPersona { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? ApePaterno { get; set; }
        public string? ApeMaterno { get; set; }
        public int IdTipoDoc { get; set; }
        public string NumDoc { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? Email { get; set; }

        public TipoDocumento TipoDocumento { get; set; } = new TipoDocumento();// Relación con TipoDocumento
        public string NombreCompleto => $"{Nombre} {ApePaterno} {ApeMaterno}".Trim();
    }
}
