using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

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
            this.nombre = nombre;
            this.tipoRetorno = tipoRetorno;
            listaParametros = new List<Parametro>();
        }

        public Procedimiento(ParseTreeNode raiz, ParseTreeNode raizCompleta)
        {
            this.raizMetodo = raiz;
            this.nombre = raiz.ChildNodes[0].Token.Text;
            this.id = getId(this.nombre, raiz.ChildNodes[1]);
            if (raiz.ChildNodes.Count==3)// Procedimiento
            {
                this.tipoRetorno = "";                
            }
            else // Es funcion
            {                
                this.tipoRetorno = raiz.ChildNodes[2].Token.Text;
            }
        }

        public String getId(String nombreFuncion, ParseTreeNode raizParametros )
        {
            foreach (ParseTreeNode nodoParametro in raizParametros.ChildNodes)
            {
                nombreFuncion = nombreFuncion + "$" + nodoParametro.ChildNodes[0].ChildNodes[0].Token.Text;
            }
            return nombreFuncion;
        }

        public void getCodigo(ParseTreeNode raiz)
        {
            foreach (ParseTreeNode nodo in raiz.ChildNodes)
            {
                getCodigo(nodo);
                if (nodo.Token!=null)
                {
                    this.codigoFuente = this.codigoFuente + nodo.Token.Text;
                    if (nodo.Token.Text.Equals(";") || nodo.Token.Text.Equals("}") || nodo.Token.Text.Equals("{"))
                    {
                        this.codigoFuente = this.codigoFuente +"\n";
                    }
                }
            }            
        }
    }
}
