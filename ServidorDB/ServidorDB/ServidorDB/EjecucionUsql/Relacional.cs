using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorBDD.EjecucionUsql
{
    class Relacional
    {
        Aritmetica opA;
        public Relacional()
        {
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
                    r1 = operar(raiz.ChildNodes[0]);
                    r2 = operar(raiz.ChildNodes[2]);
                    break;
                case "EXPA":
                    opA = new Aritmetica();
                    return opA.operar(raiz);
            }

            switch(raiz.ChildNodes[1].Token.Text)
            {
                #region igualIgual
                case "==":
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
                                    if (Convert.ToInt32(r1.valor) == Convert.ToInt32(r2.valor))
                                    {
                                        return new Resultado("Bool",true);
                                    }
                                    else
                                    {
                                        return new Resultado("Bool",false);
                                    }
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
                #endregion Fin igualIgual

            }

            return new Resultado("null",null);
        }
    }
}
