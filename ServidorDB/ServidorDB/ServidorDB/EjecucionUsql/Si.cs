using Irony.Parsing;
using ServidorBDD.EjecucionUsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.EjecucionUsql
{
    public class Si
    {
        Logica opL;
        Interprete interprete;
        public Si(Interprete interprete)
        {
            this.interprete = interprete;
        }

        public Resultado ejecutar(ParseTreeNode raiz)
        {
            Resultado resultado = null;
            opL = new Logica();
            Resultado condicion = opL.operar(raiz.ChildNodes[0]);
            if (condicion.valor != null)
            {
                if ((condicion.tipo.Equals("bool") || condicion.tipo.Equals("integer")) && (condicion.valor.ToString().Equals("0") || condicion.valor.ToString().Equals("1")))
                {
                    if (condicion.valor.ToString().Equals("1"))
                    {
                        TablaSimbolo aux = Interprete.tabla;
                        Interprete.tabla = new TablaSimbolo();
                        Interprete.tabla.anterior = aux;
                        resultado = interprete.ejecutar(raiz.ChildNodes[1]);
                        Interprete.tabla = aux;
                    }
                    else if (condicion.valor.ToString().Equals("0") && raiz.ChildNodes.Count == 3)
                    {
                        TablaSimbolo aux = Interprete.tabla;
                        Interprete.tabla = new TablaSimbolo();
                        Interprete.tabla.anterior = aux;
                        resultado = interprete.ejecutar(raiz.ChildNodes[2]);
                        Interprete.tabla = aux;

                    }
                }
                else
                {
                    agregarError("Semantico", "La condicion de una sentencia de control solo puede ser tipo Bool o Integer(0|1)", raiz.Span.Location.Line, raiz.Span.Location.Column);
                    return null;
                }
            }
            else
            {
                //error
                return null;
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
