using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    class BD
    {
        public String nombre;
        public String path;
        public List<Tabla> tablas;
        public List<Objeto> objetos;
        public List<Procedimiento> procedimientos;        

        public BD(String nombre, String path)
        {
            this.nombre = nombre;
            this.path = path;
            this.tablas = new List<Tabla>();
            this.objetos = new List<Objeto>();
            this.procedimientos = new List<Procedimiento>();
        }
    }
}
