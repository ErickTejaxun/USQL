using Irony.Parsing;
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
        public String id;
        public Object valor;

        public Simbolo(String tipo, String nombre, Object valor)
        {
            this.tipo = tipo.ToLower();
            this.nombre = nombre.ToLower().Replace("@", "");
            this.valor = valor;
        }
        public Simbolo(String tipo, String nombre)
        {
            this.tipo = tipo.ToLower();
            this.nombre = nombre.ToLower().Replace("@", "");
        }

        public Simbolo(String tipo, ParseTreeNode nodo)
        {
            this.tipo = tipo.ToLower();
            this.nombre = nodo.ChildNodes[0].Token.Text.ToLower().Replace("@", "");
            this.id = getId(nombre, nodo.ChildNodes[1].ChildNodes);
            this.valor = nodo;

        }

        private String getId(String nombre, ParseTreeNodeList parametros)
        {
            String idMetodo = nombre.ToLower();
            foreach (ParseTreeNode parametro in parametros)
            {
                idMetodo = idMetodo + "$" + parametro.ChildNodes[0].ChildNodes[0].Token.Text.ToLower();
            }
            return idMetodo;
        }
    }
}
