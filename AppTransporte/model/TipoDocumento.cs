using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class TipoDocumento
    {
        public int IdTipoDoc { get; set; }
        public string Descripcion { get; set; }


        public TipoDocumento()
        {
            Descripcion = string.Empty; // Valor predeterminado
        }

        // Constructor completo
        public TipoDocumento(int idTipoDoc, string descripcion)
        {
            IdTipoDoc = idTipoDoc;
            Descripcion = descripcion;
        }

    }








}
