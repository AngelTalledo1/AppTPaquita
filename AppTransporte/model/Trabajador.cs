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
        public string Nombre { get; set; } = string.Empty;
        public string? apePaterno { get; set; }
        public string? apeMaterno { get; set; } = string.Empty;
        public int idtipoDoc { get; set; }
        public int idcategoria { get; set; }
        public string numDoc { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string direccion { get; set; } = string.Empty;
        public string? email { get; set; } 
        public string categoria { get; set; } = string.Empty;   
        public string? licencia { get; set; } = string.Empty;
        public string usuario { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;

        public string NombreTrabajador => $"{Nombre} {apePaterno}".Trim();
        public string apellidoTrabajador => $"{apePaterno} {apeMaterno}".Trim();

        


    }

}
