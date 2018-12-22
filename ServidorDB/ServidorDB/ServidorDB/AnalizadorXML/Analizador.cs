using Irony.Parsing;
using System;
using System.Drawing;
using System.IO;
using WINGRAPHVIZLib;

namespace ServidorDB.AnalizadorXML
{
    public class Analizador
    {
        String graph = "";
        String ruta = @"C:\Users\erick\Documents\Graficas\";
        public string esCadenaValida(string cadenaEntrada, Grammar gramatica)
        {
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser p = new Parser(lenguaje);
            ParseTree arbol = p.Parse(cadenaEntrada);
            ParseTreeNode raiz = arbol.Root;
            if (raiz == null)
            {
                return getErrores(arbol);
            }
            else
            {
                //generarImagen(raiz);                
                Genarbol(raiz);
                generateGraph("ejemplo.txt");
                return getErrores(arbol);            
            }            
            //return "1";
        }

        public ParseTree generarArbol(string cadenaEntrada, Grammar gramatica)
        {
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser p = new Parser(lenguaje);
            ParseTree arbol = p.Parse(cadenaEntrada);
            return arbol;
        }

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
                    errores += "Error en: Linea"+arbol.ParserMessages[x].Location.Line + "\tColumna:" + arbol.ParserMessages[x].Location.Column + "\r\n---------->" 
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
            return cabecera  + errores;
        }



        private static void generarImagen(ParseTreeNode raiz)
        {            
            string grafoDOT = AST.diagramaDOT.getDOT(raiz);
            WINGRAPHVIZLib.DOT dot = new WINGRAPHVIZLib.DOT();            
            WINGRAPHVIZLib.BinaryImage img = dot.ToPNG(grafoDOT);
            String path = Directory.GetCurrentDirectory() + "\\AST.png";
            Console.Write(path);
            img.Save(path);
            

        }

        private static void generarImagen2(ParseTreeNode raiz)
        {

            string grafoDOT = AST.diagramaDOT.getDOT(raiz);
            WINGRAPHVIZLib.DOT dot = new WINGRAPHVIZLib.DOT();
            WINGRAPHVIZLib.BinaryImage img = dot.ToPNG(grafoDOT);
            String path = Directory.GetCurrentDirectory() + "\\AST.png";
            Console.Write(path);
            img.Save(path);


        }


        public static Image dibujarGrafo(String grafo_en_DOT)
        {
            WINGRAPHVIZLib.DOT dot = new WINGRAPHVIZLib.DOT();
            WINGRAPHVIZLib.BinaryImage img = dot.ToPNG(grafo_en_DOT);
            byte[] imageBytes = Convert.FromBase64String(img.ToBase64String());
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image imagen = Image.FromStream(ms, true);
            return imagen;
        }


        public void Genarbol(ParseTreeNode raiz)
        {
            System.IO.StreamWriter f = new System.IO.StreamWriter(ruta + "ejemplo.txt");
            f.Write("digraph lista{ rankdir=TB;node [shape = box, style=rounded]; ");
            graph = "";
            Generar(raiz);
            f.Write(graph);
            f.Write("}");
            f.Close();
        }
        public void Generar(ParseTreeNode raiz)
        {
            graph = graph + "nodo" + raiz.GetHashCode() + "[label=\"" + raiz.ToString().Replace("\"", "\\\"") + " \", fillcolor=\"yellow\", style =\"filled\", shape=\"circle\"]; \n";
            if (raiz.ChildNodes.Count > 0)
            {
                ParseTreeNode[] hijos = raiz.ChildNodes.ToArray();
                for (int i = 0; i < raiz.ChildNodes.Count; i++)
                {
                    Generar(hijos[i]);
                    graph = graph + "\"nodo" + raiz.GetHashCode() + "\"-> \"nodo" + hijos[i].GetHashCode() + "\" \n";
                }
            }
        }


        public void generateGraph(string fileName)
        {
            //try
            //{
            //    var command = string.Format("dot -Tjpg {0} -o {1}", Path.Combine(ruta, fileName), Path.Combine(ruta, fileName.Replace(".txt", ".jpg")));
            //    //String command = "dot -Tjpg " + fileName + " -o " + fileName.Replace(".txt", ".jpg");
            //    var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + command);
            //    var proc = new System.Diagnostics.Process();
            //    proc.StartInfo = procStartInfo;
            //    proc.Start();
            //    proc.WaitForExit();
            //}
            //catch (Exception x)
            //{

            //}
        }
        public void generateGraph2(string fileName) { 
        ////{
        ////    try
        ////    {
        ////        var command = string.Format("dot -Tjpg {0} -o {1}", Path.Combine(ruta, fileName), Path.Combine(ruta, fileName.Replace(".txt", ".jpg")));
        ////        String commando = "dot -Tjpg " + fileName + " -o " + fileName.Replace(".txt", ".jpg");
        ////        var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + command);
        ////        var proc = new System.Diagnostics.Process();
        ////        proc.StartInfo = procStartInfo;
        ////        proc.Start();
        ////        proc.WaitForExit();
        ////    }
        ////    catch (Exception x)
        ////    {

        ////    }
        }

    }
}