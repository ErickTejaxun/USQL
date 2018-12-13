using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServidorDB.estructurasDB
{
    class Tabla
    {
        public String nombre;
        public String path;
        public List<tupla> tuplas;
        public List<defCampo> definiciones;
        public String mensajes;

        public Tabla(String nombre, String path)
        {
            this.nombre = nombre;
            this.path = path;
            this.tuplas = new List<tupla>();
            this.definiciones = new List<defCampo>();
        }

        public void addCampoATupla(tupla tup, campo cmp)
        {
            if (integridadCampo(cmp))
            {
                tup.addCampo(cmp);
            }
        }


        public void addDefinicion(defCampo definicion)
        {
            definiciones.Add(definicion);
        }

        public void addTupla(tupla registro)
        {
            foreach (campo cmp in registro.campos)
            {
                integridadCampo(cmp);

            }
            tuplas.Add(registro);
        }

        public bool integridadCampo(campo cmp)
        {
            foreach (defCampo definicion in definiciones)
            {               
                if (definicion.nombre.Equals(cmp.id))
                {
                    String tipo = cmp.valor.GetType().ToString().ToLower();
                    switch (definicion.tipo)
                    {
                        case "text":
                            if (tipo.Equals("string"))
                            {
                                return true;
                            }
                            mensajes = mensajes +  "\n"+"Error: El campo "+cmp.id + " es de tipo text y se ha intentado ingresar un " + tipo;
                            return false;
                        case "integer":
                            if (tipo.Equals("int"))
                            {
                                return true;
                            }
                            mensajes = mensajes +  "\n"+"Error: El campo " + cmp.id + " es de tipo integer y se ha intentado ingresar un " + tipo ;
                            return false;
                        case "double":
                            if (tipo.Equals("double"))
                            {
                                return true;
                            }
                            mensajes = mensajes +  "\n"+"Error: El campo " + cmp.id + " es de tipo double y se ha intentado ingresar un " + tipo ;
                            return false;
                        case "bool":                            
                            if (cmp.valor.GetType().ToString().ToLower().Equals("bool"))
                            {
                                return true;
                            }
                            mensajes = mensajes +  "\n"+"Error: El campo " + cmp.id + " es de tipo bool y se ha intentado ingresar un " + tipo ;
                            return false;
                        case "date":
                            if (cmp.valor.GetType().ToString().ToLower().Equals("string"))
                            {
                                if (comprobarFecha((String)cmp.valor))
                                {
                                    return true;
                                }
                                else
                                {
                                    mensajes = mensajes +  "\n"+"Error: El campo " + cmp.id + " es de tipo date y se ha intentado ingresar un " + tipo ;
                                    return false;
                                }
                            }
                            break;
                        case "datetime":
                            if (cmp.valor.GetType().ToString().ToLower().Equals("string"))
                            {
                                if (comprobarFecha((String)cmp.valor))
                                {
                                    return true;
                                }
                                else
                                {
                                    mensajes = mensajes +  "\n"+"Error: El campo " + cmp.id + " es de tipo datetime y se ha intentado ingresar un " + tipo ;
                                    return false;
                                }
                            }
                            break;
                    }
                }
            }
            return false;
        }

        public bool comprobarFecha(String fecha)
        {
            try
            {                
                CultureInfo provider = CultureInfo.InvariantCulture;               
                DateTime fechaF = DateTime.ParseExact(fecha, "MM-dd-yyyy", provider);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }


        }

    }
}
