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
                    Form1.Mensajes.Add("Base de datos --" + nombreDB +"-- seleccionada.");
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
    }
}
