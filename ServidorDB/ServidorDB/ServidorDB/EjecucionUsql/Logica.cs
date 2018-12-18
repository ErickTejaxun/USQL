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
    public class Logica
    {
        public Relacional opR;
        public tupla tuplaActual;
        public Logica()
        {

        }
        public Logica(tupla tuplaActual)
        {
            this.tuplaActual = tuplaActual;
        }



        public Resultado operar(ParseTreeNode raiz)
        {

            Resultado r1 = null;
            Resultado r2 = null;

            switch (raiz.Term.Name)
            {
                case "EXPL":
                    if (raiz.ChildNodes.Count == 3)
                    {
                        r1 = operar(raiz.ChildNodes[0]);
                        r2 = operar(raiz.ChildNodes[2]);
                    }
                    else if (raiz.ChildNodes.Count == 2)
                    {
                        //not
                        r1 = operar(raiz.ChildNodes[1]);
                        if (!r1.tipo.ToString().Equals("Error"))
                        {
                            if (r1.valor.ToString().Equals("1"))
                            {
                                return new Resultado("bool", 0);
                            }
                            else if (r1.valor.ToString().Equals("0"))
                            {
                                return new Resultado("bool", 1);
                            }
                        }
                        else
                        {
                            return new Resultado("Error", null);
                        }

                    }
                    else
                    {

                        return operar(raiz.ChildNodes[0]);
                    }
                    break;
                case "EXPR":
                    opR = new Relacional(tuplaActual);
                    return opR.operar(raiz);
            }


            if (!((r1.tipo.Equals("integer") || r1.tipo.Equals("bool")) && r2.tipo.Equals("integer") || r2.tipo.Equals("bool")))
            {
                agregarError("Semantico", "Bool solo puede ser 1/0", raiz.Span.Location.Line, raiz.Span.Location.Column);
                return new Resultado("Error", null);
            }

            if (!((Convert.ToInt32(r1.valor) == 1 || Convert.ToInt32(r1.valor) == 0) && (Convert.ToInt32(r2.valor) == 1 || Convert.ToInt32(r2.valor) == 0)))
            {
                agregarError("Semantico", "Bool solo puede ser 1/0", raiz.Span.Location.Line, raiz.Span.Location.Column);
                return new Resultado("Error", null);
            }


            switch (raiz.ChildNodes[1].Token.Text)
            {
                case "&&":
                    if (Convert.ToInt32(r1.valor) == 1 && Convert.ToInt32(r2.valor) == 1)
                    {
                        return new Resultado("bool", 1);
                    }
                    else
                    {

                        return new Resultado("bool", 0);
                    }
                case "||":
                    if (Convert.ToInt32(r1.valor) == 1 || Convert.ToInt32(r2.valor) == 1)
                    {
                        return new Resultado("bool", 1);
                    }
                    else
                    {

                        return new Resultado("bool", 0);
                    }
            }

            return new Resultado("Error", null);
        }

        private void agregarError(String tipo, String descripcion, int linea, int columna)
        {
            Error error = new Error(tipo, descripcion, linea, columna);
            Form1.errores.Add(error);
        }
    }
}
