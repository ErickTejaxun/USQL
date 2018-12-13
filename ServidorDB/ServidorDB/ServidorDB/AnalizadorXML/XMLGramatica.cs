using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;
using Irony.Interpreter.Ast;

namespace ServidorDB.AnalizadorXML
{

    //public class Nodo : AstNode
    //{
    //    public Object Name;
    //    public List<Nodo> hijos;
    //    public String tipo;
    //    public override void Init(Irony.Ast.AstContext context, Irony.Parsing.ParseTreeNode treeNode)
    //    {
    //        hijos = new List<Nodo>();//inicializamos la lista
    //        base.Init(context, treeNode);
    //        switch ((String)treeNode.Term.Name)
    //        {           //  0 1 2
    //            case "E":// E + E | E - E | E * E  | num
    //                if (treeNode.ChildNodes.Count > 1)
    //                {
    //                    Name = treeNode.ChildNodes[1].FindTokenAndGetText(); //SIGNO
    //                    hijos.Add((Nodo)treeNode.ChildNodes[0].AstNode); // PRIMER VALOR
    //                    hijos.Add((Nodo)treeNode.ChildNodes[2].AstNode); // SEGUNDO VALOR
    //                }
    //                else
    //                {
    //                    Name = treeNode.ChildNodes[0].FindTokenAndGetText(); //OBTENEMOS EL NUMERO
    //                }
    //                break;
    //            case "INTEGER":
    //                Name = treeNode.Token.Value;                    
    //                tipo = treeNode.Token.Terminal.Name;
    //                break;
    //            case "ATRIBUTO":
    //                if (treeNode.ChildNodes.Count > 1)
    //                {
    //                    Name = treeNode.ChildNodes[1].Token.Value; //SIGNO                                               
    //                    tipo = "ATRIBUTO";
    //                    hijos.Add((Nodo)treeNode.ChildNodes[3].AstNode);                                                
    //                }
    //                break;
    //            case "INICIO":
    //                if (treeNode.ChildNodes.Count > 0)
    //                {
    //                    foreach (ParseTreeNode node in treeNode.ChildNodes)
    //                    {
    //                        hijos.Add((Nodo)node.AstNode);
    //                    }
    //                }
    //                break;
    //            case "LATRIBUTOS":
    //                tipo = "ATRIBUTOS";
    //                Name = "ATRIBUTOS";
    //                foreach (ParseTreeNode node in treeNode.ChildNodes)
    //                {
    //                    hijos.Add((Nodo)node.AstNode);
    //                }
    //                break;
    //            case "S_IDENTIFICADOR":
    //                Name = treeNode.Token.Value;
    //                tipo = treeNode.Token.Terminal.Name;
    //                break;
    //            case "CADENA_LITERAL":
    //                Name = treeNode.Token.Value;
    //                tipo = treeNode.Token.Terminal.Name;
    //                break;                    
    //            case "REGISTRO":
    //                Name = "REGISTRO";
    //                tipo = "REGISTRO";
    //                hijos.Add((Nodo)treeNode.ChildNodes[1].AstNode);
    //                hijos.Add((Nodo)treeNode.ChildNodes[3].AstNode);
    //                break;
    //            case "LREGISTRO":
    //                Name = "REGISTROS";
    //                tipo = "REGISTROS";
    //                foreach (ParseTreeNode node in treeNode.ChildNodes)
    //                {
    //                    hijos.Add((Nodo)node.AstNode);
    //                }
    //                break;
    //            case "LROW":
    //                Name = "LROW";
    //                tipo = "LROW";
    //                if (treeNode.ChildNodes.Count == 8)
    //                {
    //                    hijos.Add((Nodo)treeNode.ChildNodes[1].AstNode);
    //                    hijos.Add((Nodo)treeNode.ChildNodes[3].AstNode);
    //                }
    //                if (treeNode.ChildNodes.Count == 9)
    //                {
    //                    hijos.Add((Nodo)treeNode.ChildNodes[1].AstNode);
    //                    hijos.Add((Nodo)treeNode.ChildNodes[3].AstNode);
    //                    hijos.Add((Nodo)treeNode.ChildNodes[4].AstNode);
    //                }
    //                break;
    //            default:
    //                break;
    //        }

