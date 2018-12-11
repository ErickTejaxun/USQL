using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServidorDB
{
    public partial class Form1 : Form
    {
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
            string respuesta = analizador.esCadenaValida(contenidoArchivo, gramatica);
            if (respuesta.Equals("1"))
            {
                MessageBox.Show("Se ha construido el AST.");
                //outputConsola.Text =  gramatica.Obtener_Resultado(contenidoArchivo).ToString(); 
            }
            else
            {
                MessageBox.Show("Errores en la cadena de entrada");
                Console.WriteLine(respuesta);
            }
        }
    }
}
