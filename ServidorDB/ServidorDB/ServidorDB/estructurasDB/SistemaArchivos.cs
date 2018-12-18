using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServidorDB.EjecucionUsql;
using Irony.Parsing;
using ServidorBDD.EjecucionUsql;

namespace ServidorDB.estructurasDB
{
    public class SistemaArchivos
    {
        public List<BD> basesdedatos;
        public List<Usuario> usuarios;
        public String baseActual = "";
        public String usuarioActual = "";

        public SistemaArchivos()
        {
            this.basesdedatos = new List<BD>();
            this.usuarios = new List<Usuario>();
        }

        public void addBD(BD db)
        {
            basesdedatos.Add(db);
        }
        public void addUsuario(Usuario user)
        {
            usuarios.Add(user);
        }


        public void realizarConsulta(ParseTreeNode raiz)
        {
            if (getBase() != null)
            {
                getBase().seleccionar(raiz);
            }
        }

        public void setBaseActual(ParseTreeNode nodo)
        {
            String nombreDB = nodo.Token.Text;
            foreach (BD boss in basesdedatos)
            {
                if (boss.nombre.ToLower().Equals(nombreDB))
                {
                    Form1.Mensajes.Add("Base de datos --" + nombreDB + "-- seleccionada.");
                    this.baseActual = nombreDB;
                    return;
                }
            }
            //Form1.errores.Add(new Error("Semantico", "La base de datos " + nombreDB + " no existe en el sistema." , nodo.Token.Location.Line, nodo.Token.Location.Column));
            Form1.Mensajes.Add(new Error("Semantico", "La base de datos " + nombreDB + " no existe en el sistema.", nodo.Token.Location.Line, nodo.Token.Location.Column).getMensaje());
        }
        public BD getBase()
        {
            foreach (BD db in basesdedatos)
            {
                if (db.nombre.Equals(baseActual))
                {
                    return db;
                }
            }
            Form1.Mensajes.Add("Error :No se ha elegido la base de datos.");
            return null;
        }



        #region INSERT
        public void insertar(ParseTreeNode raiz)
        {
            if(getBase()==null)
            {
                Error error = new Error("Ejecucion","No se ha seleccionado alguna base de datos.",0,0);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
                return;
            }
            BD baseActual = getBase(); // Obtenemos la base actual
            String idTabla = raiz.ChildNodes[0].Token.Text; // ID de la tabla desde el arbol :v
            int linea = raiz.ChildNodes[0].Token.Location.Line;
            int columna = raiz.ChildNodes[0].Token.Location.Column;
            //Si no existe la base salimos
            if (!baseActual.existeTabla(idTabla))
            {
                Error error = new Error("Semantico",
                        "La tabla " + idTabla + " no existe en la base de datos",
                        raiz.ChildNodes[0].Token.Location.Line,
                        raiz.ChildNodes[0].Token.Location.Column);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
                return;
            }            
            switch (raiz.ChildNodes.Count)
            {
                case 2: // 0 idbase, 1  lista valores           
                    insertar(idTabla, raiz.ChildNodes[1],linea, columna);
                    break;
                case 3: // 0 idbase, 1. Lista id campos, 2. lista valores
                    insertar(idTabla, raiz.ChildNodes[1], raiz.ChildNodes[2]);
                    break;               
            }            
        }

