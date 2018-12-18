using Irony.Parsing;
using ServidorBDD.EjecucionUsql;
using ServidorDB.estructurasDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.EjecucionUsql
{
    public class Asignacion
    {

        Logica opL;
        public Asignacion()
        {
        }

        public Boolean asignar(ParseTreeNode raiz)
        {
            Object variable = getVariable(raiz.ChildNodes[0].ChildNodes[0]);
            opL = new Logica();
            Resultado resultado = opL.operar(raiz.ChildNodes[1]);
            if (variable.GetType().Name.Equals("Simbolo"))
            {
                Simbolo s = (Simbolo)variable;
                s.valor = resultado.valor; ;
            }
            else if (variable.GetType().Name.Equals("Atributo"))
            {
                Atributo a = (Atributo)variable;
                a.valor = resultado.valor;
            }
            int ab = 10;
            return false;

        }

        private Object getVariable(ParseTreeNode nodoVariables)
        {
            int contador = 0;
            object resultado = null;
            Simbolo s = null;
            Object actual = null;
            foreach (ParseTreeNode var in nodoVariables.ChildNodes)
            {
                String nombre = var.Token.Text.ToLower();
                if (contador == 0)
                {
                    s = Interprete.tabla.getSimbolo(nombre);
                    if (s != null)
                    {
                        actual = s.valor;
                        resultado = s;
                    }
                    else
                    {
                        agregarError("Semantico", "Variable " + nombre + " no declarada", var.Span.Location.Line, var.Span.Location.Column);
                        return new Resultado("Error", null);
                    }
                }
                else
                {
                    Objeto objeto = (Objeto)actual;
                    Atributo atributo = objeto.getAtributo(nombre);
                    if (atributo != null)
                    {
                        actual = atributo.valor;
                        resultado = atributo;
                    }
                    else
                    {
                        agregarError("Semantico", "Variable " + nombre + " no declarada", var.Span.Location.Line, var.Span.Location.Column);
                        return new Resultado("Error", null);
                    }
                }
                contador++;

            }

            return resultado;
        }

        private void agregarError(String tipo, String descripcion, int linea, int columna)
        {
            Error error = new Error(tipo, descripcion, linea, columna);
            Form1.Mensajes.Add(error.getMensaje());
            Form1.errores.Add(error);
        }

    }
}
