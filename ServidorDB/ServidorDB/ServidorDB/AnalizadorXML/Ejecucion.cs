using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using ServidorDB.estructurasDB;

namespace ServidorDB.AnalizadorXML
{
    class Ejecucion
    {
        public ParseTreeNode raiz;

        public Ejecucion(ParseTreeNode raiz)
        {
            this.raiz = raiz;
        }

        public List<BD> recorrerArbolMaestro()
        {
            List<BD> listaBases = new List<BD>();
            foreach (ParseTreeNode nodo in raiz.ChildNodes[0].ChildNodes)
            {
                if (nodo.ChildNodes.Count == 6)
                {
                    BD baseNueva = new BD(nodo.ChildNodes[1].ToString(), nodo.ChildNodes[4].ToString());
                    listaBases.Add(baseNueva);
                }
            }
            return listaBases;
        }

        public List<Usuario> recorrerUsuarios()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            foreach (ParseTreeNode nodo in raiz.ChildNodes[0].ChildNodes)
            {
                if (nodo.ChildNodes.Count == 6)
                {

                }
            }
            return listaUsuarios;
        }

        
    }
}
