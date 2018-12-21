using Irony.Parsing;
using ServidorBDD.EjecucionUsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.EjecucionUsql
{
    class Mientras
    {
        Logica opL;
        Interprete interprete;
        public Mientras(Interprete interprete)
        {
            this.interprete = interprete;
        }

        public Resultado ejecutar(ParseTreeNode raiz)
        {
            Resultado resultado = null;
            opL = new Logica();
            Resultado condicion = opL.operar(raiz.ChildNodes[0]);
            if (!(condicion.tipo.Equals("integer") || (condicion.tipo.Equals("bool"))))
            {
                agregarError("Semantico", "La condicion de una sentencia de control solo puede ser tipo Bool o Integer(0|1)", raiz.Span.Location.Line, raiz.Span.Location.Column);
                return null;
            }
            while (condicion.valor != null && (condicion.tipo.Equals("integer") || (condicion.tipo.Equals("bool"))))
            {
                if (condicion.valor.ToString().Equals("1"))
                {
                    TablaSimbolo aux = Interprete.tabla;
                    Interprete.tabla = new TablaSimbolo();
                    Interprete.tabla.anterior = aux;
                    resultado = interprete.ejecutar(raiz.ChildNodes[1]);
                    Interprete.tabla = aux;
                    if (resultado != null)
                    {
                        if (resultado.detener)
                        {
                            resultado = null;
                            break;
                        }
                    }
                    opL = new Logica();
                    condicion = opL.operar(raiz.ChildNodes[0]);
                }
                else
                {

                    break;
                }
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
