using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
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
        //public Ejecucion(ParseTreeNode raiz)
        //{
        //    this.raiz = raiz;
        //}
        public void cargarBaseDatos(BD baseActual)
        {
            ParseTreeNode inicio = raiz.ChildNodes[0];
            ParseTreeNode aux;
            if (inicio.ChildNodes.Count == 3)
            {
                aux = inicio.ChildNodes[0]; //  Procedimientos . Tomar el segundo hijo que trae el path
                baseActual.procedimientos = cargarProcedimientos(aux.ChildNodes[1].Token.Text);
                aux = inicio.ChildNodes[1]; // Objetos. Tomar el segundo hijo que trae el path del archivo.
                baseActual.objetos = cargarObjetos(aux.ChildNodes[1].Token.Text);
                aux = inicio.ChildNodes[2]; // Tablas. 
                baseActual.tablas = cargarTablas(aux);
                /*Cargar tuplas para cada tabla*/
                foreach (Tabla tab in baseActual.tablas)
                {
                    Form1.Mensajes.Add(tab.path);
                    cargarTuplas(tab);
                }
                
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
            raiz = raiz.ChildNodes[0];            
            /*Recorremos la lista de tuplas/registros / :v*/
            foreach (ParseTreeNode nodoTupla in raiz.ChildNodes)
            {
                tupla newtupla = new tupla();
                /*Reocorremos la lista de atributos*/
                foreach (ParseTreeNode nodoCampo in nodoTupla.ChildNodes)
                {
                    campo cmp = new campo(nodoCampo.ChildNodes[0].Token.Text,
                            nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text);
                    if (nodoCampo.ChildNodes[0].Token.Text.ToLower().Equals(nodoCampo.ChildNodes[2].Token.Text.ToLower()))
                    {
                        if (nodoCampo.ChildNodes[1].ChildNodes.Count == 2) // Si tiene dos nodos el nodo uno es la fecha y el segundo es la hora. Concatenar
                        {
                            cmp.valor = DateTime.Parse(nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text
                                +" "+ nodoCampo.ChildNodes[1].ChildNodes[1].Token.Text
                                );
                        }
                        else
                        {
                            switch (nodoCampo.ChildNodes[1].ChildNodes[0].Term.Name.ToLower())
                            {
                                case "integer":
                                    cmp.valor = Int32.Parse(nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text);
                                    break;
                                case "cadena_literal":
                                    cmp.valor = nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text;
                                    break;
                                case "date":                                
                                    cmp.valor = DateTime.Parse(nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text);
                                    break;
                                case "double":
                                    cmp.valor = Double.Parse(nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text);
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
        public List<Tabla> cargarTablas(ParseTreeNode raiz)
        {
            List<Tabla> listaTablas = new List<Tabla>();
            foreach (ParseTreeNode nodo in raiz.ChildNodes)
            {
                Tabla nuevaTabla = new Tabla(nodo.ChildNodes[1].Token.Text, nodo.ChildNodes[4].Token.Text);                
                /*Ahora recorremos la lista de definicion de campos*/
                foreach (ParseTreeNode nodoDef in nodo.ChildNodes[6].ChildNodes)
                {
                    if (nodoDef.ChildNodes[0].Token.Text.ToLower().Equals(nodoDef.ChildNodes[2].Token.Text.ToLower()))
                    {
                        String nombre = nodoDef.ChildNodes[1].Token.Text;
                        String tipo = nodoDef.ChildNodes[0].Token.Text;                        
                        bool auto;
                        bool nulo;
                        bool primaria;
                        String foranea;
                        ParseTreeNode nodoAtributo = nodoDef.ChildNodes[3];
                        #region Autoincremento
                        if (nodoAtributo.ChildNodes[0].ChildNodes[0].ChildNodes[0].Token.Text.Equals("1"))
                        {
                            auto = true;
                        }
                        else
                        {
                            auto = false;
                        }
                        #endregion
                        #region Nulo
                        if (nodoAtributo.ChildNodes[1].ChildNodes[0].ChildNodes[0].Token.Text.Equals("1"))
                        {
                            nulo = true;
                        }
                        else
                        {
                            nulo = false;
                        }
                        #endregion
                        #region Llave primaria
                        if (nodoAtributo.ChildNodes[2].ChildNodes[0].ChildNodes[0].Token.Text.Equals("1"))
                        {
                            primaria = true;
                        }
                        else
                        {
                            primaria = false;
                        }
                        #endregion
                        #region Llave foranea
                        if (nodoAtributo.ChildNodes[3].ChildNodes[0].ChildNodes[0].Token.Text.Equals("0"))
                        {
                            foranea = "";
                        }
                        else
                        {
                            foranea = nodoAtributo.ChildNodes[3].ChildNodes[0].ChildNodes[0].Token.Text;
                        }
                        #endregion
                        defCampo definicion = new defCampo(nombre,tipo,auto,nulo,primaria,foranea);
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
            foreach (ParseTreeNode nodo in raiz.ChildNodes[0].ChildNodes)
            {
                if (nodo.ChildNodes.Count == 6)
                {
                    BD baseNueva = new BD(nodo.ChildNodes[1].Token.Text, nodo.ChildNodes[4].Token.Text);
                    listaBases.Add(baseNueva);
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
                    Usuario usuario = new Usuario(nodo.ChildNodes[2].ChildNodes[0].Token.Text, nodo.ChildNodes[5].ChildNodes[0].Token.Text);                    
                    if (nodo.ChildNodes.Count == 9) // No tiene lista de permisos
                    {
                        ParseTreeNode raizPermisos = nodo.ChildNodes[7];
                        foreach (ParseTreeNode tmp in raizPermisos.ChildNodes)
                        {
                            Permiso perm = new Permiso(tmp.ChildNodes[0].Token.Text);
                            foreach (ParseTreeNode nodoObjeto in tmp.ChildNodes[1].ChildNodes)
                            {
                                perm.listaObjetos.Add(nodoObjeto.Token.Text);
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

            foreach (ParseTreeNode nodo in raiz.ChildNodes[0].ChildNodes)
            {
                Procedimiento nuevoProc = new Procedimiento("","");
                //= new Procedimiento()
                if (nodo.ChildNodes.Count == 7) // No tiene retorno
                {
                    nuevoProc = new Procedimiento(nodo.ChildNodes[2].ChildNodes[0].Token.Text, "");
                }
                else if(nodo.ChildNodes.Count == 10 ) // tiene retorno
                {
                    nuevoProc = new Procedimiento(nodo.ChildNodes[2].ChildNodes[0].Token.Text, nodo.ChildNodes[7].Token.Text);
                }
                nuevoProc.codigoFuente = nodo.ChildNodes[5].Token.Text;

                /*Ahora vamos a cargar los parametros*/
                foreach (ParseTreeNode nodoParametro in nodo.ChildNodes[4].ChildNodes)
                {
                    if (nodoParametro.ChildNodes[0].Token.Text.ToLower().Equals
                        (nodoParametro.ChildNodes[2].Token.Text.ToLower()))
                    {
                        Parametro parametro = new Parametro(nodoParametro.ChildNodes[0].Token.Text, nodoParametro.ChildNodes[1].ChildNodes[0].Token.Text);
                        nuevoProc.listaParametros.Add(parametro);
                    }
                    else
                    {
                        Form1.Mensajes.Add("Procedimiento " + nuevoProc.nombre +" Error en esta etiqueta. Linea:" +nodoParametro.ChildNodes[0].Token.Location.Line 
                            + " Columna:" + +nodoParametro.ChildNodes[0].Token.Location.Column);
                    }                    
                }
                listaProcedimientos.Add(nuevoProc);
            }
            return listaProcedimientos;
        }
        private List<Objeto> recorrerObjetos(ParseTreeNode raiz)
        {
            List<Objeto> listaObjetos = new List<Objeto>();

            foreach (ParseTreeNode nodo in raiz.ChildNodes[0].ChildNodes)
            {
                /*Obtenemos el tipo del objeto*/
                Objeto nuevoObjeto = new Objeto(nodo.ChildNodes[1].ChildNodes[0].Token.Text);

                /*Obtenemos la lista de atributos*/
                ParseTreeNode raizLista = nodo.ChildNodes[3].ChildNodes[0];
                foreach (ParseTreeNode nodoCampo in raizLista.ChildNodes)
                {
                    if (nodoCampo.ChildNodes[0].Token.Text.Equals
                        (nodoCampo.ChildNodes[2].Token.Text))
                    {
                        nuevoObjeto.atributos.Add(new Atributo(nodoCampo.ChildNodes[0].Token.Text, nodoCampo.ChildNodes[1].ChildNodes[0].Token.Text, null));
                    }
                    else
                    {
                        Form1.Mensajes.Add("Error en la etiquetas de declaracion de objeto en: linea " + nodoCampo.ChildNodes[0].Token.Location.Line
                            +" Columna "+ nodoCampo.ChildNodes[0].Token.Location.Column);
                    }
                }
                listaObjetos.Add(nuevoObjeto);
            }
            return listaObjetos;
        }
    }
}
