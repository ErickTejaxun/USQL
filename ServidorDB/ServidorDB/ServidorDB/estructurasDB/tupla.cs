using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    class tupla
    {
        public List<campo> campos;

        public tupla()
        {
            campos = new List<campo>();
        }
        public campo getCampo(String id)
        {
            foreach (campo camp in campos)
            {
                if (camp.id.Equals(id))
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
    }
}
