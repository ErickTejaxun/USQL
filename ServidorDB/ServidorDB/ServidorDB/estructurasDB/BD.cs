using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using ServidorBDD.EjecucionUsql;
using ServidorDB.EjecucionUsql;

namespace ServidorDB.estructurasDB
{
    public class BD
    {
        public String nombre;
        public String path;
        public List<Tabla> tablas;
        public List<Objeto> objetos;
        public List<Procedimiento> procedimientos;        

        // Constructor de la clase
        public BD(String nombre, String path)
        {
            this.nombre = nombre;
            this.path = path;
            this.tablas = new List<Tabla>();
            this.objetos = new List<Objeto>();
            this.procedimientos = new List<Procedimiento>();
        }
        //< @param orden: 1 ascendente, 0 descendente>
        //< @param campoOrdenacion:Nombre del campo para ordenar>
        //< @param Campos: campos a seleccionar>
        //< @param listaTabla: lista de nombres de las tablas>
        //< @param raiz : Raiz de la condición a cumplirse>
        public String seleccionar(List<String> listaCampos, 
            List<String> listaTablas, ParseTreeNode raiz, String campoOrdenacion, int orden)
        {            
            String data = "\n";
            List<tupla> cartesianoTemporal = new List<tupla>();
            List<tupla> cartesiano = new List<tupla>();
            /*Primero realizamos el producto cartesiano de las tablas involucradas*/
            foreach (String ntab in listaTablas)
            {

                if (buscarTabla(ntab) == null)
                {
                    data =  data + "Error: Tabla " + ntab + " no existe en la base de datos. \n";
                    cartesianoTemporal = productoCartesiano(cartesianoTemporal, new List<tupla>(), ntab);
                }
                else
                {
                    cartesianoTemporal = productoCartesiano(cartesianoTemporal, buscarTabla(ntab), ntab);
                }
                
            }
            /*Verificamos las condiciones para filtrar resultados*/
            foreach (tupla tp in cartesianoTemporal)
            {
                if (comprobarCondicion(tp, raiz))
                {
                    cartesiano.Add(tp);
                }
            }

            /*Elegimos las celdas que se solicitan. Las demás se descartan*/
            /*Tambien se ordena los resultados según el usuario haya indicado*/
            cartesiano = filtrarResultados(listaCampos, cartesiano);

            /**/
            cartesiano = ordenarResultados(cartesiano, campoOrdenacion, orden);

            #region imprimir resultado
            if (cartesiano.Count > 0)
            {
                foreach (campo cp in cartesiano[0].campos)
                {
                    if (data.Equals("\n"))
                    {
                        data = cp.id;
                    }
                    else
                    {
                        data = data + "," + cp.id;
                    }
                }
            }
            else
            {
                return data + "\n Sin resultados";
            }
            data = data + "\n";
            foreach (tupla tpm in cartesiano)
            {
                bool flag = false;
                foreach (campo cmp in tpm.campos)
                {
                    if (!flag)
                    {
                        data = data + cmp.valor.ToString();
                        flag = true;
                    }
                    else
                    {
                        data = data + "," + cmp.valor.ToString();
                    }                    
                }
                data = data + "\n";
            }
            #endregion

            return data;
        }
        public List<tupla> ordenarResultados(List<tupla> listaTuplas, String campoOrdenacion, int orden)
        {
            // Si no hay filas, salimos de la función.
            if (listaTuplas.Count == 0 || campoOrdenacion.Equals("")) { return listaTuplas; }
            // El dos o mayor significa que no debe ordenarse.
            if (orden>2)
            {
                return listaTuplas;
            }

            // Ahora encontramos el índice por el cuál vamos a ordenar.
            int indice = 0;
            bool encontrada = false;
            foreach (campo cp in listaTuplas[0].campos)
            {                
                if (cp.id.ToLower().Equals(campoOrdenacion.ToLower()))
                {
                    encontrada = true;
                    break;                    
                }
                indice++;
            }
            if(!encontrada)
            {
                Form1.Mensajes.Add("Error en tiempo de ejecución: El campo '" + campoOrdenacion + "' no existe en los resultados. No se ordenarán los resultados");
                //return new List<tupla>();
                return listaTuplas;
            } 
            List<tupla> listaOrdenada = new List<tupla>();            
            if (orden == 0)
            {                
                var ordenado =
                    from campos in listaTuplas
                    orderby campos.campos[indice].valor ascending
                    select new { campos };
                foreach (var data in ordenado)
                {
                    tupla tpm = new tupla();
                    foreach (campo cmp in data.campos.campos)
                    {
                        tpm.addCampo(cmp);
                    }
                    listaOrdenada.Add(tpm);
                }
            }
            if (orden == 1)
            {
                var ordenado =
                    from campos in listaTuplas
                    orderby campos.campos[indice].valor descending
                    select new { campos };
                foreach (var data in ordenado)
                {
                    tupla tpm = new tupla();
                    foreach (campo cmp in data.campos.campos)
                    {
                        tpm.addCampo(cmp);
                    }
                    listaOrdenada.Add(tpm);
                }
            }

            return listaOrdenada;
        }
        public Boolean comprobarCondicion(tupla tup, ParseTreeNode raiz)
        {
            if (raiz==null)
            {
                return true;
            }
            Relacional rel = new Relacional();
            Resultado result = rel.operar(raiz);
            return (bool) result.valor;            
        }
        public List<tupla> filtrarResultados(List<String> listaCampos, List<tupla> cartesiano)
        {
            List<tupla> listaFiltrada = new List<tupla>();
            #region Eleccion de campos
            if (listaCampos.Count == 0)
            {
                return cartesiano;
            }
            foreach (tupla tp in cartesiano)
            {
                tupla nuevaTupla = new tupla();
                foreach (campo cp in tp.campos)
                {
                    foreach (String campoBuscado in listaCampos)
                    {
                        if (campoBuscado.ToLower().Equals(cp.id.ToLower()))// Es un campo buscado
                        {
                            nuevaTupla.addCampo(new campo(cp.id, cp.valor));
                        }
                    }
                }
                if (nuevaTupla.campos.Count !=0)
                {
                    listaFiltrada.Add(nuevaTupla);
                }
                
            }
            #endregion
            #region Ordenar salida 
            List<tupla> listaFinal = new List<tupla>();
            foreach (tupla tp in listaFiltrada)
            {
                tupla newTp = new tupla();
                foreach (String nombre in listaCampos)
                {
                    foreach (campo cp in tp.campos)
                    {
                        if (nombre.Equals(cp.id))
                        {
                            newTp.addCampo(new campo(cp.id, cp.valor));
                        }
                    }
                }
                listaFinal.Add(newTp);
            }
            #endregion
            if (listaFinal.Count==0) { Form1.Mensajes.Add(new Error("Semantico", "Los campos indicados no existen en las tablas.", 0, 0).getMensaje()); }
            return listaFinal;
        }
        public List<tupla> productoCartesiano(List<tupla> tab1, List<tupla> tab2 , String nombre)
        {
            List<tupla> tablaCar = new List<tupla>();
            if (tab1.Count > 0 && tab2.Count > 0)
            {
                foreach (tupla tp in tab2)
                {
                    foreach (campo cp in tp.campos)
                    {
                        if (!cp.id.Contains("."))
                        {
                            cp.id = nombre + "." + cp.id;
                        }                        
                    }
                }

                var cartesiano = from campo1 in tab1
                                 from campo2 in tab2
                                 select new { campo1, campo2 };
                foreach (var data in cartesiano)
                {
                    tupla tpm = new tupla();
                    foreach (campo cmp in data.campo1.campos)
                    {
                        tpm.addCampo(new campo(cmp.id, cmp.valor));
                    }
                    foreach (campo cmp in data.campo2.campos)
                    {
                        tpm.addCampo(new campo(cmp.id, cmp.valor));
                    }
                    tablaCar.Add(tpm);
                }
            }
            else if (tab1.Count > 0)
            {
                foreach (tupla tp in tab1)
                {
                    foreach (campo cp in tp.campos)
                    {
                        if (!cp.id.Contains("."))
                        {
                            cp.id = nombre + "." + cp.id;
                        }
                    }
                }
                return tab1;
            }
            else if (tab2.Count > 0)
            {
                foreach (tupla tp in tab2)
                {
                    foreach (campo cp in tp.campos)
                    {
                        if (!cp.id.Contains("."))
                        {
                            cp.id = nombre + "." + cp.id;
                        }                        
                    }
                }
                return tab2;
            }
            return tablaCar;
        }
        public List<tupla> buscarTabla(String id)
        {
            foreach (Tabla tab in tablas)
            {
                if (tab.nombre.Equals(id))
                {
                    return tab.tuplas;
                }
            }
            return null;            
        }


