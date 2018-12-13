using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    class defCampo
    {
        public String nombre;
        public String tipo;
        public Object valor;
        public bool auto;
        public bool nulo;
        public bool primaria;
        public String foranea;

        public defCampo(String nombre, String tipo, Object valor, bool auto, bool nulo, bool primaria, String foranea) // Constructor
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.valor = valor;
            this.auto = auto;
            this.nulo = nulo;
            this.primaria = primaria;
            this.foranea = foranea;
        }
    }         
}