    //    }

    //    public static explicit operator Nodo(ParseTreeNode v)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    class XMLGramatica : Grammar
    {
        public XMLGramatica() : base(false)
        {
            CommentTerminal comentarioSimple = new CommentTerminal("comentarioSimple", "#", "\n", "\r\n");
            CommentTerminal comentarioMulti = new CommentTerminal("comentarioMulti", "#*", "*#");

            var FALSO = ToTerm("0");
            var VERDADERO = ToTerm("1");
            RegexBasedTerminal DIGITO = new RegexBasedTerminal("DIGITO", "[0-9]");
            RegexBasedTerminal DOUBLE = new RegexBasedTerminal("DOUBLE", "(-)?[0-9]+\\.[0-9]+");
            RegexBasedTerminal INTEGER = new RegexBasedTerminal("INTEGER", "(-)?[0-9]+");
            RegexBasedTerminal DATE = new RegexBasedTerminal("DATE", "[0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]");
            RegexBasedTerminal HORA = new RegexBasedTerminal("HORA", "[0-9][0-9]:[0-9][0-9]:[0-9][0-9]");
            RegexBasedTerminal NUMERO = new RegexBasedTerminal("NUMERO", "[0-9]+\\.[0-9]{6}");
            RegexBasedTerminal PATH = new RegexBasedTerminal("PATH", @"[a-zA-Z]:([\\][a-zA-Z]([a-zA-Z]|[0-9]|\.|_|-)*)*");
            //+		$exception	{"analizando \"\\G([a-zA-Z]:([\\])*)\" - Conjunto [] sin terminar."}	System.ArgumentException

            RegexBasedTerminal S_IDENTIFICADOR = new RegexBasedTerminal("S_IDENTIFICADOR", "(@)?([a-zA-Z])+(([a-zA-Z])|([0-9])|_|#|$|@|\\.)+");
            // ([a-zA-Z])+([0-9]|[a-zA-Z]|$|_|#)*
            StringLiteral CADENA_LITERAL = new StringLiteral("CADENA_LITERAL", "\"");
            StringLiteral CODIGO = new StringLiteral("CODIGO", "~");


            /*
             	<nombre>bd_clases</nombre>
	<path>C:\DBMS\bd_clases\DB.xml</path>
             */

            var abrir = ToTerm("<");
            var diagonal = ToTerm("/");
            var cerrar = ToTerm(">");
            var KNULO = ToTerm("nulo");
            var KDB = ToTerm("db");
            var KNOMBRE = ToTerm("nombre");
            var KPATH = ToTerm("path");
            var KTABLA = ToTerm("tabla");
            var KROWS = ToTerm("rows");
            var KPERMISO = ToTerm("permiso");
            var KINT = ToTerm("int");
            var KDOUBLE = ToTerm("double");
            var KBOOL = ToTerm("bool");
            var KDATE = ToTerm("date");
            var KDATEHORA = ToTerm("datehora");
            var KAUTO = ToTerm("autoincrementable");
            var KPRIM = ToTerm("primaria");
            var KFOR = ToTerm("foranea");
            var KUNICO = ToTerm("unico");
            var KPROCEDURE = ToTerm("procedure");
            var KOBJECT = ToTerm("object");
            var KOBJ = ToTerm("obj");
            var KATRIB = ToTerm("atr");
            var KSRC = ToTerm("src");
            var KCLAVE = ToTerm("clave");
            var KUSUARIO = ToTerm("usuario");


            NonTerminal
                       ATRIBUTO = new NonTerminal("ATRIBUTO"),
                       K_iden = new NonTerminal("K_iden"),
                       K_idenf = new NonTerminal("K_idenf"),
                       Inicio = new NonTerminal("INICIO"),
                       lista_xml = new NonTerminal("lista_xml"),
                       db = new NonTerminal("Db"),
                       TABLAXML = new NonTerminal("Tablaxml"),
                       proc = new NonTerminal("proc"),
                       obj = new NonTerminal("obj"),
                       objeto = new NonTerminal("objeto"),
                       procedimiento = new NonTerminal("procedimiento"),
                       usuarioxml = new NonTerminal("usuarioxml"),
                       row = new NonTerminal("row"),
                       listareg = new NonTerminal("listareg"),
                       LROWS = new NonTerminal("LROWS"),
                       REGISTRO = new NonTerminal("REGISTRO"),
                       LREGISTRO = new NonTerminal("LREGISTRO"),
                       LATRIBUTOS = new NonTerminal("LATRIBUTOS"),
                       DATOSDB = new NonTerminal("DATOSDB"),
                       MAESTRO = new NonTerminal("LISTADB"),
                       PROPIEDADES = new NonTerminal("PROPIEDADES"),
                       CAMPODEF = new NonTerminal("CAMPODEF"),
                       LCAMPOS = new NonTerminal("LCAMPOS"),
                       POBJECT = new NonTerminal("POBJECT"),// Path archivo object
                       PPROCEDURE = new NonTerminal("PPROCEDURE"),
                       DB = new NonTerminal("DB"),
                       TABLA = new NonTerminal("TABLA"),
                       PFUNCTION = new NonTerminal("FUNCTION"),
                       LTABLA = new NonTerminal("LTABLA"),
                       ROW = new NonTerminal("ROW"),
                       LROW = new NonTerminal("REGISTROS"),
                       CAMPO = new NonTerminal("CAMPO"),
                       VALOR = new NonTerminal("VALOR"),
                       LCAMPO = new NonTerminal("LCAMPO"),
                       PROC = new NonTerminal("PROC"),
                       PROCEDURE = new NonTerminal("PROCEDURE"),
                       OBJ = new NonTerminal("OBJ"),
                       ATRIBUTOS = new NonTerminal("ATRIBUTOS"),
                       OBJETOS = new NonTerminal("OBJETOS"),
                       ARCHIVO = new NonTerminal("ARCHIVO"),
                       ARCHIVOS = new NonTerminal("ARCHIVOS"),
                       FUNCIONES = new NonTerminal("FUNCIONES"),
                       FUNCION = new NonTerminal("FUNCION"),
                       NOBJETO = new NonTerminal("NOBJETO"), // NOMBRE DEL OBJETO
                       LNOBJETO = new NonTerminal("LNOBJETO"), // LISTA DE NOMBRES DE OBJETOS
                       PERDB = new NonTerminal("PERDB"), // Permisos base de datos
                       LPERDB = new NonTerminal("LPERDB"), // Lista de bases de datos para un usuario
                       PERMISO = new NonTerminal("PERMISO"),
                       PERMISOS = new NonTerminal("PERMISOS")
                       ;

            this.Root = ARCHIVO;
            ARCHIVOS.Rule = MakePlusRule(ARCHIVOS, ARCHIVO);
            ARCHIVO.Rule = OBJETOS // Archivo de objetos 
                        | MAESTRO  // Archivo Maestro
                        | LROW // Archivo registros en cada tabla
                        | DB // Archivo de informacion de dbq 
                        | FUNCIONES // Archivo de funciones
                        | PERMISOS // Archivo de usuario 
                        | PROCEDURE
                        ;
            /*
<usuario>
    <nombre> Erick </nombre>
    <base>
        <nombredb>Base1</nombredb>
        <objetos>
            <objeto>objeto1</objeto>
            <objeto>objeto2</objeto>
            <objeto>objeto3</objeto>
        </objetos>
    </base>
    <base>
        <nombredb>Base2</nombredb>
        <objetos>
            <objeto>objeto4</objeto>
            <objeto>objeto5</objeto>
            <objeto>objeto6</objeto>
        </objetos>
    </base>
</usuario>
            */

            #region Archivo de usuarios y permisos
            PERMISOS.Rule = MakePlusRule(PERMISOS, PERMISO);

            PERMISO.Rule =
                abrir + ToTerm("usuario") + cerrar +
                    abrir + ToTerm("nombre") + cerrar +
                        VALOR +
                    abrir + diagonal + ToTerm("nombre") + cerrar +
                    abrir + ToTerm("password") + cerrar +
                        VALOR +
                    abrir + diagonal + ToTerm("password") + cerrar +
                    LPERDB +
                abrir + diagonal + ToTerm("usuario") + cerrar
|
                abrir + ToTerm("usuario") + cerrar +
                    abrir + ToTerm("nombre") + cerrar +
                        VALOR +
                    abrir + diagonal + ToTerm("nombre") + cerrar +
                    abrir + ToTerm("password") + cerrar +
                        VALOR +
                    abrir + diagonal + ToTerm("password") + cerrar +                    
                abrir + diagonal + ToTerm("usuario") + cerrar;

            LPERDB.Rule = MakePlusRule(LPERDB, PERDB);
            PERDB.Rule =
                abrir + ToTerm("base") + cerrar +                   // <base>
                    abrir + ToTerm("nombredb") + cerrar +             //      <objeto>
                    S_IDENTIFICADOR +
                    abrir + diagonal + ToTerm("nombredb") + cerrar+
                    abrir + ToTerm("objetos") + cerrar +             //      <objeto>
                    LNOBJETO +
                    abrir + diagonal + ToTerm("objetos") + cerrar +                    
                abrir + diagonal + ToTerm("base") + cerrar 
                ;


            LNOBJETO.Rule = MakePlusRule(LNOBJETO, NOBJETO);
            NOBJETO.Rule =
                    abrir + ToTerm("objeto") + cerrar +
                        S_IDENTIFICADOR+
                    abrir + diagonal + ToTerm("objeto") + cerrar;
            #endregion

            #region Archivo de funciones
            FUNCIONES.Rule = MakePlusRule(FUNCIONES, FUNCION);
            FUNCION.Rule =
            abrir + ToTerm("func") + cerrar +
                abrir + ToTerm("nombre") + cerrar +
                    VALOR+
                abrir + diagonal + ToTerm("nombre") + cerrar+
                abrir + ToTerm("params") + cerrar +
                    LCAMPO +
                abrir + diagonal + ToTerm("params") + cerrar+
                abrir + ToTerm("src") + cerrar +
                    CODIGO +
                abrir + diagonal + ToTerm("src") + cerrar +
                abrir + ToTerm("tipo") + cerrar +
                    S_IDENTIFICADOR +
                abrir + diagonal + ToTerm("tipo") + cerrar +
            abrir + diagonal + ToTerm("func") + cerrar;

            #endregion



            #region Archivo de objetos
            OBJETOS.Rule = MakePlusRule(OBJETOS, OBJ);

            OBJ.Rule =
                abrir + ToTerm("obj") + cerrar +
                    abrir + ToTerm("nombre") + cerrar +
                        VALOR +
                    abrir + diagonal + ToTerm("nombre") + cerrar +
                    ATRIBUTOS+
                abrir + diagonal + ToTerm("obj") + cerrar;

            ATRIBUTOS.Rule =
                    abrir + ToTerm("attr") + cerrar +
                        LCAMPO+
                    abrir + diagonal + ToTerm("attr") + cerrar;
            #endregion

            #region Archivo de procedimientos
            PROCEDURE.Rule = MakePlusRule(PROCEDURE, PROC);
            PROC.Rule =
                    //Funcion porque tiene tipo de retorno
                    abrir + ToTerm("proc") + cerrar +
                        abrir + ToTerm("nombre") + cerrar +
                            VALOR+
                        abrir + diagonal + ToTerm("nombre") + cerrar+
                        abrir + ToTerm("PARAMS") + cerrar +
                            LCAMPO + //  todos los campos
                        abrir + diagonal + ToTerm("PARAMS") + cerrar +                        
                        abrir + ToTerm("src") + cerrar +
                            CODIGO + 
                        abrir + diagonal + ToTerm("src") + cerrar+
                        abrir + ToTerm("tipo") + cerrar +
                            S_IDENTIFICADOR +
                        abrir + diagonal + ToTerm("tipo") + cerrar +
                    abrir + diagonal + ToTerm("proc") + cerrar
|
                    abrir + ToTerm("proc") + cerrar +
                        abrir + ToTerm("nombre") + cerrar +
                            VALOR +
                        abrir + diagonal + ToTerm("nombre") + cerrar +
                        abrir + ToTerm("PARAMS") + cerrar +
                            //LCAMPO + //  todos los campos
                        abrir + diagonal + ToTerm("PARAMS") + cerrar +
                        abrir + ToTerm("src") + cerrar +
                            CODIGO +
                        abrir + diagonal + ToTerm("src") + cerrar +
                        abrir + ToTerm("tipo") + cerrar +
                            S_IDENTIFICADOR +
                        abrir + diagonal + ToTerm("tipo") + cerrar +
                    abrir + diagonal + ToTerm("proc") + cerrar

|                   // Procedimiento porque no tiene tipo de retorno
                    abrir + ToTerm("proc") + cerrar +
                        abrir + ToTerm("nombre") + cerrar +
                            VALOR +
                        abrir + diagonal + ToTerm("nombre") + cerrar +
                        abrir + ToTerm("PARAMS") + cerrar +
                            LCAMPO + //  todos los campos
                        abrir + diagonal + ToTerm("PARAMS") + cerrar +
                        abrir + ToTerm("src") + cerrar +
                            CODIGO +
                        abrir + diagonal + ToTerm("src") + cerrar +
                    abrir + diagonal + ToTerm("proc") + cerrar
|
                    abrir + ToTerm("proc") + cerrar +
                        abrir + ToTerm("nombre") + cerrar +
                            VALOR +
                        abrir + diagonal + ToTerm("nombre") + cerrar +
                        abrir + ToTerm("PARAMS") + cerrar +
                        //LCAMPO + //  todos los campos
                        abrir + diagonal + ToTerm("PARAMS") + cerrar +
                        abrir + ToTerm("src") + cerrar +
                            CODIGO +
                        abrir + diagonal + ToTerm("src") + cerrar +
                    abrir + diagonal + ToTerm("proc") + cerrar
                    ;

            #endregion

            #region Archivos de cada tabla
            LROW.Rule = MakePlusRule(LROW, ROW);
            
            ROW.Rule =
                        abrir + ToTerm("ROW") + cerrar +
                        LCAMPO+
                        abrir + diagonal + ToTerm("ROW") + cerrar;
            LCAMPO.Rule = MakePlusRule(LCAMPO, CAMPO);
            CAMPO.Rule =
                        abrir + S_IDENTIFICADOR + cerrar +
                        VALOR+
                        abrir + diagonal + S_IDENTIFICADOR + cerrar;
            VALOR.Rule = S_IDENTIFICADOR | INTEGER | CADENA_LITERAL|DATE | DATE+ HORA| DOUBLE;

            #endregion

            #region Archivo DB
            DB.Rule = PPROCEDURE+ // Datos procedimientos
                      PFUNCTION + // Datos funciones
                      POBJECT + // Datos objetos
                      LTABLA;  // Tablas

            PFUNCTION.Rule =
                        abrir + ToTerm("FUNCTION") + cerrar +
                            abrir + ToTerm("PATH") + cerrar +
                            PATH +
                            abrir + diagonal + ToTerm("PATH") + cerrar +
                        abrir + diagonal + ToTerm("FUNCTION") + cerrar;

            PPROCEDURE.Rule =
                        abrir + ToTerm("procedure") + cerrar +
                            abrir + ToTerm("PATH") + cerrar +
                            PATH +
                            abrir + diagonal + ToTerm("PATH") + cerrar +
                        abrir + diagonal + ToTerm("procedure") + cerrar;

            POBJECT.Rule = 
                        abrir + ToTerm("object") + cerrar +
                            abrir + ToTerm("PATH") + cerrar +
                            PATH+
                            abrir + diagonal + ToTerm("PATH") + cerrar+
                        abrir + diagonal + ToTerm("object") + cerrar;

            LTABLA.Rule = MakePlusRule(LTABLA, TABLA);

            TABLA.Rule =
                        abrir + ToTerm("tabla") + cerrar +
                            abrir + ToTerm("nombre") + cerrar +                            
                            S_IDENTIFICADOR+
                            abrir + diagonal + ToTerm("nombre") + cerrar +
                            abrir + ToTerm("PATH") + cerrar +
                            PATH +
                            abrir + diagonal + ToTerm("PATH") + cerrar +
                            LROWS+
                        abrir + diagonal + ToTerm("tabla") + cerrar;            
            
            LROWS.Rule = abrir + ToTerm("ROWS") + cerrar +
                        LCAMPOS+
                        abrir + diagonal + ToTerm("ROWS") + cerrar;


            LCAMPOS.Rule = MakePlusRule(LCAMPOS, CAMPODEF);
            CAMPODEF.Rule = abrir + ToTerm("CAMPO") + cerrar +
                        abrir + S_IDENTIFICADOR + cerrar + S_IDENTIFICADOR +
                        abrir + diagonal + S_IDENTIFICADOR + cerrar +
                        PROPIEDADES +                        
                        abrir + diagonal + ToTerm("CAMPO") + cerrar;
            
            PROPIEDADES.Rule =  abrir + ToTerm("PROPIEDADES") + cerrar+
                                LATRIBUTOS+
                                abrir+ diagonal + ToTerm("PROPIEDADES") + cerrar;

            LATRIBUTOS.Rule = MakePlusRule(LATRIBUTOS, ATRIBUTO);
            ATRIBUTO.Rule =  abrir+ KNULO+ cerrar + VALOR + abrir + diagonal + KNULO  + cerrar
                           | abrir + KPRIM + cerrar + VALOR + abrir + diagonal + KPRIM + cerrar
                           | abrir + KFOR + cerrar + VALOR + abrir + diagonal + KFOR + cerrar
                           | abrir + KFOR + cerrar + VALOR + abrir + diagonal + KFOR + cerrar
                           | abrir + KAUTO + cerrar + VALOR + abrir + diagonal + KAUTO + cerrar
                           | abrir + KUNICO + cerrar + VALOR + abrir + diagonal + KUNICO + cerrar                           
                        ;

            #endregion

            #region  Archivo maestro
            MAESTRO.Rule = MakePlusRule(MAESTRO, DATOSDB);
            DATOSDB.Rule = abrir + KDB + cerrar +
                           abrir + KNOMBRE + cerrar + S_IDENTIFICADOR + abrir + diagonal + KNOMBRE + cerrar +
                           abrir + KPATH + cerrar + PATH + abrir + diagonal + KPATH + cerrar +
                           abrir + diagonal + KDB + cerrar

                          |abrir + KDB + cerrar +
                           abrir + KPATH + cerrar + PATH + abrir + diagonal + KPATH + cerrar +
                           abrir + KNOMBRE + cerrar + S_IDENTIFICADOR + abrir + diagonal + KNOMBRE + cerrar +
                           abrir + diagonal + KDB + cerrar;
            DATOSDB.ErrorRule = SyntaxError + ">";
            #endregion 

            
            MarkPunctuation("<", "/", ">", "db","PROPIEDADES","CAMPO","rows","object", "procedure","tabla","attr",
                "obj","row","ROWS", "PROPIEDADES","nulo","autoincrementable","primaria","foranea"
                ,"params","src", "func", "base","nombredb","objetos","objeto");//para quitar hojas inutiles del arbol
            MarkTransient(LROWS,/* ATRIBUTO, */PROPIEDADES,NOBJETO,ROW);
            NonGrammarTerminals.Add(comentarioSimple); 
            NonGrammarTerminals.Add(comentarioMulti);
            //LanguageFlags = LanguageFlags.CreateAst;

        }

    }
}
