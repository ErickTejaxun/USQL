using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Irony.Parsing;

namespace ServidorDB.Ejecucion
{
    public class Simbolo
    {
        public String id;
        public Object valor;
    }

    class Entorno
    {
        private Hashtable tabla;
        protected Entorno ant;

        public Entorno(Entorno padre)
        {
            tabla = new Hashtable();
            ant = padre;
        }

        public bool put(String s, Simbolo sim)
        {
            Simbolo aux = buscarSimbolo(s);
            if (aux == null)
            {
                tabla.Add(s, sim);
                return true;
            }
            return false;
        }

        public Simbolo get(String s)
        {
            for (Entorno e = this; e != null; e = e.ant)
            {
                Simbolo encontro = (Simbolo)(e.tabla[s]);
                if (encontro != null)
                {
                    return encontro;
                }
            }
            return null;
        }

        public void actualizarSimbolo(String s, Object valor)
        {
            for (Entorno e = this; e != null; e = e.ant)
            {
                Simbolo encontro = (Simbolo)(e.tabla[s]);
                if (encontro != null)
                {
                    encontro.valor = valor;
                }
            }
        }

        public Simbolo buscarSimbolo(String id)
        {
            //Simbolo sim = tablaGlobal.get(nodo.getValue());
            Simbolo sim = (Simbolo)tabla[id];
            if (sim == null)
            {
                errorNoDeclarada(id);
            }
            return sim;
        }


        public void errorNoDeclarada(String nombre)
        {
            Console.WriteLine("Error Semantico, no se ha declarado la variable " + nombre);
        }

        public Boolean existeSimbolo(String id)
        {

            Simbolo sim = this.get(id);
            if (sim == null)
            {
                errorNoDeclarada(id);
                return false;
            }
            return true;
        }
    }
}
