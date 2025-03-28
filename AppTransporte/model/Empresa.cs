using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Empresa
    {
        public required string NombreEmpresa { get; set; }
        public int IdEmpresa { get; set; }
        public required string Ruc { get; set; }
        public string Razon { get; set; } = string.Empty;
        public string Direccion { get; set; }
        public bool Estado { get; set; }
    }
}
