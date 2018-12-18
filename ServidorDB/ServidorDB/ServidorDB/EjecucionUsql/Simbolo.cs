using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.EjecucionUsql
{
    public class Simbolo
    {
        public String tipo;
        public String nombre;
        public Object valor;

        public Simbolo(String tipo, String nombre, Object valor)
        {
            this.tipo = tipo;
            this.nombre = nombre.ToLower().Replace("@", "");
            this.valor = valor;
        }
        public Simbolo(String tipo, String nombre)
        {
            this.tipo = tipo;
            this.nombre = nombre.ToLower().Replace("@", "");
        }
    }
}
