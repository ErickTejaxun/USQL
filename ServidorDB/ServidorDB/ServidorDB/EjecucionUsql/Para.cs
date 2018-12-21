using Irony.Parsing;
using ServidorBDD.EjecucionUsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.EjecucionUsql
{
    class Para
    {
        Interprete interprete;
        Logica opL;
        public Para(Interprete interprete)
        {
            this.interprete = interprete;
        }

        public Resultado ejecutar(ParseTreeNode raiz)
        {
            Resultado resultado = null;

            ParseTreeNode nodoDeclaracion = raiz.ChildNodes[0];
            if (!nodoDeclaracion.ChildNodes[1].ChildNodes[0].Token.Text.ToLower().Equals("integer"))
            {
                //reportar error
                return null;
            }

            //cambiar ambito
            TablaSimbolo aux = Interprete.tabla;
            Interprete.tabla = new TablaSimbolo();
            Interprete.tabla.anterior = aux;
            ParseTreeNode SENTSPROC = new ParseTreeNode(new NonTerminal("SENTSPROC"), raiz.Span);
            SENTSPROC.ChildNodes.Add(nodoDeclaracion);
            interprete.ejecutar(SENTSPROC);

            opL = new Logica();
            Resultado condicion = opL.operar(raiz.ChildNodes[1]);
            String operacion = raiz.ChildNodes[2].ChildNodes[0].Token.Text;
            ParseTreeNode nodoSentencias = raiz.ChildNodes[3];

            if (condicion.valor != null && (condicion.tipo.Equals("integer") || (condicion.tipo.Equals("bool"))))
            {
                while (condicion.valor.ToString().Equals("1"))
                {
                    TablaSimbolo aux2 = Interprete.tabla;
                    Interprete.tabla = new TablaSimbolo();
                    Interprete.tabla.anterior = aux2;
                    resultado = interprete.ejecutar(nodoSentencias);
                    Interprete.tabla = aux2;
                    if (condicion.valor.ToString().Equals("1"))
                    {

                        if (resultado != null)
                        {
                            if (resultado.detener)
                            {
                                resultado = null;
                                break;
                            }
                        }

                        Simbolo simbolo = Interprete.tabla.getSimbolo2(nodoDeclaracion.ChildNodes[0].ChildNodes[0].Token.Text.Replace("@", "").ToLower());
                        if (operacion.Equals("++"))
                        {
                            simbolo.valor = Convert.ToInt64(simbolo.valor) + 1;
                        }
                        else
                        {
                            simbolo.valor = Convert.ToInt64(simbolo.valor) - 1;
                        }

                        opL = new Logica();
                        condicion = opL.operar(raiz.ChildNodes[1]);

                    }
                }
            }
            else
            {
                agregarError("Semantico", "La condicion de una sentencia de control solo puede ser tipo Bool o Integer(0|1)", raiz.Span.Location.Line, raiz.Span.Location.Column);
                return null;
            }

            Interprete.tabla = aux;

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
