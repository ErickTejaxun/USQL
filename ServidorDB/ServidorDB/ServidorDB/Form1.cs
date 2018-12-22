using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irony.Parsing;
using ServidorDB.estructurasDB;
using ServidorBDD.AnalisisUsql;
using ServidorBDD.EjecucionUsql;
using ServidorDB.EjecucionUsql;

namespace ServidorDB
{    
    public partial class Form1 : Form
    {        

        public static String pathRaiz = "C:\\DB\\";
        public static SistemaArchivos sistemaArchivos;
        public static List<Error> errores;
        public static List<String> Mensajes;
        public static Interprete i;

        public Form1()
        {
            InitializeComponent();            
        }

        private void runButton_Click(object sender, EventArgs e)
        {            
            String contenidoArchivo = inputConsole.Text;
            Console.WriteLine("Texto encontrado --- \n" + contenidoArchivo);
            AnalizadorXML.Analizador analizador = new AnalizadorXML.Analizador();
            AnalizadorXML.XMLGramatica gramatica = new AnalizadorXML.XMLGramatica();
            //AnalizadorXML.analizador gramatica = new AnalizadorXML.analizador();
            ParseTree arbol = analizador.generarArbol(contenidoArchivo, gramatica);
            string respuesta = analizador.esCadenaValida(contenidoArchivo, gramatica);
            if (respuesta.Equals(""))
            {
                imprimirSalida("Archivo cargado correctamente.------------------------------------------------------------");
            }
            else
            {
                imprimirSalida("Archivo con errores. Verificar Archivo.------------------------------------------------------------");
                imprimirSalida(respuesta);
            }
        }

        private void ArrancarSistemaDeArchivos()
        {
            sistemaArchivos = new SistemaArchivos(); // Inicializamos el sistema de archivos
            analizarArchivoMaestro();
            analizarArchivoUsuarios();
            analizarBasesDatos();            
        }

        //Este metodo analiza el archivo DB y carga las tablas, metodos, y objetos.
        public void analizarBasesDatos()
        {
            foreach (BD bass in sistemaArchivos.basesdedatos)
            {
                cargarBaseDatos(bass);
            }
        }

        private BD cargarBaseDatos(BD baseActual)
        {
            String contenidoArchivo = getArchivo(baseActual.path);
            inputConsole.Text = contenidoArchivo;
            AnalizadorXML.Analizador analizador = new AnalizadorXML.Analizador();
            AnalizadorXML.XMLGramatica gramatica = new AnalizadorXML.XMLGramatica();
            ParseTree arbol = analizador.generarArbol(contenidoArchivo, gramatica);
            ParseTreeNode raiz = arbol.Root;
            String errores = "";
            errores = getErrores(arbol);
            if (errores.Equals(""))
            {
                imprimirSalida("Archivo de base "+baseActual.path+" de datos cargado correctamente------------------------------------------------------------");
            }
            else
            {
                imprimirSalida("El archivo de base de datos  " + baseActual.path + " contiene errores. Cargado parcialmente------------------------------------------------------------");
                imprimirSalida(errores);
            }
            if (raiz != null)
            {
                analizador.Genarbol(raiz);
                analizador.generateGraph("ejemplo.txt");
                ServidorDB.AnalizadorXML.Ejecucion ejecutor = new ServidorDB.AnalizadorXML.Ejecucion(raiz, this);
                ejecutor.cargarBaseDatos(baseActual);
                //sistemaArchivos.basesdedatos = ejecutor.recorrerArbolMaestro(); // Recorre el archivo maestro y nos devuelve la lista de bases de datos;
            }
            return null;
        }

        public void analizarArchivoMaestro()
        {
            String contenidoArchivo = getArchivo(pathRaiz + "maestro.xml");
            Form1.errores = new List<Error>();
            inputConsole.Text = contenidoArchivo;
            AnalizadorXML.Analizador analizador = new AnalizadorXML.Analizador();
            AnalizadorXML.XMLGramatica gramatica = new AnalizadorXML.XMLGramatica();
            ParseTree arbol = analizador.generarArbol(contenidoArchivo, gramatica);
            ParseTreeNode raiz = arbol.Root;
            String errores = "";
            errores = getErrores(arbol);
            if (errores.Equals(""))
            {
                imprimirSalida("Archivo maestro cargado correctamente------------------------------------------------------------");
            }
            else
            {
                imprimirSalida("El archivo maestro contiene errores. Cargado parcialmente------------------------------------------------------------");
                imprimirSalida(errores);
            }
            if (raiz != null)
            {
                analizador.Genarbol(raiz);
                analizador.generateGraph("ejemplo.txt");
                ServidorDB.AnalizadorXML.Ejecucion ejecutor = new ServidorDB.AnalizadorXML.Ejecucion(raiz, this);
                sistemaArchivos.basesdedatos = ejecutor.recorrerArbolMaestro(); // Recorre el archivo maestro y nos devuelve la lista de bases de datos;
            }
            imprimirSalida("Se han encontrado " + sistemaArchivos.basesdedatos.Count +" bases de datos.");
        }

