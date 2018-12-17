using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    public class defTabla
    {
        public String nombre;
        public String path;
        public List<defCampo> listaCampos;

        public defTabla(String nombre, String path)
        {
            this.nombre = nombre;
            this.path = path;
            this.listaCampos = new List<defCampo>();
        }

        public void insertarCampo(defCampo campo)
        {
            listaCampos.Add(campo);
        }

        public defCampo getCampo(String nombre)
        {
            foreach (defCampo camp in this.listaCampos)
            {
                if (camp.nombre.Equals(nombre))
                {
                    return camp;
                }
            }
            return null;
        }
    
    }
}
