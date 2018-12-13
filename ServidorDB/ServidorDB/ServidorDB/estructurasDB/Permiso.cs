using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    class Permiso
    {
        public String nombreDB;
        public List<String> listaObjetos;

        public Permiso(String nombre)
        {
            this.nombreDB = nombre;
            this.listaObjetos = new List<String>();
        }

        public void addPermisoObjeto(String nombreObjeto)
        {
            listaObjetos.Add(nombreObjeto);
        }

        public bool tengoPermiso(String nombreObjeto)
        {
            String obj = null;
            foreach (String nombre in listaObjetos)
            {
                if (nombre.Equals(nombreObjeto))
                {
                    return true;
                }
            }
            if (obj == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
