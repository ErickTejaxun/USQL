using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    public class Objeto
    {
        public String nombre;
        public List<Atributo> atributos;

        public Objeto(String tipo)
        {
            this.nombre = tipo.ToLower();
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

        public Atributo getAtributo(String id)
        {
            foreach (Atributo attr in atributos)
            {
                if (attr.id.Equals(id))
                {
                    return attr;
                }
            }
            return null;
        }
    }
}