        #region para manejar errores.

        public void seleccionar(ParseTreeNode hijo)
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
                                  hijo.ChildNodes[2].ChildNodes[0].Token.Location.Line
                                , hijo.ChildNodes[2].ChildNodes[0].Token.Location.Column
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
            Form1.Mensajes.Add(seleccionar(campos, tablas, condicion, campoOrdenamiento, orden));
        }
        //public List<tupla> filtrarResultados( ParseTreeNode raiz, List<tupla> cartesiano)
        //{
        //    List<String> listaCampos = new List<String>;
        //    foreach ()
        //    {
        //    }



        //    List<tupla> listaFiltrada = new List<tupla>();
        //    #region Eleccion de campos
        //    if (listaCampos.Count == 0)
        //    {
        //        return cartesiano;
        //    }
        //    foreach (tupla tp in cartesiano)
        //    {
        //        tupla nuevaTupla = new tupla();
        //        foreach (campo cp in tp.campos)
        //        {
        //            foreach (String campoBuscado in listaCampos)
        //            {
        //                if (campoBuscado.ToLower().Equals(cp.id.ToLower()))// Es un campo buscado
        //                {
        //                    nuevaTupla.addCampo(new campo(cp.id, cp.valor));
        //                }
        //            }
        //        }
        //        if (nuevaTupla.campos.Count != 0)
        //        {
        //            listaFiltrada.Add(nuevaTupla);
        //        }

