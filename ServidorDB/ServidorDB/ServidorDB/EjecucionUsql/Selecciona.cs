using Irony.Parsing;
using ServidorBDD.EjecucionUsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.EjecucionUsql
{
    class Selecciona
    {
        Interprete interprete;
        Logica opL;
        public Selecciona(Interprete interprete)
        {
            this.interprete = interprete;
        }

        public Resultado ejecutar(ParseTreeNode raiz)
        {
            Resultado resultado = null;
            opL = new Logica();
            Resultado valor = opL.operar(raiz.ChildNodes[0]);
            Boolean estado = false;
            foreach (ParseTreeNode nodoCaso in raiz.ChildNodes[1].ChildNodes)
            {
                opL = new Logica();
                Resultado valorCaso = opL.operar(nodoCaso.ChildNodes[0]);
                if (valorCaso.tipo.Equals(valor.tipo))
                {
                    if (valorCaso.valor.ToString().Equals(valor.valor.ToString()))
                    {
                        TablaSimbolo aux = Interprete.tabla;
                        Interprete.tabla = new TablaSimbolo();
                        Interprete.tabla.anterior = aux;
                        resultado = interprete.ejecutar(nodoCaso.ChildNodes[1]);
                        Interprete.tabla = aux;
                        estado = true;
                    }
                }
                else
                {
                    agregarError("Semantico", "El valor a comparar debe ser de igual tipo al valor del caso", nodoCaso.Span.Location.Line, nodoCaso.Span.Location.Column);
                }

                if (resultado != null)
                {
                    if (resultado.detener)
                    {
                        resultado = null;
                        break;
                    }
                }
            }
            if (!estado && raiz.ChildNodes.Count == 3)
            {
                TablaSimbolo aux = Interprete.tabla;
                Interprete.tabla = new TablaSimbolo();
                Interprete.tabla.anterior = aux;
                resultado = interprete.ejecutar(raiz.ChildNodes[2].ChildNodes[0]);
                Interprete.tabla = aux;
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
