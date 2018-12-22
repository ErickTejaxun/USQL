using Irony.Parsing;
using ServidorBDD.EjecucionUsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.EjecucionUsql
{
    public class Llamada
    {
        private Interprete interprete;
        public Llamada(Interprete interprete)
        {
            this.interprete = interprete;
        }

        public Resultado ejecutar(ParseTreeNode raiz)
        {
            Resultado resultado = null;
            List<Resultado> valores = getvalParametros(raiz.ChildNodes[1]);
            String id = getId(raiz.ChildNodes[0].Token.Text, valores);
            Simbolo metodo = Interprete.metodos.getSimboloId(id);
            if (metodo != null)
            {
                TablaSimbolo aux = Interprete.tabla;
                Interprete.tabla = new TablaSimbolo();
                Interprete.tabla.anterior = getGlobal(aux);
                ParseTreeNode nodoMetodo = (ParseTreeNode)metodo.valor;

                for (int i = 0; i < nodoMetodo.ChildNodes[1].ChildNodes.Count; i++)
                {
                    Declaracion decla = new Declaracion();
                    decla.declaracionParametros(nodoMetodo.ChildNodes[1].ChildNodes[i], valores[i]);
                }

                if (nodoMetodo.Term.Name.Equals("PROCEDIMIENTO"))
                {
                    resultado = interprete.ejecutar(nodoMetodo.ChildNodes[2]);
                    if (resultado != null)
                    {
                        agregarError("Semantico", "Los procedimientos no devuelven ningun valor", raiz.Span.Location.Line, raiz.Span.Location.Column);
                        resultado = new Resultado("", null);
                    }
                }
                else if (nodoMetodo.Term.Name.Equals("FUNCION"))
                {
                    resultado = interprete.ejecutar(nodoMetodo.ChildNodes[3]);
                    if (resultado == null)
                    {
                        agregarError("Semantico", "La funcion " + metodo.nombre + " debe retornar algun valor ", raiz.Span.Location.Line, raiz.Span.Location.Column);
                        resultado = new Resultado("Error", null);
                    }
                    else
                    if (!resultado.tipo.Equals(metodo.tipo))
                    {
                        agregarError("Semantico", "La funcion " + metodo.nombre + " no es de tipo " + resultado.tipo, raiz.Span.Location.Line, raiz.Span.Location.Column);
                        resultado = new Resultado("Error", null);
                    }
                }
                Interprete.tabla = aux;
            }
            else
            {
                agregarError("Semantico", "El metodo " + id + " no existe", raiz.Span.Location.Line, raiz.Span.Location.Column);
            }

            return resultado;

            //return new Resultado("Error",null);
        }

        private List<Resultado> getvalParametros(ParseTreeNode nodoValores)
        {
            List<Resultado> valores = new List<Resultado>();
            foreach (ParseTreeNode exp in nodoValores.ChildNodes)
            {
                Logica opL = new Logica();
                Resultado resultado = opL.operar(exp);
                if (!resultado.tipo.Equals("Error"))
                {
                    valores.Add(resultado);
                }
            }
            return valores;
        }

        private String getId(String nombre, List<Resultado> valores)
        {
            String id = nombre.ToLower();
            foreach (Resultado r in valores)
            {
                id = id + "$" + r.tipo;
            }
            return id;
        }

        private void agregarError(String tipo, String descripcion, int linea, int columna)
        {
            Error error = new Error(tipo, descripcion, linea, columna);
            Form1.errores.Add(error);
            Form1.Mensajes.Add(error.getMensaje());
        }

        public TablaSimbolo getGlobal(TablaSimbolo actual)
        {
            TablaSimbolo global = actual;
            while (actual.anterior != null)
            {
                global = actual.anterior;
                actual = actual.anterior;
            }

            return global;
        }
    }
}
