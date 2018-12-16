using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    public class Atributo
    {
        public String tipo;
        public String id;
        public Object valor;

        public Atributo(String tipo, String id, Object valor)
        {
            this.tipo = tipo;
            this.id = id;
            this.valor = valor;
        }
        public Object getValor()
        {
            return valor;
        }
    }
}