        //Metodo insertar cuando no hay una lista de campos
        public void insertar(String nombreTabla, ParseTreeNode raizValores, int linea, int columna)
        {
            tupla nuevaTupla = new tupla();
            foreach (ParseTreeNode nodo in raizValores.ChildNodes)
            {                
                 Logica opL = new Logica();
                 Resultado result = opL.operar(nodo);
                nuevaTupla.addCampo(new campo("", result.valor,result.tipo));
            }
            int contador = 0;
            Tabla tabActual= getTabla(nombreTabla, linea, columna);
            List<defCampo> definiciones = tabActual.definiciones;
            #region Verificamos que el número de valores coinicida con 
            if (raizValores.ChildNodes.Count != definiciones.Count)
            {
                Error error = new Error("Semantico","El número de campos no coincide, se esperaban " + definiciones.Count + " campos y se han ingresado " + raizValores.ChildNodes.Count
                    , raizValores.ChildNodes[0].Span.Location.Line, raizValores.ChildNodes[0].Span.Location.Column);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
                return;
            }
            #endregion



            bool flag = true;  // Si es true se guarda, si es false no se guarda.
            for (contador = 0; contador<definiciones.Count; contador++ )
            {
                if (nuevaTupla.campos[contador].tipo.ToLower().Equals(definiciones[contador].tipo.ToLower())
                   || (nuevaTupla.campos[contador].tipo.ToLower().Equals("integer") && definiciones[contador].tipo.ToLower().Equals("bool")))
                {

                    /*Codigo para ver si los datos cumplen con la definicion*/
                    nuevaTupla.campos[contador].id = definiciones[contador].nombre; // Nombre del campo
                    #region Verificamos si es llave primaria
                    if (definiciones[contador].primaria)
                    {
                        // Verificamos que no exista una igual.
                        foreach(tupla tup in tabActual.tuplas)
                        {
                            campo tmpcampo = tup.getCampo(definiciones[contador].nombre);
                            if (tmpcampo.valor.ToString().Equals(nuevaTupla.campos[contador].valor.ToString()))
                            {
                                flag = false;
                                Error error = new Error("Ejecución", "Condicion de unico (Llave primaria) fallada " +nombreTabla +"."+ nuevaTupla.campos[contador].id,
                                    raizValores.ChildNodes[contador].Span.Location.Line, raizValores.ChildNodes[contador].Span.Location.Column);
                                Form1.errores.Add(error);
                                Form1.Mensajes.Add(error.getMensaje());
                            }
                        }
                    }
                    #endregion
                    #region Verificamos si es Unico
                    if (definiciones[contador].unico)
                    {
                        // Verificamos que no exista una igual.
                        foreach (tupla tup in tabActual.tuplas)
                        {
                            if (tup.getCampo(definiciones[contador].nombre).valor == nuevaTupla.campos[contador].valor)
                            {
                                flag = false;
                                Error error = new Error("Ejecución", "Condicion de unico (Unico) fallada " + nombreTabla + "." + nuevaTupla.campos[contador].id,
                                    raizValores.ChildNodes[contador].Span.Location.Line, raizValores.ChildNodes[contador].Span.Location.Column);
                                Form1.errores.Add(error);
                                Form1.Mensajes.Add(error.getMensaje());
                            }
                        }
                    }
                    #endregion
                    #region Verificamos que exista la llave Foranea
                    if(!definiciones[contador].foranea.Equals(""))
                    {
                        if (!existeTupla(definiciones[contador].foranea, nuevaTupla.campos[contador].id, nuevaTupla.campos[contador].valor, linea, columna))
                        {
                            flag = false;
                            Error error = new Error("Semantico", "Error, no se encuentra la llave foranea " + definiciones[contador].foranea, raizValores.ChildNodes[contador].Span.Location.Line, raizValores.ChildNodes[contador].Span.Location.Column);
                            Form1.errores.Add(error);
                            Form1.Mensajes.Add(error.getMensaje());
                        }
                    }
                    #endregion
                }
                else
                {
                    flag = false;
                    Error error = new Error("Semantico",
                        "Se esperaba un dato de tipo "+ definiciones[contador].tipo + ", valor " + nuevaTupla.campos[contador].valor +" inválido",
                        raizValores.ChildNodes[contador].Span.Location.Line, raizValores.ChildNodes[contador].Span.Location.Column);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                } 
            }
            if(flag)
            {
                getTabla(nombreTabla,linea,columna).tuplas.Add(nuevaTupla);
            }
            // Ahora ya tenemos la tupla nueva :v
        }
        //Metodo insertar cuando hay una lista de campos
        public void insertar(String nombreTabla, ParseTreeNode raizCampos, ParseTreeNode raizValores)
        {

        }

        //Verifica si existe el registro (para verificar la llave foranea
        public bool existeTupla(String nombreTabla, String nombreCampo, object valorPrimaria,  int linea, int columna)
        {
            String[] partes = nombreTabla.Split('.');
            if (partes.Length==2) { nombreTabla = partes[0]; nombreCampo = partes[1]; }
            
            Tabla tab = getTabla(nombreTabla,linea,columna);
            if (tab!=null)
            {
                foreach(tupla tp in tab.tuplas)
                {

                    if (tp.getCampo(nombreCampo.ToLower()).valor.ToString().Equals(valorPrimaria.ToString().ToLower()))
                    {
                        return true;
                    }
                    else
                    {
                        int n = 3;
                    }
                }
            }
            return false;
        }


        #endregion

