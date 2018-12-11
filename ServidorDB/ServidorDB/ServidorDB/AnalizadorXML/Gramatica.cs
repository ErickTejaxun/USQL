using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;
using Irony.Interpreter.Ast;
using System.Collections;
/**
 * PROYECTO QUE REALIZA SUMAS RESTAS MULTIPLICACIONES Y DIVICIONES DE NUMEROS IMPLEMENTANDO IRONY
 * */
namespace ServidorDB.AnalizadorXML
{

    /**
     * CLASE ARBOL
     * HEREDA DE ASTNODE PARA QUE SE PUEDA IMPLEMENTAR EN LOS NO TERMINALES
     * EL METODO A SOBREESCRIBIR ES INIT QUE SE EJECUTARA AL REDUCIR POR EL NOTERMINAL QUE IMPLEMENTE ESTA CLASE
     * LOS PARAMETROS SERA EL CONTEXTO Y EL PARSETREENODE QUE SE REDUJO
     * */
    public class Arbol : AstNode
    {
        public Object Name;
        public List<Arbol> hijos;
        public override void Init(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            hijos = new List<Arbol>();//inicializamos la lista
            base.Init(context, treeNode);
            switch ((String)treeNode.Term.Name)
            {           //  0 1 2
                case "E":// E + E | E - E | E * E  | num
                    if (treeNode.ChildNodes.Count > 1)
                    {
                        Name = treeNode.ChildNodes[1].FindTokenAndGetText(); //SIGNO
                        hijos.Add((Arbol)treeNode.ChildNodes[0].AstNode); // PRIMER VALOR
                        hijos.Add((Arbol)treeNode.ChildNodes[2].AstNode); // SEGUNDO VALOR
                    }
                    else
                    {
                        Name = treeNode.ChildNodes[0].FindTokenAndGetText(); //OBTENEMOS EL NUMERO
                    }
                    break;
                case "number":
                    Name = treeNode.Token.Text;
                    break;
                default:
                    break;
            }
            
        }
    }

    class analizador : Grammar
    {
        //METODO QUE RECORRERA EL ARBOL AST          
        public Double Valor(Arbol arbol)
        {
            Double entero1;
            Double entero2;
            switch ((String)arbol.Name)
            {
                case "+":
                    entero1 = Valor(arbol.hijos[0]);
                    entero2 = Valor(arbol.hijos[1]);
                    return entero1 + entero2;
                case "-":
                    entero1 = Valor(arbol.hijos[0]);
                    entero2 = Valor(arbol.hijos[1]);
                    return entero1 - entero2;
                case "*":
                    entero1 = Valor(arbol.hijos[0]);
                    entero2 = Valor(arbol.hijos[1]);
                    return entero1 * entero2;
                case "/":
                    entero1 = Valor(arbol.hijos[0]);
                    entero2 = Valor(arbol.hijos[1]);
                    return entero1 / entero2;
                default:
                    return Convert.ToDouble(arbol.Name);
            }
        }

        public analizador()
        {
            var entero = new RegexBasedTerminal("entero", "[0-9]+");
            IdentifierTerminal id = new IdentifierTerminal("id");
            var singleLineComment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
            var delimitedComment = new CommentTerminal("DelimitedComment", "/*", "*/");
            var p = new StringLiteral("p", "\"");

            var ENTERO = ToTerm("entero");
            var CADENA = ToTerm("cadena");
            var DOUBLE = ToTerm("double");

            //ENTERO.AstConfig.NodeType = typeof(Arbol);
            //CADENA.AstConfig.NodeType = typeof(Arbol);
            //DOUBLE.AstConfig.NodeType = typeof(Arbol);


            p.AstConfig.NodeType = typeof(Arbol);
            entero.AstConfig.NodeType = typeof(Arbol);
            id.AstConfig.NodeType = typeof(Arbol);
            NonTerminal E = new NonTerminal("E", typeof(Arbol));



            E.Rule = E + ToTerm("+") + E
                | E + ToTerm("-") + E
                | E + ToTerm("*") + E
                | E + ToTerm("/") + E
                | ToTerm("(") + E + ToTerm(")")
                | entero
                | id;
                
            RegisterOperators(1, "+", "-");//ESTABLESEMOS PRESEDENCIA
            RegisterOperators(2, "*", "/");

            this.Root = E;
            NonGrammarTerminals.Add(singleLineComment); // QUITAMOS LOS COMENTARIOS DE LA GRAMATICA
            NonGrammarTerminals.Add(delimitedComment);
            LanguageFlags = LanguageFlags.CreateAst;    //IMPORTANTE PARA CREAR EL ARBOL SIN ESTO NO LO CREARA
        }

        public Double Obtener_Resultado(string sourceCode)
        {
            LanguageData language = new LanguageData(this);
            Parser parser = new Parser(language);
            ParseTree parseTree = parser.Parse(sourceCode);
            ParseTreeNode root = parseTree.Root;
            Arbol a = (Arbol)root.AstNode;
            return Valor(a); //OBTENEMOS EL VALRO DE LA ENTRADA
        }

    }

}