        //    }
        //    #endregion
        //    #region Ordenar salida 
        //    List<tupla> listaFinal = new List<tupla>();
        //    foreach (tupla tp in listaFiltrada)
        //    {
        //        tupla newTp = new tupla();
        //        foreach (String nombre in listaCampos)
        //        {
        //            foreach (campo cp in tp.campos)
        //            {
        //                if (nombre.Equals(cp.id))
        //                {
        //                    newTp.addCampo(new campo(cp.id, cp.valor));
        //                }
        //            }
        //        }
        //        listaFinal.Add(newTp);
        //    }
        //    #endregion
        //    if (listaFinal.Count == 0) { Form1.Mensajes.Add(new Error("Semantico", "Los campos indicados no existen en las tablas.", 0, 0).getMensaje()); }
        //    return listaFinal;
        //}
        public List<tupla> buscarTabla( ParseTreeNode raiz)
        {
            String id = raiz.ChildNodes[0].Token.Text;
            foreach (Tabla tab in tablas)
            {
                if (tab.nombre.Equals(id))
                {
                    return tab.tuplas;
                }
            }
            Form1.Mensajes.Add( new Error("Semantico", "La tabla: " + id + "No existe en la base de datos seleccionada", raiz.ChildNodes[0].Token.Location.Line, raiz.ChildNodes[0].Token.Location.Column).getMensaje());
            return null;
        }

        //public String seleccionar(ParseTreeNode raizCamposSeleccionados,
        //    ParseTreeNode raizTablasSeleccionadas, ParseTreeNode raizComprobacion, ParseTreeNode campoOrdenamiento, ParseTreeNode orden)
        //{
        //    String data = "\n";
        //    List<tupla> cartesianoTemporal = new List<tupla>();
        //    List<tupla> cartesiano = new List<tupla>();
        //    /*Primero realizamos el producto cartesiano de las tablas involucradas*/
        //    foreach (String ntab in listaTablas)
        //    {

        //        if (buscarTabla(ntab) == null)
        //        {
        //            data = data + "Error: Tabla " + ntab + " no existe en la base de datos. \n";
        //            cartesianoTemporal = productoCartesiano(cartesianoTemporal, new List<tupla>(), ntab);
        //        }
        //        else
        //        {
        //            cartesianoTemporal = productoCartesiano(cartesianoTemporal, buscarTabla(ntab), ntab);
        //        }

        //    }
        //    /*Verificamos las condiciones para filtrar resultados*/
        //    foreach (tupla tp in cartesianoTemporal)
        //    {
        //        if (comprobarCondicion(tp, raizComprobacion))
        //        {
        //            cartesiano.Add(tp);
        //        }
        //    }

        //    /*Elegimos las celdas que se solicitan. Las demás se descartan*/
        //    /*Tambien se ordena los resultados según el usuario haya indicado*/
        //    cartesiano = filtrarResultados(listaCampos, cartesiano);

        //    /**/
        //    cartesiano = ordenarResultados(cartesiano, campoOrdenacion, orden);

        //    #region imprimir resultado
        //    if (cartesiano.Count > 0)
        //    {
        //        foreach (campo cp in cartesiano[0].campos)
        //        {
        //            if (data.Equals("\n"))
        //            {
        //                data = cp.id;
        //            }
        //            else
        //            {
        //                data = data + "," + cp.id;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return data + "\n Sin resultados";
        //    }
        //    data = data + "\n";
        //    foreach (tupla tpm in cartesiano)
        //    {
        //        bool flag = false;
        //        foreach (campo cmp in tpm.campos)
        //        {
        //            if (!flag)
        //            {
        //                data = data + cmp.valor.ToString();
        //                flag = true;
        //            }
        //            else
        //            {
        //                data = data + "," + cmp.valor.ToString();
        //            }
        //        }
        //        data = data + "\n";
        //    }
        //    #endregion

        //    return data;
        //}
        #endregion
    }

}
