using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using ServidorDB.EjecucionUsql;
using ServidorDB.estructurasDB;

namespace ServidorDB.AnalizadorXML
{ 
    
    class Ejecucion
    {
        public Form1 formularioACtual;
        public ParseTreeNode raiz;
        public Ejecucion(ParseTreeNode raiz, Form1 formulario)
        {
            this.raiz = raiz;
            formularioACtual = formulario;
        }
        public void cargarBaseDatos(BD baseActual)
        {
            ParseTreeNode inicio = raiz.ChildNodes[0];
            ParseTreeNode aux;
            if (inicio.ChildNodes.Count == 3)
            {
                aux = inicio.ChildNodes[0]; //  Procedimientos . Tomar el segundo hijo que trae el path
                baseActual.procedimientos = cargarProcedimientos(aux.ChildNodes[1].Token.Text.Replace("\"",""));
                baseActual.pathProcedimientos = aux.ChildNodes[1].Token.Text.Replace("\"","");
                aux = inicio.ChildNodes[1]; // Objetos. Tomar el segundo hijo que trae el path del archivo.
                baseActual.objetos = cargarObjetos(aux.ChildNodes[1].Token.Text.Replace("\"",""));
                baseActual.pathObjetos = aux.ChildNodes[1].Token.Text.Replace("\"","");
                aux = inicio.ChildNodes[2]; // Tablas. 
                baseActual.tablas = cargarTablas(aux);
                /*Cargar tuplas para cada tabla*/
                foreach (Tabla tab in baseActual.tablas)
                {
                    Form1.Mensajes.Add(tab.path);
                    cargarTuplas(tab);
                }

            }
            else if (inicio.ChildNodes.Count == 2)
            {
                aux = inicio.ChildNodes[0]; //  Procedimientos . Tomar el segundo hijo que trae el path
                baseActual.procedimientos = cargarProcedimientos(aux.ChildNodes[1].Token.Text.Replace("\"",""));
                baseActual.pathProcedimientos = aux.ChildNodes[1].Token.Text.Replace("\"","");
                aux = inicio.ChildNodes[1]; // Objetos. Tomar el segundo hijo que trae el path del archivo.
                baseActual.objetos = cargarObjetos(aux.ChildNodes[1].Token.Text.Replace("\"",""));
                baseActual.pathObjetos = aux.ChildNodes[1].Token.Text.Replace("\"","");
            }
        }
        private void cargarTuplas(Tabla tabla)
        {
            String path = tabla.path;
            String contenidoArchivo = formularioACtual.getArchivo(path);
            AnalizadorXML.Analizador analizador = new AnalizadorXML.Analizador();
            AnalizadorXML.XMLGramatica gramatica = new AnalizadorXML.XMLGramatica();
            ParseTree arbol = analizador.generarArbol(contenidoArchivo, gramatica);
            ParseTreeNode raiz = arbol.Root;
            String errores = "";
            errores = formularioACtual.getErrores(arbol);
            if (errores.Equals(""))
            {
                Form1.Mensajes.Add("Archivo de registros " + path + " de datos cargado correctamente------------------------------------------------------------");
            }
            else
            {
                Form1.Mensajes.Add("El archivo de registros  " + path + " contiene errores. Cargado parcialmente------------------------------------------------------------");
                Form1.Mensajes.Add(errores);
            }
            if (raiz != null)
            {
                analizador.Genarbol(raiz);
                analizador.generateGraph("ejemplo.txt");
                ServidorDB.AnalizadorXML.Ejecucion ejecutor = new ServidorDB.AnalizadorXML.Ejecucion(raiz, formularioACtual);
                obtenerTuplas(raiz, tabla);
            }            
        }
        public void obtenerTuplas(ParseTreeNode raiz, Tabla tabla)
        {
            if (raiz.ChildNodes.Count>0)
            {
                raiz = raiz.ChildNodes[0];
                /*Recorremos la lista de tuplas/registros / :v*/
                foreach (ParseTreeNode nodoTupla in raiz.ChildNodes)
                {
                    tupla newtupla = new tupla();
                    /*Reocorremos la lista de atributos*/
                    foreach (ParseTreeNode nodoCampo in nodoTupla.ChildNodes)
                    {
                        campo cmp = new campo(nodoCampo.ChildNodes[0].Token.Text.Replace("\"",""),
                                nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text.Replace("\"",""));
                        if (nodoCampo.ChildNodes[0].Token.Text.Replace("\"","").ToLower().Equals(nodoCampo.ChildNodes[2].Token.Text.Replace("\"","").ToLower()))
                        {
                            if (nodoCampo.ChildNodes[1].ChildNodes.Count == 2) // Si tiene dos nodos el nodo uno es la fecha y el segundo es la hora. Concatenar
                            {
                                cmp.valor = DateTime.Parse(nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text.Replace("\"","")
                                    + " " + nodoCampo.ChildNodes[1].ChildNodes[1].Token.Text.Replace("\"","")
                                    );
                                cmp.tipo = "datetime";
                            }
                            else
                            {
                                switch (nodoCampo.ChildNodes[1].ChildNodes[0].Term.Name.ToLower())
                                {
                                    case "integer":
                                        cmp.valor = Int64.Parse(nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text.Replace("\"",""));
                                        cmp.tipo = "integer";
                                        break;
                                    case "cadena_literal":
                                        cmp.valor = nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text.Replace("\"","").Replace("\"", "");
                                        cmp.tipo = "text";
                                        break;
                                    case "date":
                                        cmp.valor = DateTime.Parse(nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text.Replace("\"",""));
                                        cmp.tipo = "date";
                                        break;
                                    case "double":
                                        cmp.valor = Double.Parse(nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text.Replace("\"",""));
                                        cmp.tipo = "double";
                                        break;
                                }
                            }
                            /*Comprobamos la integridad de los datos*/
                            newtupla.campos.Add(cmp);
                            //if (tabla.integridadCampo(cmp, formularioACtual))
                            //{
                            //    newtupla.campos.Add(cmp);
                            //}
                            //else
                            //{
                            //    Form1.Mensajes.Add("Error en tipos de datos en el campo");
                            //}
                        }
                        else
                        {
                            Form1.Mensajes.Add("Error en etiquetas del archivo " + tabla.path + " En linea:" + (nodoCampo.ChildNodes[0].Token.Location.Line + 1)
                                + " En columna:" + nodoCampo.ChildNodes[0].Token.Location.Column + " Registro no cargado.");
                            continue;
                        }
                    }
                    tabla.tuplas.Add(newtupla);
                }

            }
        }
        public List<Tabla> cargarTablas(ParseTreeNode raiz)
        {
            List<Tabla> listaTablas = new List<Tabla>();
            foreach (ParseTreeNode nodo in raiz.ChildNodes)
            {
                Tabla nuevaTabla = new Tabla(nodo.ChildNodes[1].Token.Text.Replace("\"",""), nodo.ChildNodes[4].Token.Text.Replace("\"",""));                
                /*Ahora recorremos la lista de definicion de campos*/
                foreach (ParseTreeNode nodoDef in nodo.ChildNodes[6].ChildNodes)
                {
                    if (nodoDef.ChildNodes[0].Token.Text.Replace("\"","").ToLower().Equals(nodoDef.ChildNodes[2].Token.Text.Replace("\"","").ToLower()))
                    {
                        String nombre = nodoDef.ChildNodes[1].Token.Text.Replace("\"","");
                        String tipo = nodoDef.ChildNodes[0].Token.Text.Replace("\"","");                        
                        bool auto;
                        bool nulo;
                        bool primaria;
                        bool unico;
                        String foranea;
                        ParseTreeNode nodoAtributo = nodoDef.ChildNodes[3];
                        if (nodoAtributo.ChildNodes.Count<5)
                        {
                            Error error = new Error("Sintactico", "Error en definicion de campos de tabla "+nuevaTabla.nombre +" , hace falta un parametro.", nodo.ChildNodes[1].Token.Location.Line, nodo.ChildNodes[1].Token.Location.Column);
                            Form1.errores.Add(error);
                            Form1.Mensajes.Add(error.getMensaje());
                            return listaTablas;
                        }

                        #region Autoincremento
                        if (nodoAtributo.ChildNodes[0].ChildNodes[0].ChildNodes[0].Token.Text.Replace("\"","").Equals("1"))
                        {
                            auto = true;
                        }
                        else
                        {
                            auto = false;
                        }
                        #endregion
                        #region Nulo
                        if (nodoAtributo.ChildNodes[1].ChildNodes[0].ChildNodes[0].Token.Text.Replace("\"","").Equals("1"))
                        {
                            nulo = true;
                        }
                        else
                        {
                            nulo = false;
                        }
                        #endregion
                        #region Llave primaria
                        if (nodoAtributo.ChildNodes[2].ChildNodes[0].ChildNodes[0].Token.Text.Replace("\"","").Equals("1"))
                        {
                            primaria = true;
                        }
                        else
                        {
                            primaria = false;
                        }
                        #endregion
                        #region Llave foranea
                        if (nodoAtributo.ChildNodes[3].ChildNodes[0].ChildNodes[0].Token.Text.Replace("\"","").Equals("0"))
                        {
                            foranea = "";
                        }
                        else
                        {
                            foranea = nodoAtributo.ChildNodes[3].ChildNodes[0].ChildNodes[0].Token.Text.Replace("\"","");
                        }
                        #endregion
                        #region unico
                        if (nodoAtributo.ChildNodes[4].ChildNodes[0].ChildNodes[0].Token.Text.Replace("\"","").Equals("0"))
                        {
                            unico = false;
                        }
                        else
                        {
                            unico = true;
                        }
                        #endregion
                        defCampo definicion = new defCampo(nombre,tipo,auto,nulo,primaria,foranea, unico);
                        nuevaTabla.definiciones.Add(definicion);
                    }
                    else
                    {
                        Form1.Mensajes.Add("Tabla " + nuevaTabla.nombre+ "Path:" + nuevaTabla.path+" Error de etiquetas que no coinciden. Linea:" + nodoDef.ChildNodes[0].Token.Location.Line
                            +" Columna:" + nodoDef.ChildNodes[0].Token.Location.Column);
                    }
                }
                listaTablas.Add(nuevaTabla);
            }
            return listaTablas;
        }
        public List<Objeto> cargarObjetos(String path)
        {
            List<Objeto> listaObjetos = new List<Objeto>();
            Form1.Mensajes.Add("Abriendo archivo de objetos " + path);
            listaObjetos = obtenerObjetos(path);
            return listaObjetos;
        }
        public List<Procedimiento> cargarProcedimientos(String path)/*Falta ver qué putas con este archivo*/
        {
            List<Procedimiento> listaProcedimientos = new List<Procedimiento>();
            Form1.Mensajes.Add("Abriendo archivo de procedimientos " + path);
            listaProcedimientos = obtenerProcedimientos(path);
            return listaProcedimientos;
        }
        public List<BD> recorrerArbolMaestro()
        {
            List<BD> listaBases = new List<BD>();
            if (raiz.ChildNodes.Count>0)// Verificamos si hay archivos
            {
                foreach (ParseTreeNode nodo in raiz.ChildNodes[0].ChildNodes)
                {
                    if (nodo.ChildNodes.Count == 6)
                    {
                        BD baseNueva = new BD(nodo.ChildNodes[1].Token.Text.Replace("\"",""), nodo.ChildNodes[4].Token.Text.Replace("\"",""));
                        listaBases.Add(baseNueva);
                    }
                }
            }
            return listaBases;
        }
        public List<Usuario> recorrerUsuarios()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            foreach (ParseTreeNode nodo in raiz.ChildNodes[0].ChildNodes)
            {
                if (nodo.ChildNodes.Count > 7) 
                {
                    Usuario usuario = new Usuario(nodo.ChildNodes[2].ChildNodes[0].Token.Text.Replace("\"",""), nodo.ChildNodes[5].ChildNodes[0].Token.Text.Replace("\"",""));                    
                    if (nodo.ChildNodes.Count == 9) // No tiene lista de permisos
                    {
                        ParseTreeNode raizPermisos = nodo.ChildNodes[7];
                        foreach (ParseTreeNode tmp in raizPermisos.ChildNodes)
                        {
                            Permiso perm = new Permiso(tmp.ChildNodes[0].Token.Text.Replace("\"",""));
                            foreach (ParseTreeNode nodoObjeto in tmp.ChildNodes[1].ChildNodes)
                            {
                                perm.listaObjetos.Add(nodoObjeto.Token.Text.Replace("\"",""));
                            }
                            usuario.permisos.Add(perm);
                        }
                    }
                    listaUsuarios.Add(usuario);
                } 
            }
            return listaUsuarios;
        }
        private List<Procedimiento> obtenerProcedimientos(String path)
        {
            List<Procedimiento> listaProcedimientos = new List<Procedimiento>();

            String contenidoArchivo = formularioACtual.getArchivo(path);
            AnalizadorXML.Analizador analizador = new AnalizadorXML.Analizador();
            AnalizadorXML.XMLGramatica gramatica = new AnalizadorXML.XMLGramatica();
            ParseTree arbol = analizador.generarArbol(contenidoArchivo, gramatica);
            ParseTreeNode raiz = arbol.Root;
            String errores = "";
            errores = formularioACtual.getErrores(arbol);
            if (errores.Equals(""))
            {
                Form1.Mensajes.Add("Archivo de Procedimientos " + path + " de datos cargado correctamente------------------------------------------------------------");
            }
            else
            {
                Form1.Mensajes.Add("El archivo de Procedimientos  " + path + " contiene errores. Cargado parcialmente------------------------------------------------------------");
                Form1.Mensajes.Add(errores);
            }
            if (raiz != null)
            {
                analizador.Genarbol(raiz);
                analizador.generateGraph("ejemplo.txt");
                ServidorDB.AnalizadorXML.Ejecucion ejecutor = new ServidorDB.AnalizadorXML.Ejecucion(raiz, formularioACtual);
                listaProcedimientos = recorrerProcedimiento(raiz);
            }
            return listaProcedimientos;
        }
        private List<Objeto> obtenerObjetos(String path)
        {
            List<Objeto> listaObjetos = new List<Objeto>();

            String contenidoArchivo = formularioACtual.getArchivo(path);            
            AnalizadorXML.Analizador analizador = new AnalizadorXML.Analizador();
            AnalizadorXML.XMLGramatica gramatica = new AnalizadorXML.XMLGramatica();
            ParseTree arbol = analizador.generarArbol(contenidoArchivo, gramatica);
            ParseTreeNode raiz = arbol.Root;
            String errores = "";
            errores = formularioACtual.getErrores(arbol);
            if (errores.Equals(""))
            {
                Form1.Mensajes.Add("Archivo de objetos " + path + " de datos cargado correctamente------------------------------------------------------------");
            }
            else
            {
                Form1.Mensajes.Add("El archivo de base de objetos  " + path + " contiene errores. Cargado parcialmente------------------------------------------------------------");
                Form1.Mensajes.Add(errores);
            }
            if (raiz != null)
            {
                analizador.Genarbol(raiz);
                analizador.generateGraph("ejemplo.txt");
                ServidorDB.AnalizadorXML.Ejecucion ejecutor = new ServidorDB.AnalizadorXML.Ejecucion(raiz, formularioACtual);
                listaObjetos = recorrerObjetos(raiz);
            }
            return listaObjetos;
        }
        private List<Procedimiento> recorrerProcedimiento(ParseTreeNode raiz)
        {
            List<Procedimiento> listaProcedimientos = new List<Procedimiento>();

            // Primero verificamos de que existan procedimientos
            if (raiz.ChildNodes.Count > 0)
            {
                foreach (ParseTreeNode nodo in raiz.ChildNodes[0].ChildNodes)
                {
                    Procedimiento nuevoProc = new Procedimiento("", "");
                    //= new Procedimiento()
                    if (nodo.ChildNodes.Count == 7) // No tiene retorno
                    {
                        nuevoProc = new Procedimiento(nodo.ChildNodes[2].ChildNodes[0].Token.Text.Replace("\"",""), "");
                    }
                    else if (nodo.ChildNodes.Count == 10) // tiene retorno
                    {
                        nuevoProc = new Procedimiento(nodo.ChildNodes[2].ChildNodes[0].Token.Text.Replace("\"",""), nodo.ChildNodes[7].Token.Text.Replace("\"",""));
                    }
                    nuevoProc.codigoFuente = nodo.ChildNodes[5].Token.Text.Replace("\"","");
                    /*Generamos el arbol de la funcion*/
                    

                    /*Ahora vamos a cargar los parametros*/
                    foreach (ParseTreeNode nodoParametro in nodo.ChildNodes[4].ChildNodes)
                    {
                        if (nodoParametro.ChildNodes[0].Token.Text.Replace("\"","").ToLower().Equals
                            (nodoParametro.ChildNodes[2].Token.Text.Replace("\"","").ToLower()))
                        {
                            Parametro parametro = new Parametro(nodoParametro.ChildNodes[0].Token.Text.Replace("\"",""), nodoParametro.ChildNodes[1].ChildNodes[0].Token.Text.Replace("\"",""));
                            nuevoProc.listaParametros.Add(parametro);
                        }
                        else
                        {
                            Form1.Mensajes.Add("Procedimiento " + nuevoProc.nombre + " Error en esta etiqueta. Linea:" + nodoParametro.ChildNodes[0].Token.Location.Line
                                + " Columna:" + +nodoParametro.ChildNodes[0].Token.Location.Column);
                        }
                    }
                    listaProcedimientos.Add(nuevoProc);
                }
            }
            return listaProcedimientos;
        }
        private List<Objeto> recorrerObjetos(ParseTreeNode raiz)
        {
            List<Objeto> listaObjetos = new List<Objeto>();

            if (raiz.ChildNodes.Count>0)
            {
                foreach (ParseTreeNode nodo in raiz.ChildNodes[0].ChildNodes)
                {
                    /*Obtenemos el tipo del objeto*/
                    Objeto nuevoObjeto = new Objeto(nodo.ChildNodes[1].ChildNodes[0].Token.Text.Replace("\"",""));

                    /*Obtenemos la lista de atributos*/
                    ParseTreeNode raizLista = nodo.ChildNodes[3].ChildNodes[0];
                    foreach (ParseTreeNode nodoCampo in raizLista.ChildNodes)
                    {
                        if (nodoCampo.ChildNodes[0].Token.Text.Replace("\"","").Equals
                            (nodoCampo.ChildNodes[2].Token.Text.Replace("\"","")))
                        {
                            nuevoObjeto.atributos.Add(new Atributo(nodoCampo.ChildNodes[0].Token.Text.Replace("\"","").ToLower(), nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text.Replace("\"","").ToLower(), null));
                        }
                        else
                        {
                            Form1.Mensajes.Add("Error en la etiquetas de declaracion de objeto en: linea " + nodoCampo.ChildNodes[0].Token.Location.Line
                                + " Columna " + nodoCampo.ChildNodes[0].Token.Location.Column);
                        }
                    }
                    listaObjetos.Add(nuevoObjeto);
                }
            }
            return listaObjetos;
        }
    }
}
