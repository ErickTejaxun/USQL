using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    public class Usuario
    {
        public String username;
        public String password;
        public List<Permiso> permisos;

        public Usuario(String username, String password)
        {
            this.username = username;
            this.password = password;
            this.permisos = new List<Permiso>();
        }

        public String cambiarPassword(String passActual , String newPass)
        {
            if (passActual.Equals(password))
            {
                password = newPass;
                return "1";
            }
            else
            {
                return "Erro, el password ingresado no coincide con el actual. No se puede modificar el password.";
            }
        }
    }
}
