﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}
