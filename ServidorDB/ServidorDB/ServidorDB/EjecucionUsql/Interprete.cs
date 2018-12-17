using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServidorDB.estructurasDB;
using ServidorDB;
using ServidorDB.EjecucionUsql;

namespace ServidorBDD.EjecucionUsql
{
    class Interprete
    {
        public SistemaArchivos sistemaActual;
        public Resultado ejecutar(ParseTreeNode raiz)
        {
            foreach (ParseTreeNode hijo in raiz.ChildNodes)
            {
                String tipoAccion = hijo.Term.Name;
                switch (tipoAccion) {
                    case "DECLARACION":
                        Aritmetica EXPA = new Aritmetica();
                        Relacional EXPR = new Relacional();
                        Resultado r = EXPR.operar(hijo.ChildNodes[2]);
                        break;
                    case "SELECCIONAR":
                        Form1.sistemaArchivos.realizarConsulta(hijo);
                        break;
                    case "LISTDDL":
                        ejecutar(hijo);
                        break;
                    case "USAR":
                        Form1.sistemaArchivos.setBaseActual(hijo.ChildNodes[0]);
                        break;
                }
            }
            return new Resultado("null", null);
        }
    }
}
