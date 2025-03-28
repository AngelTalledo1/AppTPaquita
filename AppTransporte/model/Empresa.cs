using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Empresa
    {
        public int id_empresa { get; set; }
        public string razonSocial { get; set; }
        public string RUC { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }
}
