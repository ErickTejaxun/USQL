using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irony.Parsing;
using ServidorDB.estructurasDB;

namespace ServidorDB
{    
    public partial class Form1 : Form
    {        

        private String pathRaiz = "C:\\DB\\";
        private SistemaArchivos sistemaArchivos;
        public Form1()
        {
            InitializeComponent();
            ArrancarSistemaDeArchivos();
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
                imprimirSalida("Archivo cargado correctamente.");
            }
            else
            {
                imprimirSalida("Archivo con errores. Verificar Archivo.");
                imprimirSalida(respuesta);
            }
        }

        private void ArrancarSistemaDeArchivos()
        {
            String contenidoArchivo = getArchivo(pathRaiz+"maestro.xml");
            inputConsole.Text = contenidoArchivo;
            AnalizadorXML.Analizador analizador = new AnalizadorXML.Analizador();
            AnalizadorXML.XMLGramatica gramatica = new AnalizadorXML.XMLGramatica();
            ParseTree arbol = analizador.generarArbol(contenidoArchivo, gramatica);
            ParseTreeNode raiz = arbol.Root;
            String errores = "";
            errores = getErrores(arbol);
            if (errores.Equals(""))
            {
                imprimirSalida("Archivo maestro cargado correctamente");
            }
            else
            {
                imprimirSalida("El archivo maestro contiene errores. Cargado parcialmente");
                imprimirSalida(errores);
            }
            if (raiz != null)
            {
                analizador.Genarbol(raiz);
                analizador.generateGraph("ejemplo.txt");
                ServidorDB.AnalizadorXML.Ejecucion ejecutor = new ServidorDB.AnalizadorXML.Ejecucion(raiz);
                sistemaArchivos = new SistemaArchivos();
                sistemaArchivos.basesdedatos = ejecutor.recorrerArbolMaestro(); // Recorre el archivo maestro y nos devuelve la lista de bases de datos;
                //#region Abrir archivo de Usuarios
                //contenidoArchivo = getArchivo(pathRaiz + "usuarios.xml");
                //arbol = analizador.generarArbol(contenidoArchivo, gramatica);
                //raiz = arbol.Root;
                //#endregion
            }
            else
            {

            }
            

        }

        public String getArchivo(String path)
        {
            string textoArchivo = System.IO.File.ReadAllText(@path);            
            return textoArchivo;
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

    }
}
