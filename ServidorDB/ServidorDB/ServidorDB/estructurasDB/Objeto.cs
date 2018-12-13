using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    class Objeto
    {
        public String tipo;
        public List<Atributo> atributos;

        public Objeto(String tipo)
        {
            this.tipo = tipo;
            atributos = new List<Atributo>();
        }

        public void addAtributo(Atributo attrib)
        {
            atributos.Add(attrib);
        }

        public Object getValor(String id)
        {
            foreach (Atributo attr in atributos)
            {
                if (attr.id.Equals(id))
                {
                    return attr.valor;
                }
            }
            return null;
        }

        public void actualizarValor(String id, Object valor)
        {
            foreach (Atributo attr in atributos)
            {
                if (attr.id.Equals(id))
                {
                    attr.valor = valor;
                }
            }
        }
    }
}
