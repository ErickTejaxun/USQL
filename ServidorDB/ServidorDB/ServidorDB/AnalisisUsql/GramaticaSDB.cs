using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Irony.Parsing;
using Irony.Ast;

namespace ServidorBDD.AnalisisUsql
{
    public class GramaticaSDB : Grammar
    {
        public GramaticaSDB() : base(false)
        {

            CommentTerminal comentario2 = new CommentTerminal("comentario2", "#*", "*#");//acepta comentarios de varias lineas
            CommentTerminal comentario1 = new CommentTerminal("comentario1", "#", "\n", "\r\n");//hacepta comentarios que terminan en una sola linea
            base.NonGrammarTerminals.Add(comentario2);
            base.NonGrammarTerminals.Add(comentario1);

            //tipo de dato}
            StringLiteral tipoText = new StringLiteral("Text", "\"");
            //NumberLiteral tipoInteger = new NumberLiteral("Integer");            
            RegexBasedTerminal tipoInteger = new RegexBasedTerminal("Integer", "[0-9]+");
            RegexBasedTerminal tipoDouble = new RegexBasedTerminal("Double", "[0-9]+[.][0-9]+");
            RegexBasedTerminal tipoDate = new RegexBasedTerminal("Date", "([0-9]){2}-([0-9]){2}-([0-9]){4}");
            RegexBasedTerminal tipoDateTime = new RegexBasedTerminal("DateTime", "([0-9]){2}-([0-9]){2}-([0-9]){4} ([0-9]){2}:([0-9]){2}:([0-9]){2}");

            IdentifierTerminal id = new IdentifierTerminal("id");
            RegexBasedTerminal idProc = new RegexBasedTerminal("iden", "@[a-zA-Z]+([a-zA-Z]|[0-9]|_)*");

            //otros
            RegexBasedTerminal PATH = new RegexBasedTerminal("PATH", @"[a-zA-Z]:([\\][a-zA-Z]([a-zA-Z]|[0-9]|\.|_|-))");



            NonTerminal INICIO = new NonTerminal("INICIO"),
                        TIPODATO = new NonTerminal("TIPODATO"),
                        EXPA = new NonTerminal("EXPA"),
                        EXPR = new NonTerminal("EXPR"),
                        EXPL = new NonTerminal("EXPL"),
                        CREARDB = new NonTerminal("CREARDB"),
                        SENTDDL = new NonTerminal("SENTDDL"),
                        CREARTABLA = new NonTerminal("CREARTABLA"),
                        LCAMPOS = new NonTerminal("LCAMPOS"),
                        CAMPO = new NonTerminal("CAMPO"),
                        COMPLEMENTO = new NonTerminal("COMPLEMENTO"),
                        COMPLEMENTOS = new NonTerminal("COMPLEMENTOS"),
                        CREAROBJETO = new NonTerminal("CREAROBJETO"),
                        LATRIBUTOS = new NonTerminal("LATRIBUTOS"),
                        PROCEDIMIENTO = new NonTerminal("PROCEDIMIENTO"),
                        LPARAMETROS = new NonTerminal("LPARAMETROS"),
                        FUNCION = new NonTerminal("FUNCION"),
                        RETORNO = new NonTerminal("RETORNO"),
                        LLAMADA = new NonTerminal("LLAMADA"),
                        LVALORES = new NonTerminal("LVALORES"),
                        SENTPROC = new NonTerminal("SENTPROC"),
                        SENTSPROC = new NonTerminal("SENTSPROC"),
                        USUARIO = new NonTerminal("USUARIO"),
                        USARDB = new NonTerminal("USAR"),
                        ALTERARTABLA = new NonTerminal("ALTERARTABLA"),
                        ALTERAROBJETO = new NonTerminal("ALTERAROBJETO"),
                        ALTERARUSUARIO = new NonTerminal("ALTERARUSUARIO"),
                        LID = new NonTerminal("LID"),
                        ELIMINAR = new NonTerminal("ELIMINAR"),

                        INSERTAR = new NonTerminal("INSERTAR"),
                        ACTUALIZAR = new NonTerminal("ACTUALIZAR"),
                        BORRAR = new NonTerminal("BORRAR"),
                        SELECCIONAR = new NonTerminal("SELECCIONAR"),
                        COMPSELECCIONAR = new NonTerminal("COMPSELECCIONAR"),

                        PERMISOS = new NonTerminal("PERMISOS"),

                        DECLARAR = new NonTerminal("DECLARACION"),
                        LIDPROC = new NonTerminal("LIDPROC"),
                        ASIGNAROBJ = new NonTerminal("ASIGNAROBJ"),
                        IDACCESO = new NonTerminal("IDACCESO"),
                        ACCESOOBJ = new NonTerminal("ACCESOOBJ"),

                        SENTSI = new NonTerminal("SENTSI"),
                        SENTSELECCIONA = new NonTerminal("SENTSELECCIONA"),
                        CASO = new NonTerminal("CASO"),
                        CASOS = new NonTerminal("CASOS"),
                        DEFECTO = new NonTerminal("DEFECTO"),
                        PARA = new NonTerminal("PARA"),
                        INCREMENTO = new NonTerminal("INCREMENTO"),
                        MIENTRAS = new NonTerminal("MIENTRAS"),
                        DETENER = new NonTerminal("DETENER"),
                        IMPRIMIR = new NonTerminal("IMPRIMIR"),
                        CONTAR = new NonTerminal("CONTAR"),
                        BACKUP = new NonTerminal("BACKUP"),
                        LISTA_DDL = new NonTerminal("LISTADDL"),
                        LISTA_PRC = new NonTerminal("LISTAPROC"),
                        RESTAURARBD = new NonTerminal("RESTAURARBD"),
                        ATRIBUTO = new NonTerminal("ATRIBUTO"),
                        FORANEA = new NonTerminal("FORANEA"),
                        ETABLA = new NonTerminal("ETABLA"),
                        EBASE = new NonTerminal("EBASE"),
                        EOBJETO = new NonTerminal("EOBJETO"),
                        EUSER = new NonTerminal("EUSER"),
                        PARAMETRO = new NonTerminal("PARAMETRO");




            INICIO.Rule = LISTA_DDL;

            LISTA_DDL.Rule = MakeStarRule(LISTA_DDL, SENTDDL);

            EXPA.Rule = EXPA + "+" + EXPA
                        | EXPA + "-" + EXPA
                        | EXPA + "*" + EXPA
                        | EXPA + "/" + EXPA
                        | EXPA + "^" + EXPA
                        | "-" + EXPA
                        | "(" + EXPA + ")"
                        | tipoText
                        | tipoInteger
                        | tipoDouble
                        | tipoDate
                        | tipoDateTime
                        | LLAMADA
                        | IDACCESO
                        | ACCESOOBJ
                        | CONTAR;



            EXPR.Rule = EXPA + "==" + EXPA
                        | EXPA + "!=" + EXPA
                        | EXPA + "<" + EXPA
                        | EXPA + ">" + EXPA
                        | EXPA + "<=" + EXPA
                        | EXPA + ">=" + EXPA
                        | EXPA;

            EXPL.Rule = EXPL + "&&" + EXPL
                        | EXPL + "||" + EXPL
                        | "!" + EXPL
                        | "(" + EXPL + ")"
                        | EXPR;


            TIPODATO.Rule = ToTerm("Text") | ToTerm("Integer") | ToTerm("Double") | ToTerm("Bool") | ToTerm("Date") | ToTerm("DateTime") | id;

            //afuera de proc y func
            SENTDDL.Rule = CREARDB + ToTerm(";")
                          | CREARTABLA + ToTerm(";")
                          | CREAROBJETO + ToTerm(";")
                          | PROCEDIMIENTO
                          | FUNCION
                          | LLAMADA + ToTerm(";")
                          | DECLARAR
                          | ASIGNAROBJ
                          | IMPRIMIR
                          | BACKUP + ToTerm(";")
                          | PERMISOS + ToTerm(";")
                          | RESTAURARBD + ToTerm(";")
                          | USARDB + ToTerm(";")
                          | INSERTAR + ToTerm(";")
                          | ACTUALIZAR + ToTerm(";")
                          | BORRAR + ToTerm(";")
                          | SELECCIONAR + ToTerm(";")
                          | ALTERARTABLA + ToTerm(";")
                          | ALTERAROBJETO + ToTerm(";")
                          | ALTERARUSUARIO + ToTerm(";")
                          | ELIMINAR + ToTerm(";")
                          | USUARIO + ToTerm(";");


            SENTSPROC.Rule = MakeStarRule(SENTSPROC, SENTPROC);

            SENTPROC.Rule = RETORNO
                           | DETENER
                           | SELECCIONAR
                           | SENTSI
                           | SENTSELECCIONA
                           | MIENTRAS
                           | PARA
                           | IMPRIMIR + ToTerm(";")
                           | CREARDB + ToTerm(";")
                           | CREAROBJETO + ToTerm(";")
                           | CREARTABLA + ToTerm(";")
                           | LLAMADA + ToTerm(";")
                           | DECLARAR
                           | ASIGNAROBJ
                           | IMPRIMIR
                           | BACKUP + ToTerm(";")
                           | PERMISOS + ToTerm(";")
                           | RESTAURARBD + ToTerm(";")
                           | USARDB + ToTerm(";")
                           | INSERTAR + ToTerm(";")
                           | ACTUALIZAR + ToTerm(";")
                           | BORRAR + ToTerm(";")
                           | SELECCIONAR + ToTerm(";")
                           | ALTERARTABLA + ToTerm(";")
                           | ALTERAROBJETO + ToTerm(";")
                           | ALTERARUSUARIO + ToTerm(";")
                           | ELIMINAR + ToTerm(";")
                           | USUARIO + ToTerm(";");


            CREARDB.Rule = ToTerm("CREAR") + ToTerm("BASE_DATOS") + id;

            CREARTABLA.Rule = ToTerm("CREAR") + ToTerm("TABLA") + id + ToTerm("(") + LCAMPOS + ToTerm(")");

            LCAMPOS.Rule = MakePlusRule(LCAMPOS, ToTerm(","), CAMPO);

            CAMPO.Rule = TIPODATO + id + COMPLEMENTOS;

            COMPLEMENTOS.Rule = MakeStarRule(COMPLEMENTOS, COMPLEMENTO);

            COMPLEMENTO.Rule = 
                  ToTerm("Nulo") 
                | ToTerm("No Nulo")
                | ToTerm("Autoincrementable")
                | ToTerm("Llave_Primaria")
                | FORANEA
                | ToTerm("Unico");

            FORANEA.Rule = ToTerm("Llave_Foranea") + id + id;

            CREAROBJETO.Rule = ToTerm("Crear") + ToTerm("Objeto") + id + ToTerm("(") + LATRIBUTOS + ToTerm(")");

            LATRIBUTOS.Rule = MakeStarRule(LATRIBUTOS, ToTerm(","), ATRIBUTO);

            ATRIBUTO.Rule = TIPODATO + id;


            PROCEDIMIENTO.Rule = ToTerm("Crear") + ToTerm("Procedimiento") + id + ToTerm("(") + LPARAMETROS + ToTerm(")") + ToTerm("{") + SENTSPROC + ToTerm("}");

            LPARAMETROS.Rule = MakeStarRule(LPARAMETROS, ToTerm(","), PARAMETRO);
            PARAMETRO.Rule = TIPODATO + idProc;

            FUNCION.Rule = ToTerm("Crear") + ToTerm("Funcion") + id + ToTerm("(") + LPARAMETROS + ToTerm(")") + TIPODATO + ToTerm("{") + SENTSPROC + ToTerm("}");

            RETORNO.Rule = ToTerm("Retorno") + EXPL + ToTerm(";");

            LLAMADA.Rule = id + ToTerm("(") + LVALORES + ToTerm(")");

            LVALORES.Rule = MakeStarRule(LVALORES, ToTerm(","), EXPL);

            USUARIO.Rule = ToTerm("Crear") + ToTerm("Usuario") + id + ToTerm("Colocar") + ToTerm("Password") + ToTerm("=") + tipoText;

            USARDB.Rule = ToTerm("USAR") + id;

            ALTERARTABLA.Rule = ToTerm("Alterar") + ToTerm("Tabla") + id + ToTerm("Agregar") + ToTerm("(") + LCAMPOS + ToTerm(")")
                               | ToTerm("Alterar") + ToTerm("Tabla") + id + ToTerm("Quitar") + LID;
            LID.Rule = MakePlusRule(LID, ToTerm(","), IDACCESO);

            ALTERAROBJETO.Rule = ToTerm("Alterar") + ToTerm("Objeto") + id + ToTerm("Agregar") + ToTerm("(") + LATRIBUTOS + ToTerm(")")
                                | ToTerm("Alterar") + ToTerm("Objeto") + id + ToTerm("Quitar") + LID;

            ALTERARUSUARIO.Rule = ToTerm("Alterar") + ToTerm("Usuario") + id + ToTerm("Cambiar") + ToTerm("Password") + ToTerm("=") + tipoText;

            ELIMINAR.Rule =
                        ETABLA | EBASE | EOBJETO | EUSER;
            ETABLA.Rule = ToTerm("Eliminar") + ToTerm("Tabla") + id;
            EBASE.Rule = ToTerm("Eliminar") + ToTerm("Base_Datos") + id;
            EOBJETO.Rule = ToTerm("Eliminar") + ToTerm("Objeto") + id;
            EUSER.Rule = ToTerm("Eliminar") + ToTerm("User") + id; ;

            //sentencias DML
            INSERTAR.Rule = ToTerm("Insertar") + ToTerm("En") + ToTerm("Tabla") + id + ToTerm("(") + LID + ToTerm(")") + ToTerm("Valores") + ToTerm("(") + LVALORES + ToTerm(")")
                           | ToTerm("Insertar") + ToTerm("En") + ToTerm("Tabla") + id + ToTerm("(") + LVALORES + ToTerm(")");

            ACTUALIZAR.Rule = ToTerm("Actualizar") + ToTerm("Tabla") + id + ToTerm("(") + LID + ToTerm(")") + ToTerm("Valores") + ToTerm("(") + LVALORES + ToTerm(")") + ToTerm("Donde") + EXPL
                            | ToTerm("Actualizar") + ToTerm("Tabla") + id + ToTerm("(") + LID + ToTerm(")") + ToTerm("Valores") + ToTerm("(") + LVALORES + ToTerm(")");

            BORRAR.Rule = ToTerm("Borrar") + ToTerm("En") + ToTerm("Tabla") + id + ToTerm("Donde") + EXPL
                           | ToTerm("Borrar") + ToTerm("En") + ToTerm("Tabla") + id
                            ;

            SELECCIONAR.Rule = ToTerm("Seleccionar") + LID + ToTerm("De") + LID + COMPSELECCIONAR
                              | ToTerm("Seleccionar") + ToTerm("*") + ToTerm("De") + LID + COMPSELECCIONAR;

            COMPSELECCIONAR.Rule = ToTerm("Donde") + EXPL
                                  | ToTerm("Donde") + EXPL + ToTerm("Ordenar_Por") + IDACCESO + ToTerm("ASC")
                                  | ToTerm("Donde") + EXPL + ToTerm("Ordenar_Por") + IDACCESO + ToTerm("DESC")
                                  | ToTerm("Ordenar_Por") + IDACCESO + ToTerm("ASC")
                                  | ToTerm("Ordenar_Por") + IDACCESO + ToTerm("DESC")
                                  | Empty;

            CONTAR.Rule = ToTerm("Contar") + ToTerm("(") + ToTerm("<<") + SELECCIONAR + ToTerm(">>") + ToTerm(")");
            //sentencias DCL
            PERMISOS.Rule = ToTerm("Otorgar") + ToTerm("Permisos") + id + ToTerm(",") + id + "." + id
                          | ToTerm("Otorgar") + ToTerm("Permisos") + id + ToTerm(",") + id + "." + ToTerm("*")
                          | ToTerm("Denegar") + ToTerm("Permisos") + id + ToTerm(",") + id + "." + id
                          | ToTerm("Denegar") + ToTerm("Permisos") + id + ToTerm(",") + id + "." + ToTerm("*");

            //Sentencias SSL
            DECLARAR.Rule = ToTerm("Declarar") + LIDPROC + TIPODATO + ToTerm(";")
                           | ToTerm("Declarar") + LIDPROC + TIPODATO + ToTerm("=") + EXPL + ToTerm(";");
            LIDPROC.Rule = MakePlusRule(LIDPROC, ToTerm(","), idProc);

            ASIGNAROBJ.Rule = ACCESOOBJ + ToTerm("=") + EXPL + ToTerm(";");

            ACCESOOBJ.Rule = ToTerm("@") + IDACCESO;

            IDACCESO.Rule = MakePlusRule(IDACCESO, ToTerm("."), id);

            SENTSI.Rule = ToTerm("Si") + ToTerm("(") + EXPL + ToTerm(")") + ToTerm("{") + SENTSPROC + ToTerm("}")
                         | ToTerm("Si") + ToTerm("(") + EXPL + ToTerm(")") + ToTerm("{") + SENTSPROC + ToTerm("}") + ToTerm("Sino") + ToTerm("{") + SENTSPROC + ToTerm("}");

            SENTSELECCIONA.Rule = ToTerm("Selecciona") + ToTerm("(") + EXPL + ToTerm(")") + ToTerm("{") + CASOS + ToTerm("}")
                                 | ToTerm("Selecciona") + ToTerm("(") + EXPL + ToTerm(")") + ToTerm("{") + CASOS + DEFECTO + ToTerm("}");

            CASOS.Rule = MakeStarRule(CASOS, CASO);
            CASO.Rule = ToTerm("Caso") + EXPL + ToTerm(":") + SENTSPROC;
            DEFECTO.Rule = ToTerm("Defecto") + ToTerm(":") + SENTSPROC;

            PARA.Rule = ToTerm("Para") + ToTerm("(") + DECLARAR + EXPL + ToTerm(";") + INCREMENTO + ToTerm(")") + ToTerm("{") + SENTSPROC + ToTerm("}");

            INCREMENTO.Rule = ToTerm("++")
                | ToTerm("--");
            DETENER.Rule = ToTerm("Detener") + ToTerm(";");

            MIENTRAS.Rule = ToTerm("Mientras") + ToTerm("(") + EXPL + ToTerm(")") + ToTerm("{") + SENTSPROC + ToTerm("}");

            IMPRIMIR.Rule = ToTerm("Imprimir") + ToTerm("(") + EXPL + ToTerm(")") + ToTerm(";");

            BACKUP.Rule = ToTerm("Backup") + ToTerm("usqldump") + id + id //+
                        | ToTerm("Backup") + ToTerm("Completo") + id + id;// + ToTerm(";");

            RESTAURARBD.Rule = ToTerm("Restaurar") + ToTerm("usqldump") + tipoText
                        | ToTerm("Restaurar") + ToTerm("Completo") + tipoText;




            this.Root = INICIO;

            #region "Asociatividad"

            RegisterOperators(1, Associativity.Left, "+", "-");
            RegisterOperators(2, Associativity.Left, "*", "/");
            RegisterOperators(3, Associativity.Right, "^");
            RegisterOperators(5, "==", "!=", "<", ">", "<=", ">=");
            RegisterOperators(6, Associativity.Left, "||");
            RegisterOperators(7, Associativity.Left, "&&");
            RegisterOperators(8, Associativity.Left, "!");
            RegisterOperators(9, Associativity.Left, "(", ")");
            #endregion


            #region "Eliminacion de Nodos"
            //---------------------> Eliminacion de caracters, no terminales
            this.MarkPunctuation("(", ")", ";", ":", "{", "}");
            this.MarkPunctuation("imprimir", "si","sino", "mientras", "caso", "defecto");
            this.MarkPunctuation("crear", "objeto", "procedimiento", "funcion", "retorno", "usuario", "usar");
            this.MarkPunctuation("alterar", "tabla", "objeto", "usuario", "eliminar", "insertar", "en", "tabla");
            this.MarkPunctuation("insertar", "valores", "borrar", "seleccionar", "permisos", "declarar");
            this.MarkPunctuation("selecciona", "caso", "defecto", "para", "detener", "mientras", "backup", "restaurar", "contar", "<<", ">>", "De", "@", "=",",");
            this.MarkPunctuation("selecciona", "caso", "defecto", "para", "detener", "mientras", "backup", "restaurar", "contar", "<<", ">>", "De", "@", "=",",");
            this.MarkPunctuation("donde", "ordenar_por","Llave_Foranea");
            this.MarkTransient(SENTDDL, SENTPROC,COMPLEMENTO, SENTDDL/*, LPARAMETROS*/);
            //this.MarkTransient(CASO);

            #endregion


        }



    }
}