        #region Codigo para guardar en disco todas las chivas.
        public void commit()
        {
            String cadenaMaestro = "";
            foreach (BD boss in basesdedatos)
            {
                /*
                <DB>
                <nombre>vehiculos</nombre>
                <path>C:\DB\BD\vehiculos\DB.xml</path>
                </DB>                          
                */
                cadenaMaestro = cadenaMaestro + "<db>\n";
                cadenaMaestro = cadenaMaestro + "\t<nombre>" + boss.nombre + "</nombre>\n";
                cadenaMaestro = cadenaMaestro + "\t<path>" + boss.path + "</path>\n";                
                cadenaMaestro = cadenaMaestro + "</db>\n";

                //Guardamos las tablas.
                foreach (Tabla tab in boss.tablas)
                {
                    commitTabla(tab);
                }
                // Ahora guardamos los objetos.
                commitObjetos(boss.pathObjetos, boss.objetos);
                // Ahora guardamos los procemineots.
                commitProcedimientos(boss.pathProcedimientos, boss.procedimientos);

                // Ahora obtenemos la data de las definiciones de las tablas.
                commitDefinicion(boss);            
            }
            //Guardamos el maestro.
            guardarArchivo("C:\\DB\\maestro.xml",cadenaMaestro);

            /*Ahora guardamos los usuarios*/
            commitUsuarios("C:\\DB\\usuarios.xml");
        }
        public void commitUsuarios(String path)
        {
            String cadenaUsuarios = "";
            foreach (Usuario user in Form1.sistemaArchivos.usuarios)
            {
                cadenaUsuarios = cadenaUsuarios + "<usuario>\n";
                cadenaUsuarios = cadenaUsuarios + "<nombre>" + user.username + "</nombre>\n";
                cadenaUsuarios = cadenaUsuarios + "<password>" + user.password + "</password>\n";
                foreach (Permiso permiso in user.permisos)
                {
                    if (permiso.listaObjetos.Count>0)
                    {
                        cadenaUsuarios = cadenaUsuarios + "<base>\n";
                        cadenaUsuarios = cadenaUsuarios + "<nombredb>" + permiso.nombreDB + "</nombredb>\n";
                        cadenaUsuarios = cadenaUsuarios + "\t<objetos>\n";
                        foreach (String nameObjeto in permiso.listaObjetos)
                        {
                            cadenaUsuarios = cadenaUsuarios + "<objeto>" + nameObjeto + "</objeto>\n";
                        }
                        cadenaUsuarios = cadenaUsuarios + "\t</objetos>\n";
                        cadenaUsuarios = cadenaUsuarios + "</base>\n";
                    }
                }
                cadenaUsuarios = cadenaUsuarios + "</usuario>\n";
            }
            guardarArchivo(path, cadenaUsuarios);
        }

        public void commitDefinicion(BD baseActual)
        {
            String definiciones = "";
            String definicionesTablas = getDeficiones(baseActual.tablas);
            definiciones = definiciones + "<procedure>\n";
            definiciones = definiciones + "\t<path>" +baseActual.pathProcedimientos +"</path>\n";
            definiciones = definiciones + "</procedure>\n";
            definiciones = definiciones + "<object>\n";
            definiciones = definiciones + "\t<path>" + baseActual.pathObjetos + "</path>\n";
            definiciones = definiciones + "</object>\n";
            definiciones = definiciones + definicionesTablas;
            guardarArchivo(baseActual.path, definiciones);
        }

        public String getDeficiones(List<Tabla> listaTablas)
        {
            String cadena = "";
            foreach (Tabla tab in listaTablas)
            {
                cadena = cadena + "<tabla>\n";
                cadena = cadena + "<nombre>"+tab.nombre+"</nombre>\n";
                cadena = cadena + "<path>" + tab.path+ "</path>\n";
                cadena = cadena + "<rows>\n";                
                foreach (defCampo def in tab.definiciones)
                {
                    int autoinc = 0;
                    int nulo = 0;
                    int prim = 0;
                    int unico = 0;
                    String kforanea = def.foranea;
                    if (def.auto) { autoinc = 1; } else { autoinc = 0; }
                    if (def.nulo) { nulo = 1; } else { nulo = 0; }
                    if (def.primaria) { prim = 1; } else { prim = 0; }
                    if (kforanea.Equals("")) { kforanea = "0"; }
                    if (def.unico) { unico = 1; } else { unico = 0; }
                    cadena = cadena + "<campo>\n";
                    cadena = cadena + "\t<" + def.tipo+">" + def.nombre + "</" + def.tipo + ">\n";
                    cadena = cadena + "<propiedades>\n";                    
                    cadena = cadena + "\t<autoincrementable>" + autoinc + "</autoincrementable>\n";
                    cadena = cadena + "\t<nulo>" + nulo + "</nulo>\n";
                    cadena = cadena + "\t<primaria>" + prim + "</primaria>\n";
                    cadena = cadena + "\t<foranea>" + kforanea + "</foranea>\n";                    
                    cadena = cadena + "\t<unico>" + unico + "</unico>\n";
                    cadena = cadena + "</propiedades>\n";
                    cadena = cadena + "</campo>\n";
                }
                cadena = cadena + "</rows>\n";
                cadena = cadena + "</tabla>\n";
            }
            return cadena;
        }

