using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServidorDB.EjecucionUsql;
using Irony.Parsing;
using ServidorBDD.EjecucionUsql;
using System.IO;
using System.IO.Compression;

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

        public Resultado contar(ParseTreeNode raiz)
        {
            Resultado result = new Resultado("integer", 0);
            if (getBase() != null)
            {
                result.valor =getBase().Contar(raiz.ChildNodes[0]);
            }
            return result;
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

        public Boolean setMetodo(Procedimiento nuevo)
        {
            foreach (Procedimiento p in getBase().procedimientos)
            {
                if (nuevo.id.Equals(p.id))
                {
                    return false;
                }
            }
            getBase().procedimientos.Add(nuevo);
            return true;
        }

        public Procedimiento getMetodo(String id)
        {
            foreach (Procedimiento p in getBase().procedimientos)
            {
                if (id.Equals(p.id))
                {
                    return p;
                }
            }
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
            if (getBase() != null)
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
        #region PERMISOS
        public void permisos(ParseTreeNode raiz)
        {
            if (!usuarioActual.Equals("admin"))
            {
                Error error = new Error("Semantico", "Sólo el usuario admin puede realizar esta operacion."
                    , raiz.ChildNodes[0].Span.Location.Line, raiz.ChildNodes[0].Span.Location.Column);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
                return;
            }
            // Verificamos que exista el usuario
            String nombreUsuario = raiz.ChildNodes[1].Token.Text.ToLower();
            bool encontrado = false;
            Usuario usuarioAUtilizar = null;
            foreach (Usuario user in usuarios)
            {
                if (nombreUsuario.Equals(user.username.ToLower()))
                {
                    encontrado = true;
                    usuarioAUtilizar = user;
                }
            }
            if (!encontrado)
            {
                Error error = new Error("Semantico", "No existe el usuario " + nombreUsuario + " en el sistema."
                    , raiz.ChildNodes[1].Span.Location.Line, raiz.ChildNodes[1].Span.Location.Column);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
                return;
            }
            // Ahora obtenemos el nombre del objeto.
            String nombreBase = raiz.ChildNodes[2].Token.Text.ToLower();
            String nombreObjeto = raiz.ChildNodes[4].Token.Text.ToLower();
            switch (raiz.ChildNodes[0].Token.Text.ToLower())
            {
                case "otorgar":
                    otorgarPermisos(usuarioAUtilizar, nombreBase, nombreObjeto, raiz.ChildNodes[4].Span.Location.Line, raiz.ChildNodes[4].Span.Location.Column);
                    break;
                case "denegar":

                    denegarPermisos(usuarioAUtilizar, nombreBase, nombreObjeto, raiz.ChildNodes[4].Span.Location.Line, raiz.ChildNodes[4].Span.Location.Column);
                    break;
            }
        }
        public void otorgarPermisos(Usuario user, String basedatos, String objeto, int linea, int columna)
        {
            if (objeto.Equals("*")) { objeto = "all"; }
            if (user.permisos.Count == 0)
            {
                Permiso permiso = new Permiso(basedatos);
                permiso.nombreDB = basedatos;
                permiso.listaObjetos.Add(objeto);
                user.permisos.Add(permiso);
                Form1.Mensajes.Add("Se le han otorgado los permisos al usuario " + user.username + " sobre el objeto " + objeto + " de la base de datos " + basedatos);
            }
            else
            {
                foreach (Permiso permiso in user.permisos)
                {
                    if (permiso.nombreDB.ToLower().Equals(basedatos))
                    {
                        bool encontrado = false;
                        foreach (String etiqueta in permiso.listaObjetos)
                        {
                            if (etiqueta.ToLower().Equals(objeto))
                            {
                                encontrado = true;
                            }
                        }
                        if (encontrado) // Si ya existe el permiso se 
                        {
                            Error error = new Error("Semantico", "El usuario " + user.username + " ya tiene permisos sobre el objeto " + objeto + " de la base de datos " + basedatos,
                                linea, columna);
                            Form1.errores.Add(error);
                            Form1.Mensajes.Add(error.getMensaje());
                            return;
                        }
                        permiso.listaObjetos.Add(objeto);
                        Form1.Mensajes.Add("Se le han otorgado los permisos al usuario " + user.username + " sobre el objeto " + objeto + " de la base de datos " + basedatos);

                        return;
                    }
                }
            }

        }

        public void denegarPermisos(Usuario user, String basedatos, String objeto, int linea, int columna)
        {
            if (user.permisos.Count == 0)
            {
                Error error = new Error("Semantico", "El usuario " + user.username + " no posee permisos sobre el objeto " + objeto + " de la base de datos " + basedatos,
                    linea, columna);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
            else
            {

                for (int cont = 0; cont < user.permisos.Count; cont++)
                {
                    Permiso permiso = user.permisos[cont];
                    if (permiso.nombreDB.ToLower().Equals(basedatos))
                    {
                        if (objeto.Equals("*"))
                        {
                            user.permisos.RemoveAt(cont);
                            cont--;
                        }
                        else
                        {
                            bool encontrado = false;
                            foreach (String etiqueta in permiso.listaObjetos)
                            {
                                if (etiqueta.ToLower().Equals(objeto))
                                {
                                    encontrado = true;
                                }
                            }
                            if (!encontrado) // Si ya existe el permiso se 
                            {
                                Error error = new Error("Semantico", "El usuario " + user.username + " no posee permisos sobre el objeto " + objeto + " de la base de datos " + basedatos,
                                    linea, columna);
                                Form1.errores.Add(error);
                                Form1.Mensajes.Add(error.getMensaje());
                                return;
                            }
                            for (int x = 0; x < permiso.listaObjetos.Count; x++)
                            {
                                if (permiso.listaObjetos[x].ToLower().Equals(objeto.ToLower()))
                                {
                                    permiso.listaObjetos.RemoveAt(x);
                                    x--;
                                }
                            }
                            Form1.Mensajes.Add("Se le han denegado los permisos al usuario " + user.username + " sobre el objeto " + objeto + " de la base de datos " + basedatos);
                            return;
                        }
                    }
                }
            }
        }
        #endregion

        #region INSERT
        public void insertar(ParseTreeNode raiz)
        {
            if (getBase() == null)
            {
                Error error = new Error("Ejecucion", "No se ha seleccionado alguna base de datos.", 0, 0);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
                return;
            }
            BD baseActual = getBase(); // Obtenemos la base actual
            String idTabla = raiz.ChildNodes[0].Token.Text.Replace("\"", ""); // ID de la tabla desde el arbol :v
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
                    insertar(idTabla, raiz.ChildNodes[1], linea, columna);
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
                nuevaTupla.addCampo(new campo("", result.valor, result.tipo));
            }
            int contador = 0;
            Tabla tabActual = getTabla(nombreTabla, linea, columna);
            List<defCampo> definiciones = tabActual.definiciones;
            #region Verificamos que el número de valores coinicida con 
            if (raizValores.ChildNodes.Count != definiciones.Count)
            {
                Error error = new Error("Semantico", "El número de campos no coincide, se esperaban " + definiciones.Count + " campos y se han ingresado " + raizValores.ChildNodes.Count
                    , raizValores.ChildNodes[0].Span.Location.Line, raizValores.ChildNodes[0].Span.Location.Column);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
                return;
            }
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
                                Error error = new Error("Ejecución", "Condicion de unico (Llave primaria) fallada " + nombreTabla + "." + nuevaTupla.campos[contador].id,
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
                    if (!definiciones[contador].foranea.Equals(""))
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
        //Metodo insertar cuando hay una lista de campos
        public void insertar(String nombreTabla, ParseTreeNode raizCampos, ParseTreeNode raizValores, int linea, int columna)
        {
            tupla nuevaTupla = new tupla(); // La nueva tupla a ingresar.
            int contador = 0;
            Tabla tabActual = getTabla(nombreTabla, linea, columna);
            List<defCampo> definiciones = tabActual.definiciones;
            foreach (defCampo def in definiciones)
            {
                campo nuevoCampo = null;
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
            for (contador = 0; contador < raizValores.ChildNodes.Count; contador++)
            {
                Logica opL = new Logica();
                Resultado result = opL.operar(raizValores.ChildNodes[contador]);
                if (nuevaTupla.getCampo(raizCampos.ChildNodes[contador].ChildNodes[0].Token.Text)!=null)
                {
                    nuevaTupla.getCampo(raizCampos.ChildNodes[contador].ChildNodes[0].Token.Text).valor = result.valor;
                    nuevaTupla.getCampo(raizCampos.ChildNodes[contador].ChildNodes[0].Token.Text).tablaId = ""; // Con esto sabremos que no es nulo.
                    listaEtiquetas.Add(raizCampos.ChildNodes[contador].ChildNodes[0].Token.Text);
                }
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
                    if (definiciones[contador].nulo)
                    {
                        if (nuevaTupla.campos[contador].tablaId.ToLower().Equals("nulo"))
                        {
                            flag = false;                            
                            foreach (String etiqueta in listaEtiquetas)
                            {
                                if (!etiqueta.ToLower().Equals(definiciones[contador].nombre.ToLower()))
                                {

                                }
                                else
                                {
                                    break;
                                }
                            }

                            Error error = new Error("Semantico", "El campo " + definiciones[contador].nombre + " de la tabla " + nombreTabla+ " es no nulo.",
                                raizValores.ChildNodes[0].Span.Location.Line,
                                raizValores.ChildNodes[0].Span.Location.Column);
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
        public bool existeTupla(String nombreTabla, String nombreCampo, object valorPrimaria, int linea, int columna)
        {
            String[] partes = nombreTabla.Split('.');
            if (partes.Length == 2) { nombreTabla = partes[0]; nombreCampo = partes[1]; }

            Tabla tab = getTabla(nombreTabla, linea, columna);
            if (tab != null)
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
                        Error error = new Error("Semantico", "El campo " + nombreCampo + "no existe en la tabla " + nombreTabla, linea, columna);
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
            if (raiz.ChildNodes[0].ChildNodes.Count > 0)
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
                            Form1.Mensajes.Add("El usuario " + nombreUsuario + " ha sido eliminado exitosamente.");
                            break;
                        }
                    }
                    Error error = new Error("Semantico", "El usuario " + nombreUsuario + " no existe en el sistema.", linea, columna);
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
                for (int cont = 0; cont < basesdedatos.Count; cont++)
                {
                    if (basesdedatos[cont].nombre.ToLower().Equals(nombreBase))
                    {
                        basesdedatos.RemoveAt(cont);
                        Form1.Mensajes.Add("La base de datos " + nombreBase + " ha sido eliminada.");
                        break;
                    }
                }
                commit();
            }
            else
            {
                Error error = new Error("Semantico", "La base de datos " + nombreBase + " no existe en el sistema.", linea, columna);
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
                    Error error = new Error("Semantico", "El objeto " + nombreObjeto + " no existe en la base de datos.", linea, columna);
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
        public void eliminarArchivo(String path)
        {
            String directorio = path;
            try
            {
                // Si existe el directorio.
                if (File.Exists(directorio))
                {
                    //System.IO.File.Delete(directorio,true);
                    File.Delete(directorio);
                    Form1.Mensajes.Add("El archivo fue eliminado con exito. " + Directory.GetCreationTime(path));
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
            guardarArchivo("C:\\DB\\maestro.xml", cadenaMaestro);

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
                    if (permiso.listaObjetos.Count > 0)
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
            definiciones = definiciones + "\t<path>" + baseActual.pathProcedimientos + "</path>\n";
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
                cadena = cadena + "<nombre>" + tab.nombre + "</nombre>\n";
                cadena = cadena + "<path>" + tab.path + "</path>\n";
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
                    cadena = cadena + "\t<" + def.tipo + ">" + def.nombre + "</" + def.tipo + ">\n";
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
                cadena = cadena + "<nombre>" + proc.id.Replace("$",".") + "</nombre>\n";
                cadena = cadena + "<params>\n";
                foreach (Parametro par in proc.listaParametros)
                {
                    cadena = cadena + "<" + par.tipo + ">" + par.nombre
                        + "</" + par.tipo + ">\n";
                }
                cadena = cadena + "</params>";
                cadena = cadena + "<src>~" + proc.codigoFuente + "~</src>\n";
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
                        + "</" + atrib.tipo + ">\n";
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
                            if (cmp.valor.Equals(""))
                            {
                                cmp.valor.Equals("null");
                            }
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
            //Form1.Mensajes.Add(cadena);
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
                    Form1.Mensajes.Add("Base de datos " + nombreNuevaBase + " ha sido creada con éxito.");
                }
                else
                {
                    Error error = new Error("Semantico", "La base de datos " + nombreNuevaBase + " ya existe en el sistema.", raiz.ChildNodes[1].Span.Location.Line, raiz.ChildNodes[1].Span.Location.Column);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                }

            }
        }

        public void crearTabla(ParseTreeNode raiz)
        {
            if (getBase() != null)
            {
                String nombreNuevaTabla = raiz.ChildNodes[0].Token.Text.Replace("\"", "");
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
                String nombreTabla = raiz.ChildNodes[0].Token.Text.Replace("\"", "");
                pathTabla = pathTabla + "\\" + nombreTabla + ".xml";
                Tabla nuevaTabla = new Tabla(nombreTabla, pathTabla);
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
                        tipo = nodoCampo.ChildNodes[0].ChildNodes[0].Token.Text.Replace("\"", "");
                        nombre = nodoCampo.ChildNodes[1].Token.Text.Replace("\"", "");
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
                                foranea = nodoParametro.ChildNodes[0].Token.Text.Replace("\"", "");
                                foranea = foranea + "." + nodoParametro.ChildNodes[1].Token.Text.Replace("\"", "");
                                break;
                            }
                        }
                        defCampo nuevaDefinicion = new defCampo(nombre, tipo, auto, nulo, primaria, foranea, unico);
                        nuevaTabla.definiciones.Add(nuevaDefinicion);
                    }
                    // Agregamos la nueva tabla.
                    getBase().tablas.Add(nuevaTabla);
                }
                else
                {
                    Error error = new Error("Semantico", "La tabla " + nombreTabla + " ya existe en el sistema", raiz.ChildNodes[0].Span.Location.Line, raiz.ChildNodes[0].Span.Location.Column);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                }
                // Recorremos la lista de definicion

            }
            else
            {
                Error error = new Error("Semantico", "No se ha elegido una base de datos.", raiz.ChildNodes[0].Span.Location.Line, raiz.ChildNodes[0].Span.Location.Column);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
        }

        public void crearUsuario(ParseTreeNode raiz)
        {
            if (raiz.ChildNodes.Count == 4)
            {
                if (usuarioActual.ToLower().Equals("admin"))
                {
                    String username = raiz.ChildNodes[0].Token.Text.Replace("\"", "");
                    String password = raiz.ChildNodes[3].Token.Text.Replace("\"", "");
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
                    Form1.Mensajes.Add("Nuevo usuario " + username + " registrado con éxito.");
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
        #region BORRAR TUPLA
        public void borrar(ParseTreeNode raiz)
        {
            if (getBase() != null)
            {
                String nombreTabla = raiz.ChildNodes[0].Token.Text.ToLower();
                Tabla tabActual = getTabla(nombreTabla, raiz.ChildNodes[0].Token.Location.Line, raiz.ChildNodes[0].Token.Location.Column);
                if (tabActual != null)
                {
                    // Tenemos dos casos, con un sólo hijo : Borrar todo, con dos hijos borrar si se cumple la condicion
                    if (raiz.ChildNodes.Count == 1)
                    {
                        tabActual.tuplas = new List<tupla>();
                    }
                    else
                    {
                        ParseTreeNode condicion = raiz.ChildNodes[1];
                        for (int x = 0; x < tabActual.tuplas.Count; x++)
                        {
                            Logica opL = new Logica(tabActual.tuplas[x]);
                            Resultado result = opL.operar(condicion);
                            if (!result.tipo.ToLower().Equals("error"))
                            {
                                if (result.tipo.ToLower().Equals("bool") && result.valor.ToString().Equals("1")
                                    || result.tipo.ToLower().Equals("integer") && result.valor.ToString().Equals("1"))
                                {
                                    // Eliminamos la tupla
                                    tabActual.tuplas.RemoveAt(x);
                                    x--;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region ALTERAR USUARIO
        public void alterarUsuario(ParseTreeNode raiz)
        {
            if (!usuarioActual.Equals("admin"))// Si el usuario no es admin abortamos la operación
            {
                Error error = new Error("Semantico", "Sólo el usuario administrador puede realizar la operación de actualización de datos de usuarios.",
                    raiz.ChildNodes[0].Token.Location.Line, raiz.ChildNodes[0].Token.Location.Column);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
                return;
            }
            String nombreUsuario = raiz.ChildNodes[0].Token.Text.ToLower();
            String nuevoPassword = raiz.ChildNodes[3].Token.Text.Replace("\"", "");
            // Primero buscamos si existe el usuario;
            bool encontrado = false;
            foreach (Usuario user in usuarios)
            {
                if (user.username.ToLower().Equals(nombreUsuario))
                {
                    encontrado = true;
                    break;
                }
            }
            if (!encontrado) // No se encontró usuario, reporte y aborto de operación
            {
                Error error = new Error("Semantico", "El usuario " + nombreUsuario + " no existe en el sistema.",
                    raiz.ChildNodes[0].Token.Location.Line, raiz.ChildNodes[0].Token.Location.Column);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
                return;
            }
            // Ahora sí hacemo el cambio
            foreach (Usuario user in usuarios)
            {
                if (user.username.ToLower().Equals(nombreUsuario))
                {
                    // Verificamos si la contraseña es la misma
                    if (user.password.Equals(nuevoPassword))
                    {
                        Error error = new Error("Semantico", "Debe ingresar una contraseña distinta a la actual.",
                        raiz.ChildNodes[2].Token.Location.Line, raiz.ChildNodes[3].Token.Location.Column);
                        Form1.errores.Add(error);
                        Form1.Mensajes.Add(error.getMensaje());
                        return;
                    }
                    else
                    {
                        user.password = nuevoPassword;
                        Form1.Mensajes.Add("Se ha actualizado la contraseña del usuario " + usuarioActual + " exitosamente.");
                    }
                }
            }


        }
        #endregion

        #region ALTERAR OBJETO
        public void alterarObjeto(ParseTreeNode raiz)
        {
            switch (raiz.ChildNodes[1].Token.Text.ToLower())
            {
                case "quitar":
                    quitarEnObjeto(raiz);
                    break;
                case "agregar":
                    agregarEnObjeto(raiz);
                    break;
            }
        }

        public void quitarEnObjeto(ParseTreeNode raiz)
        {
            if (getBase() != null)
            {
                String nombreObjeto = raiz.ChildNodes[0].Token.Text.ToLower();
                List<String> atributosAQuitar = new List<String>();
                foreach (ParseTreeNode nodoAtributo in raiz.ChildNodes[2].ChildNodes)
                {
                    atributosAQuitar.Add(nodoAtributo.ChildNodes[0].Token.Text.ToLower());
                }
                // Ahora encontramos el objeto

                List<Objeto> listaTemporal = getBase().objetos;
                bool objetoEncontrado = false;
                bool atributoEncontrado = false;
                bool flagerror = false;
                // Ahora verificamos que no exista el atributo
                foreach (Objeto obj in listaTemporal)
                {
                    // Encontramos el objeto a modificar
                    if (obj.nombre.ToLower().Equals(nombreObjeto))
                    {
                        // Recorremos sus atributos
                        for (int x = 0; x < atributosAQuitar.Count; x++)
                        {
                            foreach (Atributo atrib in obj.atributos)
                            {
                                if (atrib.id.ToLower().Equals(atributosAQuitar[x]))
                                {
                                    atributoEncontrado = true;
                                }
                            }
                            if (!atributoEncontrado)
                            {
                                Error error = new Error("Semantico", "No existe el atributo " + atributosAQuitar + " en el objeto " + obj.nombre
                                     , raiz.ChildNodes[0].ChildNodes[x].ChildNodes[0].Token.Location.Line
                                     , raiz.ChildNodes[0].ChildNodes[x].ChildNodes[0].Token.Location.Column);
                                Form1.errores.Add(error);
                                Form1.Mensajes.Add(error.getMensaje());
                                flagerror = true;
                            }
                        }
                        objetoEncontrado = true;
                    }
                }
                if (!objetoEncontrado)
                {
                    Error error = new Error("Semantico", "El objeto " + nombreObjeto + " ya existe la base de datos " + getBase().nombre
                        , raiz.ChildNodes[0].Token.Location.Line
                        , raiz.ChildNodes[0].Token.Location.Column);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                    atributoEncontrado = true;
                    return;
                }
                if (flagerror) // Si hay errores abortamos la ejecucion
                {
                    return;
                }
                foreach (Objeto obj in listaTemporal)
                {
                    // Encontramos el objeto a modificar
                    if (obj.nombre.ToLower().Equals(nombreObjeto))
                    {
                        // Recorremos sus atributos
                        for (int x = 0; x < atributosAQuitar.Count; x++)
                        {
                            for (int y = 0; y < obj.atributos.Count; y++)
                            {
                                if (obj.atributos[y].id.ToLower().Equals(atributosAQuitar[x]))
                                {
                                    obj.atributos.RemoveAt(y);
                                    y--;
                                }
                            }

                        }
                        objetoEncontrado = true;
                    }
                }
            }
        }
        public void agregarEnObjeto(ParseTreeNode raiz)
        {
            if (getBase() != null)
            {
                String nombreObjeto = raiz.ChildNodes[0].Token.Text.ToLower();
                List<Atributo> listaNuevosAributos = new List<Atributo>();
                // Obtenemos los atributos a agregar
                foreach (ParseTreeNode nodoAtributo in raiz.ChildNodes[2].ChildNodes)
                {
                    String tipoAtributo = nodoAtributo.ChildNodes[0].ChildNodes[0].Token.Text.ToLower();
                    String nombreAtributo = nodoAtributo.ChildNodes[1].Token.Text.ToLower();
                    Atributo attr = new Atributo(tipoAtributo, nombreAtributo, null);
                    switch (tipoAtributo)
                    {
                        case "text":
                            attr.valor = "";
                            break;
                        case "integer":
                            attr.valor = 0;
                            break;
                        case "double":
                            attr.valor = 0.00;
                            break;
                        case "date":
                        case "datetime":
                            attr.valor = "null";
                            break;
                    }
                    listaNuevosAributos.Add(attr);
                }
                List<Objeto> listaTemporal = new List<Objeto>();
                listaTemporal = getBase().objetos;
                bool objetoEncontrado = false;
                bool atributoEncontrado = false;
                // Ahora verificamos que no exista el atributo
                foreach (Objeto obj in listaTemporal)
                {
                    // Encontramos el objeto a modificar
                    if (obj.nombre.ToLower().Equals(nombreObjeto))
                    {
                        // Recorremos sus atributos
                        foreach (Atributo attributo in obj.atributos)
                        {
                            for (int x = 0; x < listaNuevosAributos.Count; x++)
                            {
                                /// Veficiamos que no exista
                                if (attributo.id.ToLower().Equals(listaNuevosAributos[x].id))
                                {
                                    Error error = new Error("Semantico", "El atributo " + attributo.id + " ya existe en el objeto " + nombreObjeto
                                        , raiz.ChildNodes[2].ChildNodes[x].ChildNodes[0].ChildNodes[0].Token.Location.Line
                                        , raiz.ChildNodes[2].ChildNodes[x].ChildNodes[0].ChildNodes[0].Token.Location.Column);
                                    Form1.errores.Add(error);
                                    Form1.Mensajes.Add(error.getMensaje());
                                    atributoEncontrado = true;
                                }
                            }
                        }
                        objetoEncontrado = true;
                    }
                }
                if (!objetoEncontrado)
                {
                    Error error = new Error("Semantico", "El objeto " + nombreObjeto + " ya existe la base de datos " + getBase().nombre
                        , raiz.ChildNodes[0].Token.Location.Line
                        , raiz.ChildNodes[0].Token.Location.Column);
                    Form1.errores.Add(error);
                    Form1.Mensajes.Add(error.getMensaje());
                    atributoEncontrado = true;
                    return;
                }
                if (atributoEncontrado)// Abortamos la insecion
                {
                    return;
                }
                foreach (Atributo attributo in listaNuevosAributos)
                {
                    foreach (Objeto obj in listaTemporal)
                    {
                        // Encontramos el objeto a modificar
                        if (obj.nombre.ToLower().Equals(nombreObjeto))
                        {
                            obj.atributos.Add(attributo);
                        }
                    }
                }
            }
        }
        #endregion

        #region ALTERAR TABLA
        public void alterarTabla(ParseTreeNode raiz)
        {
            switch (raiz.ChildNodes[1].Token.Text.ToLower())
            {
                case "quitar":
                    quitarEnTabla(raiz);
                    break;
                case "agregar":
                    agregarEnTabla(raiz);
                    break;
            }
        }

        public void quitarEnTabla(ParseTreeNode raiz)
        {
            String nombreTabla = raiz.ChildNodes[0].Token.Text;
            List<String> listaCampos = new List<String>();
            foreach (ParseTreeNode nodo in raiz.ChildNodes[2].ChildNodes)
            {
                listaCampos.Add(nodo.ChildNodes[0].Token.Text);
            }
            int contador = 0;
            // Ahora buscamos la tabla ;
            if (getBase() != null)
            {
                Tabla tablaActual = getTabla(nombreTabla, raiz.ChildNodes[0].Token.Location.Line, raiz.ChildNodes[0].Token.Location.Column);
                if (tablaActual != null)
                {
                    // Primero quitamos los campos de la definicion
                    List<defCampo> definicionesTemporales = new List<defCampo>();
                    foreach (String nombre in listaCampos)
                    {
                        foreach (defCampo definicion in tablaActual.definiciones)
                        {
                            if (!nombre.ToLower().Equals(definicion.nombre.ToLower()))
                            {
                                definicionesTemporales.Add(definicion);
                            }
                            else
                            {
                                contador++;
                            }
                        }
                    }
                    if (contador < listaCampos.Count)// Hay error en algún campo
                    {
                        Error error = new Error("Semantico", "Uno de los campos solicitado no existen en la tabla " + nombreTabla, raiz.ChildNodes[2].ChildNodes[0].Span.Location.Line, raiz.ChildNodes[2].ChildNodes[0].Span.Location.Column);
                        Form1.errores.Add(error);
                        Form1.Mensajes.Add(error.getMensaje());
                        return;
                    }
                    tablaActual.definiciones = definicionesTemporales;
                    // Segundo quitamos los campos de las tuplas                    
                    for (int x = 0; x < tablaActual.tuplas.Count; x++)
                    {
                        tupla tup = tablaActual.tuplas[x];
                        for (int y = 0; y < tablaActual.tuplas[x].campos.Count; y++)
                        {
                            campo cmp = tablaActual.tuplas[x].campos[y];
                            foreach (String nombre in listaCampos)
                            {
                                if (cmp.id.ToLower().Equals(nombreTabla + "." + nombre.ToLower()))
                                {
                                    tablaActual.tuplas[x].campos.RemoveAt(y);
                                    y--;
                                }
                            }
                        }
                    }
                }
            }

        }

        public void agregarEnTabla(ParseTreeNode raiz)
        {
            String nombreTabla = raiz.ChildNodes[0].Token.Text;
            List<defCampo> definicionesTemporales = new List<defCampo>();
            List<int> lineas = new List<int>();
            List<int> columnas = new List<int>();
            Object valor = null;
            if (getBase() != null)
            {
                Tabla tablaActual = getTabla(nombreTabla, raiz.ChildNodes[0].Token.Location.Line, raiz.ChildNodes[0].Token.Location.Column);
                if (tablaActual != null)
                {
                    foreach (ParseTreeNode nodo in raiz.ChildNodes[2].ChildNodes)
                    {
                        String tipo = nodo.ChildNodes[0].ChildNodes[0].Token.Text.ToLower();                        
                        switch (tipo.ToLower())
                        {
                            case "text":
                                valor = " ";
                                break;
                            case "double":
                                valor = 0.00;
                                break;
                            case "integer":
                                valor = 0;
                                break;
                            case "bool":
                                valor = 0;
                                break;
                        }
                        lineas.Add(nodo.ChildNodes[0].ChildNodes[0].Token.Location.Line);
                        columnas.Add(nodo.ChildNodes[0].ChildNodes[0].Token.Location.Column);
                        String nombre = nodo.ChildNodes[1].Token.Text.ToLower();
                        bool auto = false;
                        bool primaria = false;
                        bool unico = false;
                        bool nulo = false;
                        String foranea = "";
                        // Ahora obtenemos los complementos
                        foreach (ParseTreeNode nodoCom in nodo.ChildNodes[2].ChildNodes)
                        {
                            if (nodoCom.Token != null)
                            {
                                switch (nodoCom.Token.Text.ToLower())
                                {
                                    case "llave_primaria":
                                        primaria = false;
                                        break;
                                    case "autoincrementable":
                                        auto = false;
                                        break;
                                    case "nulo":
                                        nulo = false;
                                        break;
                                    case "no nulo":
                                        nulo = false;
                                        break;
                                    case "unico":
                                        unico = false;
                                        break;
                                }
                            }
                            else
                            {
                                foranea = nodoCom.ChildNodes[0].Token.Text.ToLower().Replace("\"", "") + "." + nodoCom.ChildNodes[1].Token.Text.ToLower().Replace("\"", "");
                            }
                        }
                        // Agregamos la nueva definicion temporal
                        definicionesTemporales.Add(new defCampo(nombre, tipo, auto, nulo, primaria, foranea, unico));
                    }
                    List<defCampo> listaNuevaDefiniciones = new List<defCampo>();
                    int y = 0;
                    bool flag = false;
                    foreach (defCampo definicionActual in definicionesTemporales)
                    {
                        for (int x = 0; x < tablaActual.definiciones.Count; x++)
                        {
                            defCampo def = tablaActual.definiciones[x];
                            if (def.nombre.ToLower().Equals(definicionActual.nombre.ToLower()))
                            {
                                Error error = new Error("Semantico", "El campo " + def.nombre.ToLower() + " ya existe en la tabla " + nombreTabla,
                                    lineas[y], columnas[y]);
                                Form1.errores.Add(error);
                                Form1.Mensajes.Add(error.getMensaje());
                                flag = true;
                                y++;
                            }
                        }
                        if (!flag)
                        {
                            // Agregamos la nueva definicion                            
                            listaNuevaDefiniciones.Add(definicionActual);
                            flag = true;
                        }
                    }
                    if (y == 0)
                    {
                        // Agregamos las definiciniones
                        foreach (defCampo definicion in listaNuevaDefiniciones)
                        {
                            tablaActual.definiciones.Add(definicion);
                        }
                    }
                    else
                    {
                        return;
                    }

                    //Ahora agregamos los nuevos campos a las tuplas
                    foreach (tupla tup in tablaActual.tuplas)
                    {
                        foreach (defCampo definicion in listaNuevaDefiniciones)
                        {
                            campo nuevoCampo = new campo(definicion.nombre, null, definicion.tipo);
                            switch (nuevoCampo.tipo.ToLower())
                            {
                                case "text":
                                    nuevoCampo.valor = "";
                                    break;
                                case "integer":
                                    nuevoCampo.valor = 0;
                                    break;
                                case "double":
                                    nuevoCampo.valor = 0.00;
                                    break;
                                case "date":
                                case "datetime":
                                    nuevoCampo.valor = "null";
                                    break;
                                case "bool":
                                    nuevoCampo.valor = 0;
                                    break;
                            }
                            tup.campos.Add(nuevoCampo);
                        }
                    }
                }//--Tabla no null
            }
        }
        #endregion



        #region ACTUALIZAR
        public void actualizar(ParseTreeNode raiz)
        {
            if (getBase() != null)
            {
                if (raiz.ChildNodes.Count == 5)
                {
                    // Primero verificamos que el número de campos coincida con el número de valores.
                    if (raiz.ChildNodes[2].ChildNodes.Count == raiz.ChildNodes[3].ChildNodes.Count)
                    {
                        String nombreTabla = raiz.ChildNodes[1].Token.Text.ToLower();//Nombre de la tabla
                        List<String> listaEtiquetas = new List<String>();
                        //Obtenemos los parametros a modificar
                        foreach (ParseTreeNode nodo in raiz.ChildNodes[2].ChildNodes)
                        {
                            listaEtiquetas.Add(nodo.ChildNodes[0].Token.Text.ToLower());
                        }
                        List<campo> listaCampos = new List<campo>();
                        //Obtenemos la lista de valores.                    
                        for (int cont = 0; cont < raiz.ChildNodes[3].ChildNodes.Count; cont++)
                        {
                            ParseTreeNode nodo = raiz.ChildNodes[3].ChildNodes[cont];
                            Logica opL = new Logica();
                            Resultado result = opL.operar(nodo);
                            listaCampos.Add(new campo(listaEtiquetas[cont], result.valor, result.tipo));
                        }
                        // Ahora ya tenemos una lista de campos
                        if (getTabla(nombreTabla, raiz.ChildNodes[1].Token.Location.Line, raiz.ChildNodes[1].Token.Location.Column) != null)
                        {
                            Tabla tabActual = getTabla(nombreTabla, raiz.ChildNodes[1].Token.Location.Line, raiz.ChildNodes[1].Token.Location.Column);
                            bool flag = false;
                            foreach (defCampo definicion in tabActual.definiciones)
                            {
                                for (int x = 0; x < listaCampos.Count; x++)
                                {
                                    campo cmp = listaCampos[x];
                                    if (definicion.nombre.ToLower().Equals(cmp.id.ToLower()))
                                    {
                                        if (!definicion.tipo.ToLower().Equals(cmp.tipo.ToLower()))
                                        {
                                            Error error = new Error("Semantico", "Se esperaba un valor de tipo " + definicion.tipo.ToLower() + " y se ha recibido uno de tipo " + cmp.tipo.ToLower(),
                                               raiz.ChildNodes[3].ChildNodes[x].Token.Location.Line, raiz.ChildNodes[3].ChildNodes[x].Token.Location.Column);
                                            Form1.errores.Add(error);
                                            Form1.Mensajes.Add(error.getMensaje());
                                            flag = true;
                                        }
                                    }
                                }
                            }
                            if (flag) { return; } // Si hay errores de tipos, salimos.
                            foreach (tupla tup in getTabla(nombreTabla, raiz.ChildNodes[1].Token.Location.Line, raiz.ChildNodes[1].Token.Location.Column).tuplas)
                            {
                                Logica opL = new Logica(tup);
                                Resultado result = opL.operar(raiz.ChildNodes[4]);
                                if (!result.tipo.ToLower().Equals("error"))
                                {
                                    if (result.tipo.ToLower().Equals("bool") && result.valor.ToString().Equals("1")
                                        || result.tipo.ToLower().Equals("integer") && result.valor.ToString().Equals("1"))
                                    {
                                        // Hacemos la actualización
                                        foreach (campo campoTemp in tup.campos)
                                        {
                                            foreach (campo nuevoCampo in listaCampos)
                                            {
                                                if (campoTemp.id.ToLower().Equals(nuevoCampo.id.ToLower()))
                                                {
                                                    campoTemp.valor = nuevoCampo.valor;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            Form1.Mensajes.Add("Actualización realizada con éxito. :v");
                        }
                        else
                        {
                            Error error = new Error("Semantico", "La tabla " + nombreTabla + " no existe en la base " + getBase().nombre,
                                raiz.ChildNodes[2].ChildNodes[0].Token.Location.Line, raiz.ChildNodes[2].ChildNodes[0].Token.Location.Column);
                            Form1.errores.Add(error);
                            Form1.Mensajes.Add(error.getMensaje());
                        }

                    }
                    else
                    {
                        Error error = new Error("Semantico", "El número de parametros no coincide con el número de valores indicados.",
                            raiz.ChildNodes[2].ChildNodes[0].Token.Location.Line, raiz.ChildNodes[2].ChildNodes[0].Token.Location.Column);
                        Form1.errores.Add(error);
                        Form1.Mensajes.Add(error.getMensaje());
                    }
                }
                else if (raiz.ChildNodes.Count == 4)
                {
                    // Primero verificamos que el número de campos coincida con el número de valores.
                    if (raiz.ChildNodes[2].ChildNodes.Count == raiz.ChildNodes[3].ChildNodes.Count)
                    {
                        String nombreTabla = raiz.ChildNodes[1].Token.Text.ToLower();//Nombre de la tabla
                        List<String> listaEtiquetas = new List<String>();
                        //Obtenemos los parametros a modificar
                        foreach (ParseTreeNode nodo in raiz.ChildNodes[2].ChildNodes)
                        {
                            listaEtiquetas.Add(nodo.ChildNodes[0].Token.Text.ToLower());
                        }
                        List<campo> listaCampos = new List<campo>();
                        //Obtenemos la lista de valores.                    
                        for (int cont = 0; cont < raiz.ChildNodes[3].ChildNodes.Count; cont++)
                        {
                            ParseTreeNode nodo = raiz.ChildNodes[3].ChildNodes[cont];
                            Logica opL = new Logica();
                            Resultado result = opL.operar(nodo);
                            listaCampos.Add(new campo(listaEtiquetas[cont], result.valor, result.tipo));
                        }
                        // Ahora ya tenemos una lista de campos
                        if (getTabla(nombreTabla, raiz.ChildNodes[1].Token.Location.Line, raiz.ChildNodes[1].Token.Location.Column) != null)
                        {
                            Tabla tabActual = getTabla(nombreTabla, raiz.ChildNodes[1].Token.Location.Line, raiz.ChildNodes[1].Token.Location.Column);
                            bool flag = false;
                            foreach (defCampo definicion in tabActual.definiciones)
                            {
                                for (int x = 0; x < listaCampos.Count; x++)
                                {
                                    campo cmp = listaCampos[x];
                                    if (definicion.nombre.ToLower().Equals(cmp.id.ToLower()))
                                    {
                                        if (!definicion.tipo.ToLower().Equals(cmp.tipo.ToLower()) )
                                        {
                                            if (
                                                definicion.tipo.ToLower().Equals("integer") && cmp.tipo.ToLower().Equals("bool") ||
                                                definicion.tipo.ToLower().Equals("bool") && cmp.tipo.ToLower().Equals("integer")
                                              )
                                            {
                                            }
                                            else
                                            {
                                                Error error = new Error("Semantico", "Se esperaba un valor de tipo " + definicion.tipo.ToLower() + " y se ha recibido uno de tipo " + cmp.tipo.ToLower(),
                                                   raiz.ChildNodes[3].ChildNodes[x].Token.Location.Line, raiz.ChildNodes[3].ChildNodes[x].Token.Location.Column);
                                                Form1.errores.Add(error);
                                                Form1.Mensajes.Add(error.getMensaje());
                                                flag = true;
                                            }
                                        }
                                    }
                                }
                            }
                            if (flag) { return; } // Si hay errores de tipos, salimos.
                            foreach (tupla tup in getTabla(nombreTabla, raiz.ChildNodes[1].Token.Location.Line, raiz.ChildNodes[1].Token.Location.Column).tuplas)
                            {
                                foreach (campo campoTemp in tup.campos)
                                {
                                    foreach (campo nuevoCampo in listaCampos)
                                    {
                                        if (campoTemp.id.ToLower().Equals(nuevoCampo.id.ToLower()))
                                        {
                                            campoTemp.valor = nuevoCampo.valor;
                                        }
                                    }
                                }
                            }
                            Form1.Mensajes.Add("Actualización realizada con éxito. :v");
                        }
                        else
                        {
                            Error error = new Error("Semantico", "La tabla " + nombreTabla + " no existe en la base " + getBase().nombre,
                                raiz.ChildNodes[2].ChildNodes[0].Token.Location.Line, raiz.ChildNodes[2].ChildNodes[0].Token.Location.Column);
                            Form1.errores.Add(error);
                            Form1.Mensajes.Add(error.getMensaje());
                        }

                    }
                    else
                    {
                        Error error = new Error("Semantico", "El número de parametros no coincide con el número de valores indicados.",
                            raiz.ChildNodes[2].ChildNodes[0].Token.Location.Line, raiz.ChildNodes[2].ChildNodes[0].Token.Location.Column);
                        Form1.errores.Add(error);
                        Form1.Mensajes.Add(error.getMensaje());
                    }
                }
            }
        }
        #endregion
        #region BACKUP
        public void backup(ParseTreeNode raiz)
        {
            commit();
            String tipo = raiz.ChildNodes[0].Token.Text.ToLower();
            String nombreBase = raiz.ChildNodes[1].Token.Text.ToLower();
            String nombreArchivo = raiz.ChildNodes[2].Token.Text.ToLower();
            String pathBase = "";
            bool flag = false;
            foreach (BD boss in basesdedatos)
            {
                if (boss.nombre.ToLower().Equals(nombreBase))
                {
                    pathBase = boss.path;
                    flag = true;
                }
            }
            if (!flag)//  No se encontró la base
            {
                Error error = new Error("Semantico", "La base de datos " + nombreBase + " no existe en el sistema",
                    raiz.ChildNodes[1].Token.Location.Line, raiz.ChildNodes[1].Token.Location.Column);
                Form1.errores.Add(error);
                Form1.Mensajes.Add(error.getMensaje());
            }
            switch (tipo)
            {
                case "completo":
                    backupCompleto(nombreBase, nombreArchivo);
                    break;
                case "usqldump":
                    backupdump();
                    break;
            }
        }
        public void backupCompleto(String nombreBase, String nombreArchivo)
        {
            String pathBase = Form1.pathRaiz + "BD" + "\\"+nombreBase;
            guardarArchivo(pathBase + "nombre.txt", nombreBase);
            String zipPath = Form1.pathRaiz + "\\backup\\" + nombreBase + ".zip";
            System.IO.File.Delete(zipPath);
            ZipFile.CreateFromDirectory(pathBase, zipPath);
            System.IO.File.Delete(pathBase + "\\nombre.txt");
        }
        public void backupdump()
        {
        }

        public void restaurar(ParseTreeNode raiz)
        {
            String path = raiz.ChildNodes[1].Token.Text.Replace("\"","");
            switch (raiz.ChildNodes[0].Token.Text.ToLower())
            {
                case "completo":
                    string pathDestino = Form1.pathRaiz + "BD" ;
                    string zipPath = path;
                    guardarArchivo(pathDestino,"");
                    eliminarArchivo(pathDestino + "\\nombre.txt");
                    ZipFile.ExtractToDirectory(zipPath, pathDestino);                    
                    String nombre = getArchivo(pathDestino + "\\nombre.txt");
                    BD nuevaBAse = new BD(nombre, pathDestino + "\\"+ nombre + "\\DB.xml");
                    nuevaBAse.pathObjetos = pathDestino + "\\" + nombre + "\\objetos.xml";
                    nuevaBAse.pathProcedimientos = pathDestino + "\\" + nombre + "\\procedimientos.xml";
                    basesdedatos.Add(nuevaBAse);
                    System.IO.File.Delete(pathDestino + "\\nombre.txt");
                    break;
                    //C:\DB\BD
            }
        }
        #endregion
        public String getArchivo(String path)
        {
            try
            {
                String textoArchivo = System.IO.File.ReadAllText(path);
                return textoArchivo;
                throw new FileNotFoundException();                
            }
            catch (FileNotFoundException)
            {
                return "0";
            }
            catch (Exception ex)
            {
                
                return "0";
            }

        }
    }
}
