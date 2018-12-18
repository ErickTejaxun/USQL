using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    public class defCampo
    {
        public String nombre;
        public String tipo;        
        public bool auto;
        public bool nulo;
        public bool primaria;
        public String foranea;
        public bool unico;

        public defCampo(String nombre, String tipo, bool auto, bool nulo, bool primaria, String foranea) // Constructor
        {
            this.nombre = nombre;
            this.tipo = tipo;           
            this.auto = auto;
            this.nulo = nulo;
            this.primaria = primaria;
            this.foranea = foranea;
        }
        public defCampo(String nombre, String tipo, bool auto, bool nulo, bool primaria, String foranea, bool unico) // Constructor
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.auto = auto;
            this.nulo = nulo;
            this.primaria = primaria;
            this.foranea = foranea;
            this.unico = unico;
        }
    }         
}
