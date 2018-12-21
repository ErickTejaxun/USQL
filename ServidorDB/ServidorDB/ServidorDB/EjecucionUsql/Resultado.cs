using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorBDD.EjecucionUsql
{
    public class Resultado
    {

        public String tipo;
        public Object valor;
        public Boolean detener;
        public Resultado(String tipo, Object valor)
        {
            this.tipo = tipo;
            this.valor = valor;
            this.detener = false;

        }

    }
}
