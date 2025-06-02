using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class RespuestaProcedimiento
    {
        public int FilasAfectadas { get; set; }
        public required string Mensaje { get; set; }
        public int? id_tareaAdicional { get; set; }
        public bool EsExitoso => FilasAfectadas > 0;
    }
}