        public void commitProcedimientos(String path, List<Procedimiento> lista)
        {
            String cadena = "";
            foreach (Procedimiento proc in lista)
            {
                cadena = cadena + "\n<proc>\n";
                cadena = cadena + "<nombre>" + proc.nombre + "</nombre>\n";
                cadena = cadena + "<params>\n";
                foreach (Parametro par in proc.listaParametros)
                {
                    cadena = cadena + "<" + par.tipo + ">" + par.nombre
                        + "</" + par.tipo + ">\n";
                }
                cadena = cadena + "</params>";
                cadena = cadena + "<src>" + proc.codigoFuente + "</src>\n";
                if (!proc.tipoRetorno.Equals(""))
                {
                    cadena = cadena + "<tipo>" + proc.tipoRetorno + "</tipo>";
                }
                cadena = cadena + "\n</proc>\n";
            }
            guardarArchivo(path, cadena);
        }

        public void commitObjetos(String path, List<Objeto> lista)
        {
            String cadena = "";
            foreach (Objeto objt in lista)
            {
                cadena = cadena + "\n<obj>\n";
                cadena = cadena + "\t<nombre>" + objt.nombre + "</nombre>\n";
                cadena = cadena + "\t<attr>\n";
                foreach (Atributo atrib in objt.atributos)
                {
                    cadena = cadena + "<" + atrib.tipo + ">" + atrib.id
                        + "</" + atrib.tipo + ">\n" ;
                }
                cadena = cadena + "\t</attr>\n";
                cadena = cadena + "\t</obj>\n";
            }
            guardarArchivo(path, cadena);
        }

        public void commitTabla(Tabla tab)
        {
            String cadena = "";
            foreach (tupla tp in tab.tuplas)
            {
                cadena = cadena + "\n" + "<" + "row" + ">";
                //("MM/dd/yyyy HH:mm:ss")
                foreach (campo cmp in tp.campos)
                {
                    if (cmp.valor is DateTime)
                    {
                        DateTime fecha = DateTime.Parse(cmp.valor.ToString());
                        cadena = cadena + "<" + quitarNombreTabla(cmp.id.ToLower())  + ">" + fecha.ToString("dd-MM-yyyy HH:mm:ss")
                            + "</" + quitarNombreTabla(cmp.id.ToLower()) + ">\n";
                    }
                    else if (cmp.valor is String)
                    {
                        cadena = cadena + "<" + quitarNombreTabla(cmp.id.ToLower()) + ">\"" + cmp.valor.ToString()
                            + "\"</" + quitarNombreTabla(cmp.id.ToLower()) + ">\n";
                    }
                    else
                    {
                        cadena = cadena + "<" + quitarNombreTabla(cmp.id.ToLower()) + ">" + cmp.valor.ToString()
                        + "</" + quitarNombreTabla(cmp.id.ToLower()) + ">\n";
                    }

                }
                cadena = cadena + "\n" + "</" + "row" + ">";
            }
            guardarArchivo(tab.path, cadena);
            Form1.Mensajes.Add(cadena);
        }
        public String quitarNombreTabla(String tipo)
        {
            if (tipo.Contains("."))
            {
                String[] partes = tipo.Split('.');
                return partes[1];
            }
            else
            {
                return tipo;
            }
        }

        public Tabla getTabla(String nombreTabla, int linea, int columna)
        {
            if (getBase() != null)
            {
                foreach (Tabla tab in getBase().tablas)
                {
                    if (tab.nombre.ToLower().Equals(nombreTabla.ToLower()))
                    {
                        return tab;
                    }
                }
                Error error = new Error("Ejecucion", "La tabla " + nombreTabla + " no existe en la base de datos " + baseActual, linea, columna);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
            else
            {
                Error error = new Error("Ejecucion", "Se debe seleccionar una base de datos", linea, columna);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
            return null;
        }

        public void guardarArchivo(String path, String contenido)
        {                                   
            System.IO.File.WriteAllText(path, contenido); // Almacenamos el archivo
        }
        #endregion

    }
}
