using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServidorDB.estructurasDB;

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
                switch(tipoAccion){
                    case "DECLARACION":
                        Aritmetica EXPA = new Aritmetica();
                        Relacional EXPR = new Relacional();
                        Resultado r = EXPR.operar(hijo.ChildNodes[2]);
                        break;
                    case "SELECCIONAR":
                        List<String> campos = new List<String>();
                        List<String> tablas = new List<String>();
                        foreach (ParseTreeNode nodo in hijo.ChildNodes[1].ChildNodes)
                        {
                            tablas.Add(nodo.ChildNodes[0].Token.Text);
                        }
                        foreach (ParseTreeNode nodoN in hijo.ChildNodes[0].ChildNodes)
                        {
                            campos.Add(nodoN.ChildNodes[0].Token.Text + "." + nodoN.ChildNodes[1].Token.Text);
                        }
                        

                        ParseTreeNode condicion = hijo.ChildNodes[2].ChildNodes[0];
                        String campoOrdenamiento = "";
                        int orden = 2;                       
                        return new Resultado("tuplas",sistemaActual.basesdedatos[0].seleccionar(campos, tablas, condicion, campoOrdenamiento, orden));                        
                }
            }
            return new Resultado("null",null);
        }
    }
}
