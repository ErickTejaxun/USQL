using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    class Parametro
    {
        public String nombre;
        public String tipo;

        public Parametro(String nombre, String tipo)
        {
            this.nombre = nombre;
            this.tipo = tipo;
        }
    }

    class Procedimiento
    {
        public String nombre;
        public String tipoRetorno;
        public String codigoFuente;
        public List<Parametro> listaParametros;

        public Procedimiento(String nombre, String tipoRetorno)
        {
            this.nombre = nombre;
            this.tipoRetorno = tipoRetorno;
            listaParametros = new List<Parametro>();
        }
    }
}
