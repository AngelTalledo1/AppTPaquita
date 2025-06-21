using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Trabajador
    {
        internal string Categoria;
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

        // AGREGAR ESTA PROPIEDAD PARA EL ESTADO
        public bool estado { get; set; } = true;

        public string NombreCompleto => $"{Nombre} {apePaterno} {apeMaterno}".Trim();
        public string NombreTrabajador => $"{Nombre} {apePaterno}".Trim();
        public string apellidoTrabajador => $"{apePaterno} {apeMaterno}".Trim();

        // PROPIEDADES PARA ASIGNAR VIAJE CON ESTADO
        public string EstadoDescripcion { get; set; } = "Disponible";
        public string NombreTrabajadorConEstado
        {
            get
            {
                string nombre = $"{Nombre} {apePaterno} {apeMaterno}".Trim();
                return $"{nombre} ({EstadoDescripcion})";
            }
        }
    }
}