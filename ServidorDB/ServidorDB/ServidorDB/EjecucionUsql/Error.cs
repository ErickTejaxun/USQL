using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.EjecucionUsql
{
    public class Error
    {
        public String tipo;
        public String descripcion;
        public int linea;
        public int columna;

        public Error(String tipo, String desc, int linea, int col)
        {
            this.tipo = tipo;
            this.descripcion = desc;
            this.linea = linea;
            this.columna = col;
        }        
    }
}
