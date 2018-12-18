using Irony.Parsing;
using ServidorDB;
using ServidorDB.EjecucionUsql;
using ServidorDB.estructurasDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorBDD.EjecucionUsql
{
    public class Aritmetica
    {
        public tupla tuplaActual;
        public Aritmetica()
        {
        }
        public Aritmetica(tupla tuplaActual)
        {
            this.tuplaActual = tuplaActual;
        }

        public Resultado operar(ParseTreeNode raiz)
        {
            Resultado r1 = null;
            Resultado r2 = null;
            switch (raiz.Term.Name.ToLower())
            {
                case "expl":
                case "expr":
                    return operar(raiz.ChildNodes[0]);
                case "expa":
                    if (raiz.ChildNodes.Count == 3)
                    {
                        r1 = operar(raiz.ChildNodes[0]);
                        r1.tipo = r1.tipo.ToLower();
                        if (r1.tipo.Equals("text"))
                        {
                            r1.valor = r1.valor.ToString().Replace("\"", "");

                        }
                        r2 = operar(raiz.ChildNodes[2]);
                        r2.tipo = r2.tipo.ToLower();
                        if (r2.tipo.Equals("text"))
                        {
                            r2.valor = r2.valor.ToString().Replace("\"", "");

                        }
                    }
                    else if (raiz.ChildNodes.Count == 2)
                    {
                        //Si es operacion unaria
                        r1 = operar(raiz.ChildNodes[1]);
                        r1.tipo = r1.tipo.ToLower();
                        switch (r1.tipo)
                        {
                            case "integer":
                                return new Resultado("integer", Convert.ToInt64(r1.valor) * -1);
                            case "double":
                                return new Resultado("double", Convert.ToDouble(r1.valor) * -1);
                            case "bool":
                                return new Resultado("integer", Convert.ToInt64(r1.valor) * -1);
                            default:
                                return new Resultado("Error", null);

                        }
                    }
                    else
                    {
                        return operar(raiz.ChildNodes[0]);
                    }
                    break;
                case "integer":
                    return new Resultado("integer", raiz.Token.Text);
                case "text":
                    return new Resultado("text", raiz.Token.Text.Replace("\"", ""));
                case "double":
                    return new Resultado("double", raiz.Token.Text);
                case "bool":
                    return new Resultado("bool", raiz.Token.Text);
                case "date":
                    return new Resultado("date", raiz.Token.Text);
                case "datetime":
                    return new Resultado("datetime", raiz.Token.Text);
                case "accesoobj":
                    r1 = getVariable(raiz.ChildNodes[0]);
                    return r1;
                case "idacceso":
                    r1 = getValorCampo(raiz);
                    return r1;
            }

            switch (raiz.ChildNodes[1].Token.Text)
            {
                #region Suma
                case "+":
                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                    return new Resultado("text", r1.valor.ToString() + r2.valor.ToString());
                                case "integer":
                                    return new Resultado("text", r1.valor.ToString() + Convert.ToInt64(r2.valor.ToString()));
                                case "double":
                                    return new Resultado("text", r1.valor.ToString() + Convert.ToDouble(r2.valor.ToString()));
                                case "bool":
                                    return new Resultado("text", r1.valor.ToString() + Convert.ToInt64(r2.valor.ToString()));
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica + no complatible entre datos Text y Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "text":
                                    return new Resultado("text", Convert.ToInt64(r1.valor) + r2.valor.ToString());
                                case "integer":
                                    return new Resultado("integer", Convert.ToInt64(r1.valor) + Convert.ToInt64(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToInt64(r1.valor) + Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("integer", Convert.ToInt64(r1.valor) + Convert.ToInt64(r2.valor));
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica + no complatible entre datos Integer y Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "text":
                                    return new Resultado("text", Convert.ToDouble(r1.valor) + r2.valor.ToString());
                                case "integer":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) + Convert.ToInt64(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) + Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) + Convert.ToInt64(r2.valor));
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica + no complatible entre datos Double y Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "text":
                                    return new Resultado("text", Convert.ToInt64(r1.valor) + r2.valor.ToString());
                                case "integer":
                                    return new Resultado("integer", Convert.ToInt64(r1.valor) + Convert.ToInt64(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToInt64(r1.valor) + Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("integer", Convert.ToInt64(r1.valor) + Convert.ToInt64(r2.valor));
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica + no complatible entre datos Bool y Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "date":
                            switch (r2.tipo)
                            {
                                case "text":
                                    return new Resultado("text", r1.valor.ToString() + r2.valor.ToString());
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica + no complatible entre datos Date e Integer/Double/Bool/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "datetime":
                            switch (r2.tipo)
                            {
                                case "text":
                                    return new Resultado("text", r1.valor.ToString() + r2.valor.ToString());
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica + no complatible entre datos DateTime e Integer/Double/Bool/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;

                    }
                    break;
                #endregion FinSuma
                #region Resta
                case "-":
                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica - no complatible entre datos Text y Text/Integer/Double/Bool/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("integer", Convert.ToInt64(r1.valor) - Convert.ToInt64(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToInt64(r1.valor) - Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("integer", Convert.ToInt64(r1.valor) - Convert.ToInt64(r2.valor));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica - no complatible entre datos Integer y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) - Convert.ToInt64(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) - Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) - Convert.ToInt64(r2.valor));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica - no complatible entre datos Double y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("integer", Convert.ToInt64(r1.valor) - Convert.ToInt64(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToInt64(r1.valor) - Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("double", Convert.ToInt64(r1.valor) - Convert.ToInt64(r2.valor));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica - no complatible entre datos Bool y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "date":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica - no complatible entre datos Date y Text/Integer/Double/Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "datetime":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica - no complatible entre datos DateTime y Text/Integer/Double/Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;

                    }
                    break;
                #endregion FinResta
                #region Multiplicacion
                case "*":
                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica * no complatible entre datos Text y Text/Integer/Double/Bool/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("integer", Convert.ToInt64(r1.valor) * Convert.ToInt64(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToInt64(r1.valor) * Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("integer", Convert.ToInt64(r1.valor) * Convert.ToInt64(r2.valor));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica * no complatible entre datos Integer y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) * Convert.ToInt64(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) * Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) * Convert.ToInt64(r2.valor));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica * no complatible entre datos Double y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("integer", Convert.ToInt64(r1.valor) * Convert.ToInt64(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToInt64(r1.valor) * Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("double", Convert.ToInt64(r1.valor) * Convert.ToInt64(r2.valor));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica * no complatible entre datos Bool y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "date":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica * no complatible entre datos Date y Text/Integer/Double/Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "datetime":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica * no complatible entre datos DateTime y Text/Integer/Double/Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;

                    }
                    break;
                #endregion FinMultiplicacion
                #region Division
                case "/":
                    //validacion division entre 0
                    if (r2.tipo.Equals("integer") || r2.tipo.Equals("double") || r2.tipo.Equals("bool"))
                    {
                        if (Convert.ToDouble(r2.valor) == 0)
                        {
                            agregarError("Semantico", "Un numero no puede ser dividido dentro de 0", raiz.Span.Location.Line, raiz.Span.Location.Column);
                            return new Resultado("Error", null);
                        }
                    }
                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica / no complatible entre datos Text y Text/Integer/Double/Bool/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) / Convert.ToDouble(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) / Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) / Convert.ToDouble(r2.valor));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica / no complatible entre datos Integer y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) / Convert.ToDouble(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) / Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) / Convert.ToDouble(r2.valor));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica - no complatible entre datos Double y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) / Convert.ToInt64(r2.valor));
                                case "double":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) / Convert.ToDouble(r2.valor));
                                case "bool":
                                    return new Resultado("double", Convert.ToDouble(r1.valor) / Convert.ToDouble(r2.valor));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica / no complatible entre datos Bool y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "date":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica / no complatible entre datos Date y Text/Integer/Double/Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "datetime":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica / no complatible entre datos DateTime y Text/Integer/Double/Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;

                    }
                    break;
                #endregion FinDivision
                #region Potencia
                case "^":
                    //validacion resultado infinito 

                    if (r1.tipo.Equals("integer") || r1.tipo.Equals("double") || r1.tipo.Equals("bool"))
                    {
                        if (Convert.ToDouble(r2.valor) < 1 && Convert.ToDouble(r1.valor) < 0)
                        {
                            agregarError("Semantico", "Resultado indefinido en operacion ^", raiz.Span.Location.Line, raiz.Span.Location.Column);
                            return new Resultado("Error", null);
                        }
                    }

                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica ^ no complatible entre datos Text y Text/Integer/Double/Bool/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("double", Math.Pow(Convert.ToDouble(r1.valor), Convert.ToDouble(r2.valor)));
                                case "double":
                                    return new Resultado("double", Math.Pow(Convert.ToDouble(r1.valor), Convert.ToDouble(r2.valor)));
                                case "bool":
                                    return new Resultado("double", Math.Pow(Convert.ToDouble(r1.valor), Convert.ToDouble(r2.valor)));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica ^ no complatible entre datos Integer y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("double", Math.Pow(Convert.ToDouble(r1.valor), Convert.ToDouble(r2.valor)));
                                case "double":
                                    return new Resultado("double", Math.Pow(Convert.ToDouble(r1.valor), Convert.ToDouble(r2.valor)));
                                case "bool":
                                    return new Resultado("double", Math.Pow(Convert.ToDouble(r1.valor), Convert.ToDouble(r2.valor)));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica ^ no complatible entre datos Double y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "integer":
                                    return new Resultado("double", Math.Pow(Convert.ToDouble(r1.valor), Convert.ToInt64(r2.valor)));
                                case "double":
                                    return new Resultado("double", Math.Pow(Convert.ToDouble(r1.valor), Convert.ToDouble(r2.valor)));
                                case "bool":
                                    return new Resultado("double", Math.Pow(Convert.ToDouble(r1.valor), Convert.ToDouble(r2.valor)));
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica ^ no complatible entre datos Bool y Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "date":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica / no complatible entre datos Date y Text/Integer/Double/Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);
                            }
                            break;
                        case "datetime":
                            switch (r2.tipo)
                            {
                                case "text":
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Operacion aritmetica / no complatible entre datos DateTime y Text/Integer/Double/Text/Date/DateTime", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;

                    }
                    break;
                    #endregion FinPotencia

            }

            return new Resultado("Error", null);

        }

        private Resultado getVariable(ParseTreeNode nodoVariables)
        {
            int contador = 0;
            Resultado resultado = null;
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
                        resultado = new Resultado(s.tipo, s.valor);
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
                        resultado = new Resultado(atributo.tipo, atributo.valor);
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

        public Resultado getValorCampo(ParseTreeNode raiz)
        {
            String nombreCampo = "";
            foreach (ParseTreeNode nodo in raiz.ChildNodes)
            {
                if (nombreCampo.Equals(""))
                {
                    nombreCampo = nodo.Token.Text;
                }
                else
                {
                    nombreCampo = nombreCampo + "." + nodo.Token.Text;
                }
            }
            nombreCampo = nombreCampo.ToLower();
            if (tuplaActual!=null)
            {
                foreach (campo cmp in tuplaActual.campos)
                {
                    if (raiz.ChildNodes.Count > 1)
                    {
                        if (cmp.id.ToLower().Contains(nombreCampo))
                        {
                            return new Resultado(cmp.tipo,cmp.valor);
                        }
                    }
                    else
                    {
                        if (cmp.id.ToLower().Contains("."+nombreCampo))
                        {
                            return new Resultado(cmp.tipo, cmp.valor);
                        }
                    }

                }
            }
            else
            {
                agregarError("Semantico", "Campo " + nombreCampo + " no existe en la tabla", raiz.Span.Location.Line, raiz.Span.Location.Column);
                return new Resultado("Error",null);
            }
            agregarError("Semantico", "Campo " + nombreCampo + " no existe en la tabla", raiz.Span.Location.Line, raiz.Span.Location.Column);
            return new Resultado("Error", null);
        }

    }
}
