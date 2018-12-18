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
    public class Declaracion
    {
        Logica opL;
        public Boolean declaracion(ParseTreeNode raiz)
        {
            if (raiz.ChildNodes.Count == 3)
            {
                ParseTreeNodeList ids = raiz.ChildNodes[0].ChildNodes;
                String tipoDato = raiz.ChildNodes[1].ChildNodes[0].Token.Text.ToLower();
                opL = new Logica();

                foreach (ParseTreeNode id in ids)
                {

                    opL = new Logica();
                    Resultado resultado = opL.operar(raiz.ChildNodes[2].ChildNodes[0]);
                    resultado = comprobarTipos(tipoDato, resultado.tipo, resultado, raiz.Span.Location.Line, raiz.Span.Location.Column);
                    if (!resultado.tipo.Equals("Error"))
                    {
                        Simbolo variable = new Simbolo(tipoDato, id.Token.Text, resultado.valor);
                        Boolean estado = Interprete.tabla.setSimbolo(variable);
                        if (!estado)
                        {
                            agregarError("Semantico", "La variable " + id.Token.Text + " ya existe", id.Span.Location.Line, id.Span.Location.Column);
                        }

                    }

                }

            }
            else if (raiz.ChildNodes.Count == 2)
            {
                ParseTreeNodeList ids = raiz.ChildNodes[0].ChildNodes;
                String tipoDato = raiz.ChildNodes[1].ChildNodes[0].Token.Text.ToLower();
                foreach (ParseTreeNode id in ids)
                {
                    Simbolo variable = null;
                    DateTime today;
                    switch (tipoDato)
                    {
                        case "text":
                            variable = new Simbolo(tipoDato, id.Token.Text, "");
                            break;
                        case "integer":
                            variable = new Simbolo(tipoDato, id.Token.Text, 0);
                            break;
                        case "double":
                            variable = new Simbolo(tipoDato, id.Token.Text, 0.0);
                            break;
                        case "bool":
                            variable = new Simbolo(tipoDato, id.Token.Text, 0);
                            break;
                        case "date":
                            today = DateTime.Today;
                            variable = new Simbolo(tipoDato, id.Token.Text, today.ToString("dd-MM-yyyy"));
                            break;
                        case "datetime":
                            today = DateTime.Today;
                            variable = new Simbolo(tipoDato, id.Token.Text, today.ToString("dd-MM-yyyy hh:mm:ss"));
                            break;
                        default:
                            Objeto objeto = instanciarObjeto(tipoDato, id.Span.Location.Line, id.Span.Location.Column);
                            if (objeto != null)
                            {
                                String nombreObjeto = raiz.ChildNodes[0].ChildNodes[0].Token.Text.ToLower();
                                variable = new Simbolo(tipoDato, nombreObjeto, objeto);
                            }
                            else
                            {
                                return false;
                            }
                            break;
                    }
                    Boolean estado = Interprete.tabla.setSimbolo(variable);
                    if (!estado)
                    {
                        agregarError("Semantico", "La variable " + id.Token.Text + " ya existe", id.Span.Location.Line, id.Span.Location.Column);
                    }
                }

            }
            return false;
        }

        private Resultado comprobarTipos(String tipo1, String tipo2, Resultado resultado, int linea, int columna)
        {
            switch (tipo1)
            {
                case "text":
                    switch (tipo2)
                    {
                        case "text":
                            return resultado;
                        case "integer":
                        case "double":
                        case "bool":
                        case "date":
                        case "datetime":
                        default:
                            agregarError("Semantico", "Tipos de datos incompatibles", linea, columna);
                            return new Resultado("Error", null);
                    }
                case "integer":
                    switch (tipo2)
                    {
                        case "integer":
                            return resultado;
                        case "double":
                            return new Resultado("integer", Convert.ToInt64(Convert.ToDouble(resultado.valor))); ;
                        case "bool":
                            return new Resultado("integer", resultado.valor);
                        case "text":
                        case "date":
                        case "datetime":
                        default:
                            agregarError("Semantico", "Tipos de datos incompatibles", linea, columna);
                            return new Resultado("Error", null);
                    }
                case "double":
                    switch (tipo2)
                    {
                        case "integer":
                            return new Resultado("double", Convert.ToDouble(Convert.ToInt64(resultado.valor)));
                        case "double":
                            return resultado;
                        case "bool":
                            return new Resultado("double", Convert.ToDouble(Convert.ToInt64(resultado.valor)));
                        case "text":
                        case "date":
                        case "datetime":
                        default:
                            agregarError("Semantico", "Tipos de datos incompatibles", linea, columna);
                            return new Resultado("Error", null);
                    }
                case "bool":
                    switch (tipo2)
                    {
                        case "integer":
                            return new Resultado("bool", resultado.valor);
                        case "bool":
                            return resultado;
                        case "text":
                        case "double":
                        case "date":
                        case "datetime":
                        default:
                            agregarError("Semantico", "Tipos de datos incompatibles", linea, columna);
                            return new Resultado("Error", null);
                    }
                case "date":
                    switch (tipo2)
                    {
                        case "date":
                            return resultado;
                        case "datetime":
                            return new Resultado("date", Convert.ToDateTime(resultado.valor).ToString("dd-mm-yyyy"));
                        case "text":
                        case "integer":
                        case "double":
                        case "bool":
                        default:
                            agregarError("Semantico", "Tipos de datos incompatibles", linea, columna);
                            return new Resultado("Error", null);
                    }
                case "datetime":
                    switch (tipo2)
                    {
                        case "date":
                            return new Resultado("datetime", resultado.valor + " 00:00:00"); ;
                        case "datetime":
                            return resultado;
                        case "text":
                        case "integer":
                        case "double":
                        case "bool":
                        default:
                            agregarError("Semantico", "Tipos de datos incompatibles", linea, columna);
                            return new Resultado("Error", null);
                    }
                default:
                    if (tipo1.Equals(tipo2))
                    {
                        return resultado;
                    }
                    else
                    {
                        agregarError("Semantico", "Tipos de datos incompatibles", linea, columna);
                        return new Resultado("Error", null);
                    }

            }
        }


        private void agregarError(String tipo, String descripcion, int linea, int columna)
        {
            Error error = new Error(tipo, descripcion, linea, columna);
            Form1.errores.Add(error);
        }

        private Objeto instanciarObjeto(string nombre, int linea, int columna)
        {
            Objeto aux = Form1.sistemaArchivos.getBase().getObjeto(nombre, linea, columna);
            if (aux == null)
            {
                return null;
            }
            Objeto objeto = new Objeto(aux.nombre);
            foreach (Atributo atributo in aux.atributos)
            {
                switch (atributo.tipo)
                {
                    case "text":
                    case "integer":
                    case "double":
                    case "bool":
                    case "date":
                    case "datetime":
                        objeto.addAtributo(new Atributo(atributo.tipo, atributo.id, atributo.valor));
                        break;
                    default:
                        Objeto objAtributo = instanciarObjeto(atributo.tipo, linea, columna);
                        objeto.addAtributo(new Atributo(objAtributo.nombre, atributo.id, objAtributo));
                        break;
                }
            }

            return objeto;
        }

    }
}
