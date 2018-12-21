using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.EjecucionUsql
{
    public class TablaSimbolo
    {
        private List<Simbolo> simbolos;
        public TablaSimbolo anterior;
        public string ambito = "global";
        public TablaSimbolo()
        {
            this.simbolos = new List<Simbolo>();
        }

        //busquedas
        public Boolean setSimbolo2(Simbolo s)
        {
            if (getSimbolo2(s.nombre) == null)
            {
                simbolos.Add(s);
                return true;
            }

            return false;
        }

        //busquedas
        public Boolean setSimbolo3(Simbolo s)
        {
            if (getSimbolo3(s.nombre) == null)
            {
                simbolos.Add(s);
                return true;
            }

            return false;
        }

        public Simbolo getSimbolox(String nombre)
        {
            foreach (Simbolo s in simbolos)
            {
                if (s.nombre.Equals(nombre.ToLower()))
                {
                    return s;
                }
            }

            return null;
        }

        public Simbolo getSimbolo2(String nombre)
        {

            nombre = nombre.ToLower();
            for (TablaSimbolo t = this; t != null; t = t.anterior)
            {
                foreach (Simbolo s in t.simbolos)
                {
                    if (nombre.Equals(s.nombre.ToLower()))
                    {
                        return s;
                    }
                }
            }
            return null;
        }

        public Simbolo getSimbolo3(String nombre)
        {

            nombre = nombre.ToLower();
            for (TablaSimbolo t = this; t.anterior != null; t = t.anterior)
            {
                foreach (Simbolo s in t.simbolos)
                {
                    if (nombre.Equals(s.nombre.ToLower()))
                    {
                        return s;
                    }
                }
            }
            return null;
        }

        public void cambiarAmbito(TablaSimbolo actual)
        {
            foreach (Simbolo s in actual.simbolos)
            {
                setSimbolo2(s);
            }

        }

        public Boolean setSimboloId(Simbolo s)
        {
            if (getSimbolo2(s.id) == null)
            {
                simbolos.Add(s);
                return true;
            }

            return false;
        }

        public Simbolo getSimboloId(String id)
        {
            foreach (Simbolo s in simbolos)
            {
                if (s.id.Equals(id.ToLower()))
                {
                    return s;
                }
            }

            return null;
        }


    }
}
