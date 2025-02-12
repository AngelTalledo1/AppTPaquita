using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class UsuarioResponse
    {
        public UsuarioResponse(int idUsuario, int idTipoUsuario)
        {
            this.idUsuario = idUsuario;
            this.idTipoUsuario = idTipoUsuario;
        }

        public int idUsuario { get; set; }
        public int idTipoUsuario { get; set; }
    }
}
