using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    public class tupla : ICloneable
    {
        public List<campo> campos;

        public tupla()
        {
            campos = new List<campo>();
        }
        public campo getCampo(String id)
        {
            String nombre = id.ToLower();
            if (nombre.ToLower().Contains("."))
            {
                String[] partes = nombre.Split('.');
                id = partes[1];
            }

            foreach (campo camp in campos)
            {
                String nombre2 = camp.id;
                if (nombre2.ToLower().Contains("."))
                {
                    String[] partes = nombre2.Split('.');
                    nombre2 = partes[1].ToLower();
                }
                if (nombre2.ToLower().Equals(id.ToLower()))
                {
                    return camp;
                }
            }
            return null;
        }
        public void addCampo(campo camp)
        {
            campos.Add(camp);
        }
        public bool actualizarCampo(campo camp, String id)
        {
            foreach (campo cmp in campos)
            {
                if (cmp.id.Equals(id))
                {
                    campos.Remove(cmp);
                    campos.Add(camp);
                }
            }
            return false;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
