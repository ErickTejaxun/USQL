using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using ServidorBDD.AnalisisUsql;

namespace ServidorDB.estructurasDB
{
    public class Parametro
    {
        public String nombre;
        public String tipo;

        public Parametro(String nombre, String tipo)
        {
            this.nombre = nombre;
            this.tipo = tipo;
        }
    }

    public class Procedimiento
    {
        public String nombre;
        public String tipoRetorno;
        public String codigoFuente;
        public List<Parametro> listaParametros;
        public ParseTreeNode raizMetodo;
        public String id;

        public Procedimiento(String nombre, String tipoRetorno)
        {
            this.nombre = nombre.ToLower();
            this.tipoRetorno = tipoRetorno;
            listaParametros = new List<Parametro>();
        }

        public Procedimiento(ParseTreeNode raizCompleta)
        {
            listaParametros = new List<Parametro>();
            GramaticaSDB gramatica = new GramaticaSDB();
            LanguageData lenguaje1 = new LanguageData(gramatica);
            Parser par = new Parser(lenguaje1);
            agregarComa(raizCompleta);
            getCodigo(raizCompleta);
            codigoFuente = codigoFuente.Replace("@ ","@");
            ParseTree arbol = par.Parse(this.codigoFuente);
            ParseTreeNode raiz = arbol.Root.ChildNodes[0].ChildNodes[0];
            this.raizMetodo = raiz;
            this.nombre = raiz.ChildNodes[0].Token.Text.ToLower();
            this.id = getId(this.nombre, raiz.ChildNodes[1]);
            if (raiz.ChildNodes.Count==3)// Procedimiento
            {
                this.tipoRetorno = "";                
            }
            else // Es funcion
            {                
                this.tipoRetorno = raiz.ChildNodes[2].ChildNodes[0].Token.Text.ToLower();
            }

            
        }

        private void agregarComa(ParseTreeNode raiz)

        {
            ParseTreeNodeList lista = new ParseTreeNodeList();
            for (int i = 0; i < raiz.ChildNodes[0].ChildNodes[4].ChildNodes.Count; i++)
            {
                lista.Add(raiz.ChildNodes[0].ChildNodes[4].ChildNodes[i]);
                if ((i + 1) < raiz.ChildNodes[0].ChildNodes[4].ChildNodes.Count)
                {
                    lista.Add(new ParseTreeNode(new Token(new Terminal("coma"), raiz.Span.Location, ",", null)));
                }
                
            }

            for (int i = 0; i < raiz.ChildNodes[0].ChildNodes[4].ChildNodes.Count;i++)
            {
                raiz.ChildNodes[0].ChildNodes[4].ChildNodes.RemoveAt(i);
                    i--;
            }

            for(int i = 0; i < lista.Count; i++)
            {
                raiz.ChildNodes[0].ChildNodes[4].ChildNodes.Add(lista[i]);
            }
        }

            

        public String getId(String nombreFuncion, ParseTreeNode raizParametros )
        {
            foreach (ParseTreeNode nodoParametro in raizParametros.ChildNodes)
            {
                nombreFuncion = nombreFuncion + "$" + nodoParametro.ChildNodes[0].ChildNodes[0].Token.Text.ToLower();
            }
            return nombreFuncion;
        }

        public void getCodigo(ParseTreeNode raiz)
        {
            foreach (ParseTreeNode nodo in raiz.ChildNodes)
            {
                getCodigo(nodo);
                if (nodo.Token != null)
                {
                    this.codigoFuente = this.codigoFuente + nodo.Token.Text + " ";
                    this.codigoFuente = this.codigoFuente.Replace(", ,",",");
                    //if (nodo.Token.Text.Equals(";") || nodo.Token.Text.Equals("}") || nodo.Token.Text.Equals("{"))
                    //{
                    //    this.codigoFuente = this.codigoFuente +"\n";
                    //}
                }
            }            
        }

        public ParseTreeNode getRaiz()
        {
            GramaticaSDB gramatica = new GramaticaSDB();
            LanguageData lenguaje1 = new LanguageData(gramatica);
            Parser par = new Parser(lenguaje1);
            ParseTree arbol = par.Parse(this.codigoFuente.Replace("~",""));
            return arbol.Root.ChildNodes[0].ChildNodes[0];
        }
    }
}
