using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Descripcion { get; set; }


        public Categoria()
        {
            Descripcion = string.Empty;
        }

        // Constructor con parámetros
        public Categoria(string descripcion)
        {
            Descripcion = descripcion;
        }

    }
}
