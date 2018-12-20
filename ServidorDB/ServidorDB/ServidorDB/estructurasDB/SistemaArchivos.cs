using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServidorDB.EjecucionUsql;
using Irony.Parsing;
using ServidorBDD.EjecucionUsql;
using System.IO;

namespace ServidorDB.estructurasDB
{
    public class SistemaArchivos
    {
        public List<BD> basesdedatos;
        public List<Usuario> usuarios;
        public String baseActual = "";
        public String usuarioActual = "admin";

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
            String nombreDB = nodo.Token.Text.ToLower();
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

        public BD getBase(String nombre)
        {
            foreach (BD db in basesdedatos)
            {
                if (db.nombre.ToLower().Equals(nombre.ToLower()))
                {
                    return db;
                }
            }
            //Form1.Mensajes.Add("");
            return null;
        }

        public bool getObjeto(String id)
        {
            if (getBase()!=null)
            {
                foreach (Objeto obj in getBase().objetos)
                {
                    if (obj.nombre.ToLower().Equals(id.ToLower()))
                    {
                        return true;
                    }
                }
            }
            return false;
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
                    insertar(idTabla, raiz.ChildNodes[1], raiz.ChildNodes[2], linea, columna);
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
                            if (tup.getCampo(definiciones[contador].nombre).tipo.Equals("text"))
                            {
                                if (tup.getCampo(definiciones[contador].nombre).valor.Equals(nuevaTupla.campos[contador].valor))
                                {
                                    flag = false;
                                    Error error = new Error("Ejecución", "Condicion de unico (Unico) fallada " + nombreTabla + "." + nuevaTupla.campos[contador].id,
                                        raizValores.ChildNodes[contador].Span.Location.Line, raizValores.ChildNodes[contador].Span.Location.Column);
                                    Form1.errores.Add(error);
                                    Form1.Mensajes.Add(error.getMensaje());
                                }
                            }
                            else
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
        public void insertar(String nombreTabla, ParseTreeNode raizCampos, ParseTreeNode raizValores, int linea, int columna)
        {            
            tupla nuevaTupla = new tupla(); // La nueva tupla a ingresar.
            int contador = 0;            
            Tabla tabActual = getTabla(nombreTabla, linea, columna);
            List<defCampo> definiciones = tabActual.definiciones;
            foreach (defCampo def in definiciones)
            {
                campo nuevoCampo =null;
                switch (def.tipo)
                {
                    case "text":
                        nuevoCampo = new campo(def.nombre, "", def.tipo);
                        nuevoCampo.tablaId = "nulo";                        
                        break;
                    case "integer":
                        nuevoCampo = new campo(def.nombre, 0, def.tipo);
                        nuevoCampo.tablaId = "nulo";
                        break;
                    case "bool":
                        nuevoCampo = new campo(def.nombre, 0, def.tipo);
                        nuevoCampo.tablaId = "nulo";                        
                        break;
                    case "date":
                        DateTime hoy = DateTime.Today;                        
                        nuevoCampo = new campo(def.nombre, hoy.ToString("dd-MM-yyyy"), def.tipo);
                        nuevoCampo.tablaId = "nulo";
                        break;
                    case "datetime":
                        hoy = DateTime.Today;
                        nuevoCampo = new campo(def.nombre, hoy.ToString("dd-MM-yyyy hh:mm:ss"), def.tipo);
                        nuevoCampo.tablaId = "nulo";
                        break;
                }
                nuevaTupla.addCampo(nuevoCampo);
            }

            // Setear los valores que trae la lista.
            List<String> listaEtiquetas = new List<String>();
            for(contador = 0; contador<raizValores.ChildNodes.Count; contador++)
            {
                Logica opL = new Logica();
                Resultado result = opL.operar(raizValores.ChildNodes[contador]);
                nuevaTupla.getCampo(raizCampos.ChildNodes[contador].ChildNodes[0].Token.Text).valor=result.valor;
                nuevaTupla.getCampo(raizCampos.ChildNodes[contador].ChildNodes[0].Token.Text).tablaId = ""; // Con esto sabremos que no es nulo.
                listaEtiquetas.Add(raizCampos.ChildNodes[contador].ChildNodes[0].Token.Text);
            }

            #region Verificamos que el número de valores coinicida con 
            //if (raizValores.ChildNodes.Count != definiciones.Count)
            //{
            //    Error error = new Error("Semantico", "El número de campos no coincide, se esperaban " + definiciones.Count + " campos y se han ingresado " + raizValores.ChildNodes.Count
            //        , raizValores.ChildNodes[0].Span.Location.Line, raizValores.ChildNodes[0].Span.Location.Column);
            //    Form1.errores.Add(error);
            //    Form1.Mensajes.Add(error.getMensaje());
            //    return;
            //}
            #endregion



            bool flag = true;  // Si es true se guarda, si es false no se guarda.
            for (contador = 0; contador < definiciones.Count; contador++)
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
                        foreach (tupla tup in tabActual.tuplas)
                        {
                            campo tmpcampo = tup.getCampo(definiciones[contador].nombre);
                            if (tmpcampo.valor.ToString().Equals(nuevaTupla.campos[contador].valor.ToString()))
                            {
                                flag = false;
                                int contador2 = 0;
                                foreach (String etiqueta in listaEtiquetas)
                                {
                                    if (!etiqueta.Equals(definiciones[contador].nombre))
                                    {
                                        contador2++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                Error error = new Error("Ejecución", "Condicion de unico (Llave primaria) fallada " + nombreTabla + "." + nuevaTupla.campos[contador].id,
                                    raizValores.ChildNodes[contador2].Span.Location.Line, raizValores.ChildNodes[contador2].Span.Location.Column);
                                Form1.errores.Add(error);
                                Form1.Mensajes.Add(error.getMensaje());
                            }
                        }
                    }
                    #endregion
                    #region Autoincremental
                    if (definiciones[contador].auto)
                    {
                        // Si es entero y no tiene valor en el campo, se busca el mayor
                        if (nuevaTupla.campos[contador].tipo.Equals("integer") && nuevaTupla.campos[contador].tablaId.Equals("nulo"))
                        {
                            long maximo = 0;
                            if (getTabla(nombreTabla, linea, columna) != null)
                            {
                                foreach (tupla tup in getTabla(nombreTabla, linea, columna).tuplas)
                                {
                                    long valor = Convert.ToInt64(tup.campos[contador].valor);
                                    if (valor > maximo)
                                    {
                                        maximo = valor;
                                    }
                                }
                            }
                            nuevaTupla.campos[contador].valor = maximo + 1;
                            nuevaTupla.campos[contador].tablaId = "";
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
                    if (!definiciones[contador].foranea.Equals(""))
                    {
                        if (!existeTupla(definiciones[contador].foranea, nuevaTupla.campos[contador].id, nuevaTupla.campos[contador].valor, linea, columna))
                        {
                            flag = false;
                            int contador2 = 0;
                            foreach (String etiqueta in listaEtiquetas)
                            {
                                if (!etiqueta.Equals(definiciones[contador].nombre))
                                {
                                    contador2++;
                                }
                                else
                                {
                                    break;
                                }                                
                            }
                            Error error = new Error("Semantico", "Error, no se encuentra la llave foranea " + definiciones[contador].foranea, 
                                raizValores.ChildNodes[contador2].Span.Location.Line, 
                                raizValores.ChildNodes[contador2].Span.Location.Column);
                            Form1.errores.Add(error);
                            Form1.Mensajes.Add(error.getMensaje());
                        }
                    }
                    #region Verificación de nulo
                    if (!definiciones[contador].nulo)
                    {
                        if(nuevaTupla.campos[contador].tablaId.ToLower().Equals("nulo"))
                        {
                            flag = false;
                            int contador2 = 0;
                            foreach (String etiqueta in listaEtiquetas)
                            {
                                if (!etiqueta.ToLower().Equals(definiciones[contador].nombre.ToLower()))
                                {
                                    contador2++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            Error error = new Error("Semantico", "El campo " + definiciones[contador].nombre + " es no nulo.",
                                raizValores.ChildNodes[contador2].Span.Location.Line,
                                raizValores.ChildNodes[contador2].Span.Location.Column);
                            Form1.errores.Add(error);
                            Form1.Mensajes.Add(error.getMensaje());
                        }
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    flag = false;
                    Error error = new Error("Semantico",
                        "Se esperaba un dato de tipo " + definiciones[contador].tipo + ", valor " + nuevaTupla.campos[contador].valor + " inválido",
                        raizValores.ChildNodes[contador].Span.Location.Line, raizValores.ChildNodes[contador].Span.Location.Column);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                }
            }
            if (flag)
            {
                getTabla(nombreTabla, linea, columna).tuplas.Add(nuevaTupla);
            }
            // Ahora ya tenemos la tupla nueva :v
        }

        //Verifica si existe el registro (para verificar la llave foranea
        public bool existeTupla(String nombreTabla, String nombreCampo, object valorPrimaria,  int linea, int columna)
        {
            String[] partes = nombreTabla.Split('.');
            if (partes.Length==2) { nombreTabla = partes[0]; nombreCampo = partes[1]; }
            
            Tabla tab = getTabla(nombreTabla,linea,columna);
            if (tab!=null)
            {
                foreach (tupla tp in tab.tuplas)
                {
                    if (tp.getCampo(nombreCampo.ToLower()) != null)
                    {
                        if (tp.getCampo(nombreCampo.ToLower()).valor.ToString().Equals(valorPrimaria.ToString().ToLower()))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        Error error = new Error("Semantico","El campo "+nombreCampo + "no existe en la tabla "+nombreTabla,linea, columna);
                        Form1.errores.Add(error);
                        Form1.Mensajes.Add(error.getMensaje());
                    }
                }
            }
            return false;
        }


        #endregion

        #region DELETE
        public void eliminar(ParseTreeNode raiz)
        {
            if (raiz.ChildNodes[0].ChildNodes.Count>0)
            {
                switch (raiz.ChildNodes[0].Term.Name.ToLower())
                {
                    case "ebase":
                        eliminarBase(raiz.ChildNodes[0].ChildNodes[1].Token.Text.ToLower(), raiz.ChildNodes[0].ChildNodes[1].Token.Location.Line, raiz.ChildNodes[0].ChildNodes[1].Token.Location.Column);
                        break;
                    case "etabla":
                        eliminarTabla(raiz.ChildNodes[0].ChildNodes[0].Token.Text.ToLower(), raiz.ChildNodes[0].ChildNodes[0].Token.Location.Line, raiz.ChildNodes[0].ChildNodes[0].Token.Location.Column);
                        break;
                    case "eobjeto":
                        eliminarObjeto(raiz.ChildNodes[0].ChildNodes[0].Token.Text.ToLower(), raiz.ChildNodes[0].ChildNodes[0].Token.Location.Line, raiz.ChildNodes[0].ChildNodes[0].Token.Location.Column);
                        break;
                    case "euser":
                        eliminarUsuario(raiz.ChildNodes[0].ChildNodes[1].Token.Text.ToLower(), raiz.ChildNodes[0].ChildNodes[1].Token.Location.Line, raiz.ChildNodes[0].ChildNodes[1].Token.Location.Column);
                        break;
                }
            }
        }

        public void eliminarUsuario(String nombreUsuario, int linea, int columna)
        {

            if (!nombreUsuario.ToLower().Equals("admin"))
            {
                if (usuarioActual.ToLower().Equals("admin"))
                {
                    foreach (Usuario user in this.usuarios)
                    {
                        if (user.username.ToLower().Equals(nombreUsuario.ToLower()))
                        {
                            this.usuarios.Remove(user);
                            Form1.Mensajes.Add("El usuario "+nombreUsuario + " ha sido eliminado exitosamente.");
                            break;
                        }
                    }
                    Error error = new Error("Semantico", "El usuario "+nombreUsuario + " no existe en el sistema.", linea, columna);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                }
                else
                {
                    Error error = new Error("Semantico", "Sólo el administrador puede eliminar usuarios.", linea, columna);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                }
            }
            else
            {
                Error error = new Error("Semantico", "No se puede eliminar el usuario administrador.", linea, columna);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
            commit();
        }

        public void eliminarBase(String nombreBase, int linea, int columna)
        {
            if (getBase(nombreBase) != null)
            {
                BD baseTemporal = getBase(nombreBase);
                //Eliminamos los archivos.
                eliminarDirectorio(baseTemporal.path);
                // Ahora la quitamos de memoria.
                for(int cont = 0; cont<basesdedatos.Count; cont++)
                {
                    if (basesdedatos[cont].nombre.ToLower().Equals(nombreBase))
                    {
                        basesdedatos.RemoveAt(cont);
                        Form1.Mensajes.Add("La base de datos " + nombreBase + " ha sido eliminada." );
                        break;
                    }
                }
                commit();
            }
            else
            {
                Error error = new Error("Semantico","La base de datos "+ nombreBase +" no existe en el sistema." , linea, columna);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
        }

        public void eliminarTabla(String nombreTabla, int linea, int columna)
        {
            if (getBase() != null)
            {
                Tabla tablaTemporal = getTabla(nombreTabla, linea, columna);
                if (tablaTemporal != null)
                {
                    //Eliminamos los archivos.
                    eliminarDirectorio(tablaTemporal.path);
                    // Ahora la quitamos de memoria.                                
                    for (int cont = 0; cont < getBase().tablas.Count; cont++)
                    {
                        if (getBase().tablas[cont].nombre.ToLower().Equals(nombreTabla))
                        {
                            getBase().tablas.RemoveAt(cont);
                            Form1.Mensajes.Add("La tabla" + nombreTabla + " ha sido eliminada de la base de datos " + getBase().nombre);
                            break;
                        }
                    }
                }              
                commit();
            }
            else
            {
                Error error = new Error("Semantico", "No se ha seleccionado una base de datos.", linea, columna);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
        }

        public void eliminarObjeto(String nombreObjeto, int linea, int columna)
        {
            if (getBase() != null)
            {
                BD baseTemporal = getBase();
                //Eliminamos los archivos.
                eliminarDirectorio(baseTemporal.pathObjetos);
                // Ahora la quitamos de memoria.   
                if (getObjeto(nombreObjeto))
                {
                    for (int cont = 0; cont < baseTemporal.objetos.Count; cont++)
                    {
                        if (baseTemporal.objetos[cont].nombre.ToLower().Equals(nombreObjeto))
                        {
                            getBase().objetos.RemoveAt(cont);
                            Form1.Mensajes.Add("El objeto" + nombreObjeto + " ha sido eliminado de la base de datos " + baseTemporal.nombre);
                            break;
                        }
                    }
                }
                else
                {
                    Error error = new Error("Semantico", "El objeto "+ nombreObjeto + " no existe en la base de datos.", linea, columna);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                }
                commit();
            }
            else
            {
                Error error = new Error("Semantico", "La base de datos " + nombreObjeto + " no existe en el sistema.", linea, columna);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
        }

        public void eliminarDirectorio(String path)
        {
            String[] partes = path.Split('\\');
            String directorio = "";
            for (int cont = 0; cont < partes.Length - 1; cont++)
            {
                if (directorio.Equals(""))
                {
                    directorio = partes[cont];
                }
                else
                {
                    directorio = directorio + "\\" + partes[cont];
                }
            }

            try
            {
                // Si existe el directorio.
                if (Directory.Exists(directorio))
                {
                    //System.IO.File.Delete(directorio,true);
                    Directory.Delete(directorio, true);
                    Form1.Mensajes.Add("El directorio fue eliminado con existo. " + Directory.GetCreationTime(path));
                    return;
                }                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally { }
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
                cadenaUsuarios = cadenaUsuarios + "<password>\"" + user.password + "\"</password>\n";
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
                    switch (cmp.tipo.ToLower())
                    {
                        case "datetime":
                            DateTime fecha = DateTime.Parse(cmp.valor.ToString());
                            cadena = cadena + "<" + quitarNombreTabla(cmp.id.ToLower()) + ">" + fecha.ToString("dd-MM-yyyy HH:mm:ss")
                                + "</" + quitarNombreTabla(cmp.id.ToLower()) + ">\n";
                            break;
                        case "date":
                            fecha = DateTime.Parse(cmp.valor.ToString());
                            cadena = cadena + "<" + quitarNombreTabla(cmp.id.ToLower()) + ">" + fecha.ToString("dd-MM-yyyy")
                                + "</" + quitarNombreTabla(cmp.id.ToLower()) + ">\n";
                            break;
                        case "text":                            
                            cadena = cadena + "<" + quitarNombreTabla(cmp.id.ToLower()) + ">\"" + cmp.valor
                                + "\"</" + quitarNombreTabla(cmp.id.ToLower()) + ">\n";
                            break;
                        default:
                            cadena = cadena + "<" + quitarNombreTabla(cmp.id.ToLower()) + ">" + cmp.valor
                            + "</" + quitarNombreTabla(cmp.id.ToLower()) + ">\n";
                            break;
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
            nombreTabla = nombreTabla.ToLower();
            if (getBase() != null)
            {
                foreach (Tabla tab in getBase().tablas)
                {
                    if (tab.nombre.ToLower().Equals(nombreTabla.ToLower()))
                    {
                        return tab;
                    }
                }
                Error error = new Error("Semantico", "La tabla " + nombreTabla + " no existe en la base de datos " + baseActual, linea, columna);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
            else
            {
                Error error = new Error("Semantico", "Se debe seleccionar una base de datos", linea, columna);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
            return null;
        }

        public bool existeTabla(String nombreTabla, int linea, int columna)
        {
            if (getBase() != null)
            {
                foreach (Tabla tab in getBase().tablas)
                {
                    if (tab.nombre.ToLower().Equals(nombreTabla.ToLower()))
                    {
                        return true;
                    }
                }

            }
            else
            {
                Error error = new Error("Ejecucion", "Se debe seleccionar una base de datos", linea, columna);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
            return false;
        }

        public void guardarArchivo(String path, String contenido)
        {                                   
            String[] partes = path.Split('\\');
            String directorio = "";
            for (int cont = 0; cont < partes.Length - 1 ; cont++)
            {
                if (directorio.Equals(""))
                {
                    directorio = partes[cont];
                }
                else
                {
                    directorio = directorio +"\\" +partes[cont];
                }
            }

            try
            {
                // Si existe el directorio.
                if (Directory.Exists(directorio))
                {                   
                    System.IO.File.WriteAllText(path, contenido); // Almacenamos el archivo     
                    return;
                }

                // Crear la carpeta
                DirectoryInfo di = Directory.CreateDirectory(directorio);
                System.IO.File.WriteAllText(path, contenido); // Almacenamos el archivo  
                Form1.Mensajes.Add("El directorio fue creado con existo. " + Directory.GetCreationTime(path));                               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally { }
        }
        #endregion

        public void crearBase(ParseTreeNode raiz)
        {
            if (raiz.ChildNodes.Count == 2)
            {
                String nombreNuevaBase = raiz.ChildNodes[1].Token.Text.ToLower();
                bool flag = false; // Para saber si existe ya la base de datos.
                foreach (BD boss in basesdedatos)
                {
                    if (boss.nombre.ToLower().Equals(nombreNuevaBase))
                    {
                        flag = true;
                    }
                }

                if (!flag)
                {
                    String pathDB = "C:\\DB\\BD\\" + nombreNuevaBase + "\\DB.xml";
                    String pathObjetos = "C:\\DB\\BD\\" + nombreNuevaBase + "\\objetos.xml";
                    String pathProcedimientos = "C:\\DB\\BD\\" + nombreNuevaBase + "\\procedimientos.xml";
                    BD nuevaBase = new BD(nombreNuevaBase, pathDB);
                    nuevaBase.pathObjetos = pathObjetos;
                    nuevaBase.pathProcedimientos = pathProcedimientos;
                    basesdedatos.Add(nuevaBase);
                    Form1.Mensajes.Add("Base de datos "+nombreNuevaBase +" ha sido creada con éxito.");
                }
                else
                {
                    Error error = new Error("Semantico", "La base de datos "+ nombreNuevaBase + " ya existe en el sistema.", raiz.ChildNodes[1].Span.Location.Line, raiz.ChildNodes[1].Span.Location.Column);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                }

            }
        }

        public void crearTabla(ParseTreeNode raiz)
        {
            if (getBase()!=null)
            {
                String nombreNuevaTabla = raiz.ChildNodes[0].Token.Text;
                String pathBase = getBase().path;
                String[] partes = pathBase.Split('\\');
                String pathTabla = "";
                for (int cont = 0; cont < partes.Length - 1; cont++)
                {
                    if (pathTabla.Equals(""))
                    {
                        pathTabla = partes[cont];
                    }
                    else
                    {
                        pathTabla = pathTabla + "\\" + partes[cont];
                    }
                }
                String nombreTabla = raiz.ChildNodes[0].Token.Text;
                pathTabla = pathTabla + "\\" + nombreTabla +".xml";
                Tabla nuevaTabla = new Tabla(nombreTabla,pathTabla);
                // Verificamos si existe la tabla, si existe abortamos
                if (!existeTabla(nombreTabla, raiz.ChildNodes[0].Span.Location.Line, raiz.ChildNodes[0].Span.Location.Column))
                {
                    foreach (ParseTreeNode nodoCampo in raiz.ChildNodes[1].ChildNodes)
                    {
                        String nombre;
                        String tipo;
                        bool auto = false;
                        bool nulo = false;
                        bool primaria = false;
                        String foranea = "";
                        bool unico = false;
                        tipo = nodoCampo.ChildNodes[0].ChildNodes[0].Token.Text;
                        nombre = nodoCampo.ChildNodes[1].Token.Text;
                        foreach (ParseTreeNode nodoParametro in nodoCampo.ChildNodes[2].ChildNodes)
                        {
                            if (nodoParametro.ChildNodes.Count == 0)
                            {
                                switch (nodoParametro.Token.Text.ToLower())
                                {
                                    case "llave_primaria":
                                        primaria = true;
                                        break;
                                    case "autoincrementable":
                                        auto = true;
                                        break;
                                    case "nulo":
                                        nulo = true;
                                        break;
                                    case "no nulo":
                                        nulo = false;
                                        break;
                                    case "unico":
                                        unico = true;
                                        break;
                                }                                
                            }
                            else
                            {                                                                      
                                foranea = nodoParametro.ChildNodes[0].Token.Text;
                                foranea = foranea + "." + nodoParametro.ChildNodes[1].Token.Text;
                                break;
                            }
                        }
                        defCampo nuevaDefinicion = new defCampo(nombre, tipo, auto, nulo, primaria, foranea,unico);
                        nuevaTabla.definiciones.Add(nuevaDefinicion);
                    }
                    // Agregamos la nueva tabla.
                    getBase().tablas.Add(nuevaTabla);
                }
                else
                {
                    Error error = new Error("Semantico", "La tabla "+nombreTabla +" ya existe en el sistema", raiz.ChildNodes[0].Span.Location.Line, raiz.ChildNodes[0].Span.Location.Column);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                }
                // Recorremos la lista de definicion
    
            }
            else
            {
                Error error = new Error("Semantico", "No se ha elegido una base de datos.",raiz.ChildNodes[0].Span.Location.Line, raiz.ChildNodes[0].Span.Location.Column);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
        }

        public void crearUsuario(ParseTreeNode raiz)
        {
            if (raiz.ChildNodes.Count==4)
            {
                if (usuarioActual.ToLower().Equals("admin"))
                {
                    String username = raiz.ChildNodes[0].Token.Text;
                    String password = raiz.ChildNodes[3].Token.Text;
                    foreach (Usuario user in usuarios)
                    {
                        if (user.username.ToLower().Equals(username.ToLower()))
                        {
                            Error error = new Error("Semantico", "El usuario " + username + " ya existe en el sistema.", raiz.ChildNodes[0].Token.Location.Line, raiz.ChildNodes[0].Token.Location.Column);
                            Form1.errores.Add(error);
                            Form1.Mensajes.Add(error.getMensaje());
                            return;
                        }
                    }
                    Usuario newUser = new Usuario(username, password);                    
                    usuarios.Add(newUser);
                    Form1.Mensajes.Add("Nuevo usuario "+username +" registrado con éxito.");
                    commit();

                }
                else
                {
                    Error error = new Error("Semantico", "Solamente el usuario administrador puede registrar nuevos usuarios.", raiz.ChildNodes[0].Token.Location.Line, raiz.ChildNodes[0].Token.Location.Column);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                }
            }
        }

    }
}
