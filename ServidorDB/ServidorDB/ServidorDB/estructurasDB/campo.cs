using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    class campo
    {
        public String id;
        public Object valor;

        public campo(String id, Object valor)
        {
            this.id = id;
            this.valor = valor;
        }
        public void setValor(String id, Object valor)
        {
            this.id = id;
            this.valor = valor;
        }
    }
}
