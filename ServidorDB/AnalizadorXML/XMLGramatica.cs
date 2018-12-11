using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace ServidorDB.AnalizadorXML
{
    class XMLGramatica : Grammar
    {
        public XMLGramatica() : base(false)
        {
            CommentTerminal comentarioSimple = new CommentTerminal("comentarioSimple", "#", "\n", "\r\n");
            CommentTerminal comentarioMulti = new CommentTerminal("comentarioMulti", "#*", "*#");

            var FALSO = ToTerm("0");
            var VERDADERO = ToTerm("1");
            RegexBasedTerminal DIGITO = new RegexBasedTerminal("DIGITO", "[0-9]");
            RegexBasedTerminal FLOAT = new RegexBasedTerminal("FLOAT", "[0-9]+.[0-9]+");
            RegexBasedTerminal INTEGER = new RegexBasedTerminal("INTEGER", "[0-9]+");
            RegexBasedTerminal DATE = new RegexBasedTerminal("DATE", "[0-9][0-9]-[0-9][0-9]-[0-9][0-9] [0-9][0-9]");
            RegexBasedTerminal HORA = new RegexBasedTerminal("HORA", "[0-9][0-9]:[0-9][0-9]:[0-9][0-9]");
            RegexBasedTerminal NUMERO = new RegexBasedTerminal("NUMERO", "[0-9]+\\.[0-9]{6}");

            RegexBasedTerminal S_IDENTIFICADOR = new RegexBasedTerminal("(@)([a-zA-Z])+ ([0-9] | [a-zA-Z] |($|_|#))*");

            var K_db =  "<db>";
            var   K_dbf= "</db>";
            var   K_nombre= "<nombre>";
            var   K_nombref= "</nombre>";
            var   K_path= "<path>";
            var   K_pathf= "</path>";
            var   K_seek= "<seek>";
            var   K_seekf= "</seek>";
            var   K_tabla= "<tabla>";
            var   K_tablaf= "</tabla>";
            var   K_permiso= "<permiso>";
            var   K_permisof= "</permiso>";
            var   K_rows= "<rows>";
            var   K_rowsf= "</rows>";
            var   K_int= "<int>";
            var   K_intf= "</int>";
            var   K_doble= "<doble>";
            var   K_doblef= "</doble>";
            var   K_texto= "<texto>";
            var   K_textof= "</texto>";
            var   K_bool= "<bool>";
            var   K_boolf= "</bool>";
            var   K_date= "<date>";
            var   K_datef= "</date>";
            var   K_datehora= "<datehora>";
            var   K_datehoraf= "</datehora>";
            var   K_nulo= "<nulo>";
            var   K_nulof= "</nulo>";
            var   K_auto= "<auto>";
            var   K_autof= "</auto>";
            var   K_prim= "<prim>";
            var   K_primf= "</prim>";
            var   K_for= "<for>";
            var   K_forf= "</for>";
            var   K_unico= "<unico>";
            var   K_unicof= "</unico>";
            var   K_procedure= "<procedure>";
            var   K_proceduref= "</procedure>";
            var   K_object= "<object>";
            var   K_objectf= "</object>";
            var   K_obj= "<obj>";
            var   K_objf= "</obj>";
            var   K_atr= "<atr>";
            var   K_atrf= "</atr>";
            var   K_proc= "<proc>";
            var   K_procf= "</proc>";
            var   K_src= "<src>";
            var   K_srcf= "</src>";
            var   K_clave= "<clave>";
            var   K_clavef= "</clave>";
            var   K_usuario= "<usuario>";
            var   K_usuariof= "</usuario>";

            StringLiteral CADENA_LITERAL = new StringLiteral("CADENA_LITERAL", "\"");

            //var   K_iden= "<" + <S_IDENTIFICADOR> + ">";
            //var   K_idenf= "</" +<S_IDENTIFICADOR> +">";





            NonTerminal 
                       ATRIBUTO = new NonTerminal("ATRIBUTO"),
                       K_iden = new NonTerminal("K_iden"),
                       K_idenf = new NonTerminal("K_idenf"),
                       Inicio = new NonTerminal("Inicio"),
                       lista_xml = new NonTerminal("lista_xml"),
                       db = new NonTerminal("Db"),
                       Tablaxml = new NonTerminal("Tablaxml"),
                       proc = new NonTerminal("proc"),
                       obj = new NonTerminal("obj"),
                       objeto = new NonTerminal("objeto"),
                       procedimiento = new NonTerminal("procedimiento"),
                       usuarioxml = new NonTerminal("usuarioxml"),
                       row = new NonTerminal("row"),
                       reg = new NonTerminal("reg"),
                       listareg = new NonTerminal("listareg"),
                       lista_row = new NonTerminal("lista_row");

            Inicio.Rule = lista_xml;

            lista_xml.Rule = db
                            | Tablaxml
                            | proc
                            | obj
                            | objeto
                            | procedimiento
                            | usuarioxml
                            | row
                            ;


            row.Rule = "<rows>" + listareg + "</rows>";

            listareg.Rule = listareg + reg
                            | reg;

            usuarioxml.Rule = "<usuario>" +"<nombre>" + S_IDENTIFICADOR + "</nombre>" + "<seek>" + INTEGER + "</seek>" + "<clave>" + CADENA_LITERAL + "</clave>" + "</usuario>";


            procedimiento.Rule = "<proc>" + "<seek>" + INTEGER + "</seek>" +  "<nombre>" + S_IDENTIFICADOR + 
                                    "</nombre>" + "<permiso>" + CADENA_LITERAL + "</permiso>" + "<atr>" + lista_row + 
                                    "</atr>" + "<src>" + CADENA_LITERAL + "</src>" + "</proc>"
                                    
                                    | "<proc>" + "<seek>" + INTEGER + "</seek>" + "<nombre>" + S_IDENTIFICADOR +
                                    "</nombre>" + "<permiso>" + CADENA_LITERAL + "</permiso>" + "<atr>" +
                                    "</atr>" + "<src>" + CADENA_LITERAL + "</src>" + "</proc>";

            K_iden.Rule = "<" + S_IDENTIFICADOR + ">";
            K_idenf.Rule = "</" + S_IDENTIFICADOR + ">";

            ATRIBUTO.Rule = "<nulo>" + INTEGER + "</nulo>"
                           | "<prim>" + INTEGER + "</prim>"
                           | "<for>" + FLOAT + "</for>"
                           | "<auto>" + INTEGER + "</auto>"
                           | "<unico>" + INTEGER + "</unico>"
                        ;

            this.Root = Inicio;


            //this.MarkTransient(jvalue, jarrayBr, jobjectBr);
        }

    }
}
