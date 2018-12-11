using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace ServidorDB.AST
{
    public class diagramaDOT
    {
        /*
         diagraph G{
         nodo0[label="etiqueta"];
         nodo1[label="hijo1"];
         nodo2[label="hijo2"];
         nodo0->nodo1;
         nodo0->nodo2;
         }         
         */

        private static int contador;
        private static String grafo;

        public static String getDOT(ParseTreeNode raiz)
        {
            grafo = "digraph G{";
            grafo += "node[shape=\"box\"];";
            grafo += "nodo0[label=\"" + escapar(raiz.ToString()) + "\"];\n";
            contador = 1;
            recorrerAST("nodo0", raiz);
            grafo += "}";
            return grafo;
        }

        private static void recorrerAST(String padre, ParseTreeNode hijos)
        {
            foreach (ParseTreeNode hijo in hijos.ChildNodes)
            {
                //if (hijo.Term.ToString().Equals("{<}")
                //    || hijo.Term.ToString().Equals("{>}")
                //    )
                //{
                //    return;
                //}

                String nombreHijo = "nodo" + contador.ToString();
                grafo += nombreHijo + "[label=\"" + escapar("[" +hijo.Term.ToString() +"]"+hijo.Token.Text)  + "\"];\n";
                grafo += padre + "->" + nombreHijo + ";\n";
                contador++;
                recorrerAST(nombreHijo, hijo);
            }
        }

        private static String escapar(String cadena)
        {
            cadena = cadena.Replace("\\", "\\\\");
            cadena = cadena.Replace("\"", "\\\"");
            return cadena;
        }

    }
}
