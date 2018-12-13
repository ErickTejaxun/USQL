using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    class Procedimiento
    {
        public String nombre;
        public String tipoRetorno;

        public Procedimiento(String nombre)
        {
            this.nombre = nombre;
        }
    }
}
