using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServidorDB.EjecucionUsql;
using Irony.Parsing;

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
                    insertar(idTabla, raiz.ChildNodes[1]);
                    break;
                case 3: // 0 idbase, 1. Lista id campos, 2. lista valores
                    insertar(idTabla, raiz.ChildNodes[1], raiz.ChildNodes[2]);
                    break;               
            }            
        }

        public void insertar(String nombreTabla, ParseTreeNode raizValores)
        {
            tupla nuevaTupla = new tupla();
            foreach (ParseTreeNode nodo in raizValores.ChildNodes)
            {
                /*
                 * Logica opL = new Logica();
                 * Resultado result = opL.operar(nodo);                 
                 */
                 
                 //nuevaTupla.addCampo(new campo("", result.))
            }
        }
        public void insertar(String nombreTabla, ParseTreeNode raizCampos, ParseTreeNode raizValores)
        {

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
                    String kforanea = def.foranea;
                    if (def.auto) { autoinc = 1; } else { autoinc = 0; }
                    if (def.nulo) { nulo = 1; } else { nulo = 0; }
                    if (def.primaria) { prim = 1; } else { prim = 0; }
                    if (kforanea.Equals("")) { kforanea = "0"; }
                    cadena = cadena + "<campo>\n";
                    cadena = cadena + "\t<" + def.tipo+">" + def.nombre + "</" + def.tipo + ">\n";
                    cadena = cadena + "<propiedades>\n";                    
                    cadena = cadena + "\t<autoincrementable>" + autoinc + "</autoincrementable>\n";
                    cadena = cadena + "\t<nulo>" + nulo + "</nulo>\n";
                    cadena = cadena + "\t<primaria>" + prim + "</primaria>\n";
                    cadena = cadena + "\t<foranea>" + kforanea + "</foranea>\n";
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
                        cadena = cadena + "<" + cmp.id.ToLower() + ">" + fecha.ToString("dd-MM-yyyy HH:mm:ss")
                            + "</" + cmp.id.ToLower() + ">\n";
                    }
                    else
                    {
                        cadena = cadena + "<" + cmp.id.ToLower() + ">" + cmp.valor.ToString()
                            + "</" + cmp.id.ToLower() + ">\n";
                    }

                }
                cadena = cadena + "\n" + "</" + "row" + ">";
            }
            guardarArchivo(tab.path, cadena);
            Form1.Mensajes.Add(cadena);
        }


        public void guardarArchivo(String path, String contenido)
        {                                   
            System.IO.File.WriteAllText(path, contenido); // Almacenamos el archivo
        }
        #endregion

    }
}
