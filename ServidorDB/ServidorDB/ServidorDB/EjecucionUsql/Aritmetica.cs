using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorBDD.EjecucionUsql
{
    class Aritmetica
    {

        public Aritmetica()
        {
        }

        public Resultado operar(ParseTreeNode raiz)
        {
            Resultado r1 = null;
            Resultado r2 = null;
            switch (raiz.Term.Name)
            {
                case "EXPL":
                case "EXPR":
                    return operar(raiz.ChildNodes[0]);
                case "EXPA":
                    if (raiz.ChildNodes.Count == 3)
                    {
                        r1 = operar(raiz.ChildNodes[0]);
                        r2 = operar(raiz.ChildNodes[2]);
                    }
                    else
                    {
                        return operar(raiz.ChildNodes[0]);
                    }
                    break;
                case "Integer":
                    return new Resultado("Integer",raiz.Token.Text);
            }
            
            switch(raiz.ChildNodes[1].Token.Text)
            {
                #region Suma
                case "+":
                    switch(r1.tipo)
                    {
                        case "Text":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Integer":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    return new Resultado("Integer", Convert.ToInt32(r1.valor) + Convert.ToInt32(r2.valor));
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Double":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Bool":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Date":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "DateTime":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;

                    }
                    break;
                    #endregion FinSuma
                #region Resta
                case "-":
                    switch (r1.tipo)
                    {
                        case "Text":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Integer":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    return new Resultado("Integer", Convert.ToInt32(r1.valor) - Convert.ToInt32(r2.valor));
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Double":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Bool":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Date":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "DateTime":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;

                    }
                    break;
                #endregion FinResta
                #region Multiplicacion
                case "*":
                    switch (r1.tipo)
                    {
                        case "Text":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Integer":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    return new Resultado("Integer", Convert.ToInt32(r1.valor) * Convert.ToInt32(r2.valor));
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Double":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Bool":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Date":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "DateTime":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;

                    }
                    break;
                #endregion FinMultiplicacion
                #region Division
                case "/":
                    switch (r1.tipo)
                    {
                        case "Text":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Integer":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    return new Resultado("Double", Convert.ToInt32(r1.valor) + Convert.ToInt32(r2.valor));
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Double":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Bool":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Date":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "DateTime":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;

                    }
                    break;
                #endregion FinDivision
                #region Potencia
                case "^":
                    switch (r1.tipo)
                    {
                        case "Text":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Integer":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    return new Resultado("Integer", Math.Pow(Convert.ToInt32(r1.valor),Convert.ToInt32(r2.valor)));
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Double":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Bool":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "Date":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;
                        case "DateTime":
                            switch (r2.tipo)
                            {
                                case "Text":
                                    break;
                                case "Integer":
                                    break;
                                case "Double":
                                    break;
                                case "Bool":
                                    break;
                                case "Date":
                                    break;
                                case "DateTime":
                                    break;

                            }
                            break;

                    }
                    break;
                    #endregion FinPotencia
                
            }

            return new Resultado("null","null");

        }
    }
}
