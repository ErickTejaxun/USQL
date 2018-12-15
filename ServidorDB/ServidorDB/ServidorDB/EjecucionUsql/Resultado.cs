using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorBDD.EjecucionUsql
{
    class Resultado
    {

        public String tipo;
        public Object valor;

        public Resultado(String tipo,Object valor)
        {
            this.tipo = tipo;
            this.valor = valor;

        }

    }
}
