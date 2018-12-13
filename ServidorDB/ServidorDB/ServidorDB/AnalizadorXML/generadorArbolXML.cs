using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace ServidorDB.AnalizadorXML
{
    class generadorArbolXML
    {
        public generadorArbolXML()
        {
        }

        public void recorrerArbol(ParseTreeNode raiz)
        {
            switch (raiz.Term.ToString())
            {
                case "LISTADB":
                    foreach (ParseTreeNode hijo in raiz.ChildNodes)
                    {

                    }
                    break;
            }
        }
    }
}
