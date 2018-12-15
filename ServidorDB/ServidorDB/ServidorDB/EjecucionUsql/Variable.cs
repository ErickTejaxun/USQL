using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorBDD.EjecucionUsql
{
    class Variable
    {
        public String nombre;
        public Object valor;

        public Variable(String nom)
        {
            nombre = nom;
        }
        public Variable(String nom, Object val)
        {
            nombre = nom;
            valor = val;
        }
    }
}
