using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    class SistemaArchivos
    {
        public List<BD> basesdedatos;
        public List<Usuario> usuarios;

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
    }
}
