using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.EjecucionUsql
{

    public class Mensaje
    {
        public String tipo;

        public Mensaje(String tipo, String desc, int linea, int col)
        {
            this.tipo = tipo;
        }
    }

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
        public String getMensaje()
        {
            return tipo + "\t" + descripcion + "\tLinea:" + linea + "\tColumna:" + columna;
        }
    } 
    
}
