using Irony.Parsing;
using ServidorDB;
using ServidorDB.EjecucionUsql;
using ServidorDB.estructurasDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorBDD.EjecucionUsql
{
    public class Relacional
    {
        Aritmetica opA;
        public tupla tuplaActual;
        public Relacional()
        {
        }
        public Relacional(tupla tuplaActual)
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
                    return operar(raiz.ChildNodes[0]);
                case "EXPR":
                    if (raiz.ChildNodes.Count == 3)
                    {
                        r1 = operar(raiz.ChildNodes[0]);
                        r1.tipo = r1.tipo.ToLower();
                        r2 = operar(raiz.ChildNodes[2]);
                        r2.tipo = r2.tipo.ToLower();
                    }
                    else
                    {
                        //corregir
                        opA = new Aritmetica(tuplaActual);
                        return opA.operar(raiz.ChildNodes[0]);
                    }

                    break;
                case "EXPA":
                    opA = new Aritmetica(tuplaActual);
                    return opA.operar(raiz);
            }

            switch (raiz.ChildNodes[1].Token.Text)
            {
                #region igualIgual
                case "==":
                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                    if (r1.valor.ToString().Equals(r2.valor.ToString()))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "integer":
                                case "bool":
                                    if (Convert.ToInt64(r1.valor) == Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) == Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "double":
                                    if (Convert.ToDouble(r1.valor) == Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "bool":
                                    if (Convert.ToDouble(r1.valor) == Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "bool":
                                case "integer":
                                    if (Convert.ToInt64(r1.valor) == Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) == Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "date":
                            String formato;
                            DateTime date1;
                            DateTime date2;
                            switch (r2.tipo)
                            {
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 == date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "datetime":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    if (date1 == date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "text":
                                case "integer":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "datetime":

                            switch (r2.tipo)
                            {
                                case "datetime":
                                    formato = "dd-MM-yyyy hh:mm:ss";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 == date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 == date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "text":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                    }
                    break;
                #endregion Fin igualIgual
                #region NoIgual
                case "!=":
                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                    if (!r1.valor.ToString().Equals(r2.valor.ToString()))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "integer":
                                case "bool":
                                    if (Convert.ToInt64(r1.valor) != Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) != Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "double":
                                    if (Convert.ToDouble(r1.valor) != Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "bool":
                                    if (Convert.ToDouble(r1.valor) != Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "bool":
                                case "integer":
                                    if (Convert.ToInt64(r1.valor) != Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) != Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "date":
                            String formato;
                            DateTime date1;
                            DateTime date2;
                            switch (r2.tipo)
                            {
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 != date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "datetime":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    if (date1 != date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "text":
                                case "integer":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "datetime":
                            switch (r2.tipo)
                            {
                                case "datetime":
                                    formato = "dd-MM-yyyy hh:mm:ss";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 != date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 != date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "text":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                    }
                    break;
                #endregion Fin NoIgual
                #region Menor
                case "<":
                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                    int diferencia = String.CompareOrdinal(r1.valor.ToString(), r2.valor.ToString());
                                    if (diferencia < 0)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "integer":
                                case "bool":
                                    if (Convert.ToInt64(r1.valor) < Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) < Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "double":
                                    if (Convert.ToDouble(r1.valor) < Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "bool":
                                    if (Convert.ToDouble(r1.valor) < Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "bool":
                                case "integer":
                                    if (Convert.ToInt64(r1.valor) < Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) < Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "date":
                            String formato;
                            DateTime date1;
                            DateTime date2;
                            switch (r2.tipo)
                            {
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 < date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "datetime":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    if (date1 < date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "text":
                                case "integer":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "datetime":
                            switch (r2.tipo)
                            {
                                case "datetime":
                                    formato = "dd-MM-yyyy hh:mm:ss";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 < date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 < date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "integer":
                                case "text":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                    }
                    break;
                #endregion Fin Menor
                #region Mayor
                case ">":
                    Resultado aux = r1;
                    r1 = r2;
                    r2 = aux;
                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                    int diferencia = String.CompareOrdinal(r1.valor.ToString(), r2.valor.ToString());
                                    if (diferencia < 0)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "integer":
                                case "bool":
                                    if (Convert.ToInt64(r1.valor) < Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) < Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "double":
                                    if (Convert.ToDouble(r1.valor) < Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "bool":
                                    if (Convert.ToDouble(r1.valor) < Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "bool":
                                case "integer":
                                    if (Convert.ToInt64(r1.valor) < Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) < Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "date":
                            String formato;
                            DateTime date1;
                            DateTime date2;
                            switch (r2.tipo)
                            {
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 < date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "datetime":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    if (date1 < date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "text":
                                case "integer":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "datetime":
                            switch (r2.tipo)
                            {
                                case "datetime":
                                    formato = "dd-MM-yyyy hh:mm:ss";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 < date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 < date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "integer":
                                case "text":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                    }
                    break;
                #endregion Fin Mayor
                #region Menor igual
                case "<=":
                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                    int diferencia = String.CompareOrdinal(r1.valor.ToString(), r2.valor.ToString());
                                    if (diferencia <= 0)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "integer":
                                case "bool":
                                    if (Convert.ToInt64(r1.valor) <= Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) <= Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "double":
                                    if (Convert.ToDouble(r1.valor) <= Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "bool":
                                    if (Convert.ToDouble(r1.valor) <= Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "bool":
                                case "integer":
                                    if (Convert.ToInt64(r1.valor) <= Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) <= Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "date":
                            String formato;
                            DateTime date1;
                            DateTime date2;
                            switch (r2.tipo)
                            {
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 <= date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "datetime":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    if (date1 <= date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "text":
                                case "integer":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "datetime":
                            switch (r2.tipo)
                            {
                                case "datetime":
                                    formato = "dd-MM-yyyy hh:mm:ss";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 <= date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 <= date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "integer":
                                case "text":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                    }
                    break;
                #endregion Fin Menor igual
                #region Mayorigual
                case ">=":
                    Resultado aux2 = r1;
                    r1 = r2;
                    r2 = aux2;
                    switch (r1.tipo)
                    {
                        case "text":
                            switch (r2.tipo)
                            {
                                case "text":
                                    int diferencia = String.CompareOrdinal(r1.valor.ToString(), r2.valor.ToString());
                                    if (diferencia <= 0)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "double":
                                case "bool":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "integer":
                            switch (r2.tipo)
                            {
                                case "integer":
                                case "bool":
                                    if (Convert.ToInt64(r1.valor) <= Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) <= Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "double":
                            switch (r2.tipo)
                            {
                                case "double":
                                    if (Convert.ToDouble(r1.valor) <= Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "integer":
                                case "bool":
                                    if (Convert.ToDouble(r1.valor) <= Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "bool":
                            switch (r2.tipo)
                            {
                                case "bool":
                                case "integer":
                                    if (Convert.ToInt64(r1.valor) <= Convert.ToInt64(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                    if (Convert.ToInt64(r1.valor) <= Convert.ToDouble(r2.valor))
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "text":
                                case "date":
                                case "datetime":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "date":
                            String formato;
                            DateTime date1;
                            DateTime date2;
                            switch (r2.tipo)
                            {
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 <= date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "datetime":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    if (date1 <= date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "text":
                                case "integer":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                        case "datetime":
                            switch (r2.tipo)
                            {
                                case "datetime":
                                    formato = "dd-MM-yyyy hh:mm:ss";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 <= date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "date":
                                    formato = "dd-MM-yyyy";
                                    date1 = DateTime.ParseExact(r1.valor.ToString(),
                                    formato + " hh:mm:ss", CultureInfo.InvariantCulture);
                                    date2 = DateTime.ParseExact(r2.valor.ToString(),
                                    formato, CultureInfo.InvariantCulture);
                                    if (date1 <= date2)
                                    {
                                        return new Resultado("bool", 1);
                                    }
                                    else
                                    {
                                        return new Resultado("bool", 0);
                                    }
                                case "double":
                                case "bool":
                                case "integer":
                                case "text":
                                    agregarError("Semantico", "Solo se pueden comparar valores del mismo tipo", raiz.Span.Location.Line, raiz.Span.Location.Column);
                                    return new Resultado("Error", null);

                            }
                            break;
                    }
                    break;
                    #endregion Fin Mayor igual
            }

            return new Resultado("Error", null);
        }

        private void agregarError(String tipo, String descripcion, int linea, int columna)
        {
            Error error = new Error(tipo, descripcion, linea, columna);
            bool existe = false;
            foreach(Error err in Form1.errores)
            {
                if (err.getMensaje().Equals(error.getMensaje()))
                {
                    existe = true;
                }
            }
            if (!existe)
            {
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
            
        }
    }
}
