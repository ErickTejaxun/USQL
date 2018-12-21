using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServidorDB.estructurasDB;
using ServidorDB.EjecucionUsql;
using ServidorDB;

namespace ServidorBDD.EjecucionUsql
{


    public class Interprete
    {

        //nuevas
        public static TablaSimbolo tabla;
        public static TablaSimbolo metodos;
        public static int nivel;
        public Logica opL;
        public Interprete(ParseTreeNode raiz)
        {
            metodos = new TablaSimbolo();
            guardarMetodos(raiz);
            tabla = new TablaSimbolo();
            nivel = 0;
        }

        #region guardarMetodos
        public void guardarMetodos(ParseTreeNode raiz)
        {
            foreach (ParseTreeNode hijo in raiz.ChildNodes)
            {
                if (hijo.Term.Name.Equals("PROCEDIMIENTO"))
                {
                    Simbolo procedimiento = new Simbolo("", hijo);
                    Boolean estado = metodos.setSimboloId(procedimiento);
                    if (!estado)
                    {
                        agregarError("Semantico", "El procedimiento " + hijo.ChildNodes[0].Token.Text + " ya existe", hijo.Span.Location.Line, hijo.Span.Location.Column);
                    }
                }
                else if (hijo.Term.Name.Equals("FUNCION"))
                {
                    Simbolo funcion = new Simbolo(hijo.ChildNodes[2].ChildNodes[0].Token.Text, hijo);
                    Boolean estado = metodos.setSimboloId(funcion);
                    if (!estado)
                    {
                        agregarError("Semantico", "La funcion " + hijo.ChildNodes[0].Token.Text + " ya existe", hijo.Span.Location.Line, hijo.Span.Location.Column);
                    }
                }
            }

        }
        #endregion fin guardarMetodos

        public Resultado ejecutar(ParseTreeNode raiz)
        {
            Resultado resultado = null;
            foreach (ParseTreeNode hijo in raiz.ChildNodes)
            {
                if (resultado != null)
                {
                    return resultado;
                }
                String tipoAccion = hijo.Term.Name;
                switch (tipoAccion)
                {
                    case "CREARDB":
                        Form1.sistemaArchivos.crearBase(hijo);
                        break;
                    case "CREARTABLA":
                        Form1.sistemaArchivos.crearTabla(hijo);
                        break;
                    case "DECLARACION"://jose
                        Declaracion declaracion = new Declaracion();
                        Boolean estado = declaracion.declaracion(hijo);
                        break;
                    case "SELECCIONAR":
                        Form1.sistemaArchivos.realizarConsulta(hijo);
                        break;
                    case "RESTAURARBD":
                        Form1.sistemaArchivos.restaurar(hijo);
                        break;
                    case "BACKUP":
                        Form1.sistemaArchivos.backup(hijo);
                        break;
                    case "USAR":
                        Form1.sistemaArchivos.setBaseActual(hijo.ChildNodes[0]);
                        break;
                    case "IMPRIMIR":
                        opL = new Logica();
                        Form1.Mensajes.Add(opL.operar(hijo.ChildNodes[0]).valor + "");
                        break;
                    case "USUARIO":
                        Form1.sistemaArchivos.crearUsuario(hijo);
                        break;
                    case "ACTUALIZAR":
                        Form1.sistemaArchivos.actualizar(hijo);
                        break;
                    case "ALTERARTABLA":
                        Form1.sistemaArchivos.alterarTabla(hijo);
                        break;
                    case "ALTERAROBJETO":
                        Form1.sistemaArchivos.alterarObjeto(hijo);
                        break;
                    case "ALTERARUSUARIO":
                        Form1.sistemaArchivos.alterarUsuario(hijo);
                        break;
                    case "BORRAR": // Borrar registro en la tupla
                        Form1.sistemaArchivos.borrar(hijo);
                        break;
                    case "INSERTAR":
                        Form1.sistemaArchivos.insertar(hijo);
                        break;
                    case "PERMISOS":
                        Form1.sistemaArchivos.permisos(hijo);
                        break;
                    case "ELIMINAR":
                        Form1.sistemaArchivos.eliminar(hijo);
                        break;
                    case "CREAROBJETO"://jose
                        String nombreObjeto = hijo.ChildNodes[0].Token.Text;
                        ParseTreeNode atributos = hijo.ChildNodes[1];
                        Objeto objeto = new Objeto(nombreObjeto);
                        foreach (ParseTreeNode nodoAtributo in atributos.ChildNodes)
                        {

                            Atributo atributo = new Atributo(nodoAtributo.ChildNodes[0].ChildNodes[0].Token.Text, nodoAtributo.ChildNodes[1].Token.Text, null);
                            objeto.addAtributo(atributo);

                        }
                        Form1.sistemaArchivos.getBase().agregarObjeto(objeto, hijo.Span.Location.Line, hijo.Span.Location.Column);
                        break;
                    case "ASIGNAROBJ":
                        Asignacion asignacion = new Asignacion();
                        asignacion.asignar(hijo);
                        break;
                    case "LLAMADA":
                        nivel++;
                        Llamada llamada = new Llamada(this);
                        Resultado r = llamada.ejecutar(hijo);
                        nivel--;
                        break;
                    case "RETORNO":
                        opL = new Logica();
                        resultado = opL.operar(hijo.ChildNodes[0]);
                        return resultado;
                    case "SENTSI":
                        Si si = new Si(this);
                        resultado = si.ejecutar(hijo);
                        break;
                    case "MIENTRAS":
                        Mientras mientras = new Mientras(this);
                        resultado = mientras.ejecutar(hijo);
                        break;
                    case "DETENER":
                        resultado = new Resultado("Error", null);
                        resultado.detener = true;
                        return resultado;
                    case "PARA":
                        Para para = new Para(this);
                        resultado = para.ejecutar(hijo);
                        break;
                    case "SENTSELECCIONA":
                        Selecciona selecciona = new Selecciona(this);
                        resultado = selecciona.ejecutar(hijo);
                        break;
                }
            }
            return resultado;
        }

        #region Area ejecucion

        private void agregarError(String tipo, String descripcion, int linea, int columna)
        {
            Error error = new Error(tipo, descripcion, linea, columna);
            Form1.errores.Add(error);
            Form1.Mensajes.Add(error.getMensaje());
        }

        #endregion fin area ejecucion
    }
}
