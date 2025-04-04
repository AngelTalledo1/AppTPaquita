﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTransporte.model
{
    public class TrabajadorViaje
    {
        public int IdTrabajadorViaje { get; set; }
        public int IdViaje { get; set; }
        public int IdTrabajador { get; set; }

        public Viaje Viaje { get; set; } = new Viaje(); // Relación con Viaje
        public Trabajador Trabajador { get; set; } = new Trabajador(); // Relación con Trabajador
    }
}
