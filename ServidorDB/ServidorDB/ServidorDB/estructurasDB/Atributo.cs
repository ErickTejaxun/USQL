using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    public class Atributo
    {
        public String tipo;
        public String id;
        public Object valor;

        public Atributo(String tipo, String id, Object valor)
        {
            this.tipo = tipo.ToLower();
            this.id = id.ToLower();
            this.valor = valor;
            if (valor == null)
            {
                DateTime today = DateTime.Today;
                switch (tipo.ToLower())
                {
                    case "integer":
                        this.valor = 0;
                        break;
                    case "text":
                        this.valor = "";
                        break;
                    case "double":
                        this.valor = 0.0;
                        break;
                    case "bool":
                        this.valor = 0;
                        break;
                    case "date":
                        this.valor = today.ToString("dd-MM-yyyy");
                        break;
                    case "datetime":
                        this.valor = today.ToString("dd-MM-yyyy hh:mm:ss");
                        break;
                    default:
                        this.valor = null;
                        break;
                }
            }
        }
        public Object getValor()
        {
            return valor;
        }
    }
}