        public void analizarArchivoUsuarios()
        {
            String contenidoArchivo = getArchivo(pathRaiz + "usuarios.xml");
            inputConsole.Text = contenidoArchivo;
            AnalizadorXML.Analizador analizador = new AnalizadorXML.Analizador();
            AnalizadorXML.XMLGramatica gramatica = new AnalizadorXML.XMLGramatica();
            ParseTree arbol = analizador.generarArbol(contenidoArchivo, gramatica);
            ParseTreeNode raiz = arbol.Root;
            String errores = "";
            errores = getErrores(arbol);
            if (errores.Equals(""))
            {
                imprimirSalida("Archivo de Usuarios cargado correctamente------------------------------------------------------------");
            }
            else
            {
                imprimirSalida("El archivo de usuarios contiene errores. Cargado parcialmente------------------------------------------------------------");
                imprimirSalida(errores);
            }
            if (raiz != null)
            {
                analizador.Genarbol(raiz);
                analizador.generateGraph("ejemplo.txt");
                ServidorDB.AnalizadorXML.Ejecucion ejecutor = new ServidorDB.AnalizadorXML.Ejecucion(raiz, this);                
                sistemaArchivos.usuarios = ejecutor.recorrerUsuarios(); // Recorre el archivo de usuarios y nos devuelve la lista de usuarios con sus permisos
            }
        }
        public String getArchivo(String path)
        {
            // Tengo que ver que putas como hacer el try catch para que no se chingue cuando no encuentre el archivo
            // Ahorita iba a cargar las mierdas de las bases de datos: Empezando con el archivo BD donde están las paths de procedimientos, objetos y la definicion de la tabla
            //string textoArchivo = System.IO.File.ReadAllText(@path);            
            //return textoArchivo;

            try
            {
                String textoArchivo = System.IO.File.ReadAllText(@path);
                return textoArchivo;
                throw new FileNotFoundException();

                //The reste of the code
            }
            catch (FileNotFoundException)
            {
                return "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "0";
            }

        }

        public void imprimirSalida(String mensaje)
        {
            outputConsola.Text = outputConsola.Text + "\n" + "---------->" + mensaje;
        }


        #region Obtener errores del arbol de Irony
        //<param @arbol> Raiz del arbol de irony 
        public string getErrores(ParseTree arbol)
        {
            String errores = "";
            String cabecera = "";
            if (arbol.HasErrors())
            {
                int elementos = arbol.ParserMessages.Count;
                for (int x = 0; x < elementos; x++)
                {
                    cabecera += "Error en " + arbol.ParserMessages[x].Location + "\t" + arbol.ParserMessages[x].Message + "\r\n---------->";
                    errores += "Error en: Linea" + arbol.ParserMessages[x].Location.Line + "\tColumna:" + arbol.ParserMessages[x].Location.Column + "\r\n---------->"
                        + arbol.ParserMessages[x].Message + "@";
                }
            }
            errores = errores.Replace("expected", "Se esperaba");
            errores = errores.Replace("Syntax error", "Error Sintactico");
            errores = errores.Replace("Invalid character", "Caracter invalido");
            cabecera = cabecera.Replace("expected", "Se esperaba");
            cabecera = cabecera.Replace("Syntax error", "Error Sintactico");
            cabecera = cabecera.Replace("Invalid character", "Caracter invalido");
            /*
                ---------->Error en (9:1)	Invalid character: 'x'.
                ---------->Error en (12:2)	Error Sintactico, Se esperaba: db
                ---------->Error en (22:1)	Error Sintactico, Se esperaba: >
                ---------->
                8;0;Invalid character: 'x'.@11;1;Error Sintactico, Se esperaba: db@21;0;Error Sintactico, Se esperaba: >@             
             
             */
            return cabecera + errores;
        }
        #endregion

        private void iniciarServidorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1.Mensajes= new List<string>(); ;
            ArrancarSistemaDeArchivos();
            mostrarMensajes();
        }




        private void button1_Click(object sender, EventArgs e)
        {
            sistemaArchivos.commit();
            mostrarMensajes();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            outputConsola.Text = "";
            Form1.Mensajes = new List<string>(); ;
            Form1.errores = new List<Error>();
            GramaticaSDB grammar = new GramaticaSDB();
            LanguageData lenguaje = new LanguageData(grammar);
            Parser p = new Parser(lenguaje);
            ParseTree arbol = p.Parse(inputConsole.Text);
            AnalizadorXML.Analizador analizador = new AnalizadorXML.Analizador();
            if (arbol.Root != null)
            {
                imprimirSalida("Salida...");
                //analizador.Genarbol(arbol.Root);
                //analizador.generateGraph2("Ejemplo.txt");
                i = new Interprete(arbol.Root.ChildNodes[0]);                             
                Resultado result = i.ejecutar(arbol.Root.ChildNodes[0]);
                //imprimirSalida(result.valor+"");                
            }
            else
            {
                imprimirSalida(getErrores(arbol));
            }
            mostrarMensajes();
            //mostrarErrores();
            
        }
        public void mostrarMensajes()
        {
            imprimirSalida("--------------------------------------");
            foreach (String men in Mensajes)
            {
                imprimirSalida(men);
            }
            imprimirSalida("--------------------------------------");
        }
        public void mostrarErrores()
        {
            imprimirSalida("--------------------------------------");
            foreach (Error e in errores)
            {
                imprimirSalida(e.tipo + "  " + e.descripcion + "  "+e.linea + "  " + e.columna);
            }
            imprimirSalida("--------------------------------------");
        }
    }
}
