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

        public Resultado seleccionar2(ParseTreeNode hijo)
        {
            // Siempre van a haber 3 hijos 
            // LISTACAMPOS [0]. LISTATABLAS[1]. CONDICIONES[2]
            ParseTreeNode campos, tablas, condicion, campoOrdenamiento, orden;
            campos = hijo.ChildNodes[0];
            tablas = hijo.ChildNodes[1];            
            switch (hijo.ChildNodes[2].ChildNodes.Count)
            {
                case 1:
                    condicion = hijo.ChildNodes[2].ChildNodes[0];
                    break;
                case 2:                    
                    campoOrdenamiento = hijo.ChildNodes[2].ChildNodes[0];
                    orden = hijo.ChildNodes[2].ChildNodes[1];
                    break;
                case 3:
                    condicion = hijo.ChildNodes[2].ChildNodes[0];
                    campoOrdenamiento = hijo.ChildNodes[2].ChildNodes[1];
                    orden = hijo.ChildNodes[2].ChildNodes[2];
                    break;
            }
            return new Resultado
                (
                    "tuplas",
                    Form1.sistemaArchivos
                );
            //return new Resultado("tuplas", 
                //sistemaActual.basesdedatos[0].seleccionar(campos, tablas, condicion, campoOrdenamiento, orden));




            //return new Resultado("tuplas", 
            //    sistemaActual.basesdedatos[0].seleccionar(, tablas, condicion, campoOrdenamiento, orden));

            return null;
        }
        public Resultado seleccionar(ParseTreeNode hijo)
        {
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

                if (hijo.ChildNodes[2].ChildNodes[2].Token.Text.ToLower().Equals("asc"))
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
                    // Verificamos si hay más de una tabla.
                    // Formato esperado : NombreTabla.NombreCampo
                    if (tablas.Count == 1)
                    {
                        campoOrdenamiento = tablas[0] + "." + hijo.ChildNodes[2].ChildNodes[0].ChildNodes[0].Token.Text;
                    }
                    else
                    {
                        if (hijo.ChildNodes[2].ChildNodes[1].ChildNodes.Count == 2)
                        {
                            campoOrdenamiento = hijo.ChildNodes[2].ChildNodes[1].ChildNodes[0].Token.Text
                                + "." + hijo.ChildNodes[2].ChildNodes[1].ChildNodes[1].Token.Text;
                        }
                        else
                        {
                            Form1.Mensajes.Add(new Error("Semantico",
                                "El campo para ordenar debe estar en formato NombreTabla.NombreCampo para poder encontrarse",
                                  hijo.ChildNodes[2].ChildNodes[1].ChildNodes[0].Token.Location.Line
                                , hijo.ChildNodes[2].ChildNodes[1].ChildNodes[0].Token.Location.Column
                                ).getMensaje());
                        }

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
            if (hijo.ChildNodes[2].ChildNodes.Count == 1)
            {
                condicion = hijo.ChildNodes[2].ChildNodes[0];
            }
            return new Resultado("tuplas", sistemaActual.basesdedatos[0].seleccionar(campos, tablas, condicion, campoOrdenamiento, orden));
        }
    }
}
