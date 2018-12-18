using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    public class campo
    {
        public String id;
        public String tablaId;
        public Object valor;
        public String tipo;

        public campo(String id, Object valor)
        {            
            this.id = id;
            this.valor = valor;             
        }
        public campo(String id, Object valor, String tipo)
        {
            this.id = id;
            this.valor = valor;
            this.tipo = tipo;
        }
        public void setValor(String id, Object valor)
        {
            this.id = id;
            this.valor = valor;
        }
    }
}
