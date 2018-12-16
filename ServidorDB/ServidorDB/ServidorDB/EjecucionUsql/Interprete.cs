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
                        String campoOrdenamiento = "";
                        int orden = 2;
                        ParseTreeNode condicion = null;
                        foreach (ParseTreeNode nodo in hijo.ChildNodes[1].ChildNodes)
                        {
                            tablas.Add(nodo.ChildNodes[0].Token.Text);
                        }
                        foreach (ParseTreeNode nodoN in hijo.ChildNodes[0].ChildNodes)
                        {
                            if (nodoN.ChildNodes.Count == 1)
                            {
                                if (tablas.Count == 1)
                                {
                                    campos.Add(tablas[0] + "." + nodoN.ChildNodes[0].Token.Text);
                                }
                                else
                                {
                                    campos.Add(nodoN.ChildNodes[0].Token.Text);
                                }                                
                            }
                            else
                            {
                                campos.Add(nodoN.ChildNodes[0].Token.Text + "." + nodoN.ChildNodes[1].Token.Text);
                            }
                        }                        
                        /*Si hay tres nodos existe where y ordenamiento*/
                        if (hijo.ChildNodes[2].ChildNodes.Count == 3)
                        {
                            if (tablas.Count == 1)
                            {
                                campoOrdenamiento = tablas[0] + "." + hijo.ChildNodes[2].ChildNodes[0].ChildNodes[0].Token.Text;
                            }
                            else
                            {
                                campoOrdenamiento = hijo.ChildNodes[2].ChildNodes[1].ChildNodes[0].Token.Text
                                    + "." + hijo.ChildNodes[2].ChildNodes[1].ChildNodes[1].Token.Text;
                            }
                            condicion = hijo.ChildNodes[2].ChildNodes[0];

                            if(hijo.ChildNodes[2].ChildNodes[2].Token.Text.ToLower().Equals("asc"))
                            {
                                orden = 0; // Cero es ascendente.
                            }
                            if (hijo.ChildNodes[2].ChildNodes[2].Token.Text.ToLower().Equals("desc"))
                            {
                                orden = 1; // Cero es ascendente.
                            }
                        }
                        if (hijo.ChildNodes[2].ChildNodes.Count == 2)// Ordenamiento
                        {
                            if (hijo.ChildNodes[2].ChildNodes[0].ChildNodes.Count == 2)
                            {
                                campoOrdenamiento = hijo.ChildNodes[2].ChildNodes[0].ChildNodes[0].Token.Text
                                    + "." + hijo.ChildNodes[2].ChildNodes[0].ChildNodes[1].Token.Text;
                            }
                            else
                            {
                                if (tablas.Count == 1)
                                {
                                    campoOrdenamiento = tablas[0] + "." + hijo.ChildNodes[2].ChildNodes[0].ChildNodes[0].Token.Text;
                                }
                                else
                                {
                                    campoOrdenamiento = hijo.ChildNodes[2].ChildNodes[1].ChildNodes[0].Token.Text
                                        + "." + hijo.ChildNodes[2].ChildNodes[1].ChildNodes[1].Token.Text;
                                }
                            }
                            if (hijo.ChildNodes[2].ChildNodes[1].Token.Text.ToLower().Equals("asc"))
                            {
                                orden = 0; // Cero es ascendente.
                            }
                            if (hijo.ChildNodes[2].ChildNodes[1].Token.Text.ToLower().Equals("desc"))
                            {
                                orden = 1; // Cero es ascendente.
                            }
                        }
                        if (hijo.ChildNodes[2].ChildNodes.Count ==1)
                        {
                            condicion = hijo.ChildNodes[2].ChildNodes[0];
                        }
                        return new Resultado("tuplas",sistemaActual.basesdedatos[0].seleccionar(campos, tablas, condicion, campoOrdenamiento, orden));                        
                }
            }
            return new Resultado("null",null);
        }
    }
}
