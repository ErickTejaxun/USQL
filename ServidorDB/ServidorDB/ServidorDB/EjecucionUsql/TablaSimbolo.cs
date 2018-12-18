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

        public TablaSimbolo()
        {
            this.simbolos = new List<Simbolo>();
        }

        public Boolean setSimbolo(Simbolo s)
        {
            if (getSimbolo(s.nombre) == null)
            {
                simbolos.Add(s);
                return true;
            }

            return false;
        }

        public Simbolo getSimbolo(String nombre)
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

    }
}
