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
        public Tabla(String nombre, String path)
        {
            this.nombre = nombre;
            this.path = path;
            this.tuplas = new List<tupla>();
            this.definiciones = new List<defCampo>();
        }
        public void addCampoATupla(tupla tup, campo cmp, Form1 form)
        {
            if (integridadCampo(cmp,form))
            {
                tup.addCampo(cmp);
            }
        }
        public void addDefinicion(defCampo definicion)
        {
            definiciones.Add(definicion);
        }
        public void addTupla(tupla registro, Form1 form)
        {
            foreach (campo cmp in registro.campos)
            {
                integridadCampo(cmp, form);

            }
            tuplas.Add(registro);
        }
        public bool integridadCampo(campo cmp, Form1 formActual)
        {
            foreach (defCampo definicion in definiciones)
            {               
                if (definicion.nombre.Equals(cmp.id))
                {
                    //String tipo = cmp.id.GetType().ToString().ToLower();
                    String tipo = cmp.valor.GetType().ToString();
                    switch (definicion.tipo.ToLower())
                    {
                        case "text":
                            if (cmp.valor is String)
                            {
                                return true;
                            }
                            formActual.imprimirSalida("Error: El campo "+cmp.id + " es de tipo text y se ha intentado ingresar un " + tipo);
                            return false;
                        case "int":
                            if (cmp.valor is Int32)
                            {
                                return true;
                            }
                            formActual.imprimirSalida("Error: El campo " + cmp.id + " es de tipo text y se ha intentado ingresar un " + tipo);
                            return false;
                        case "double":
                            if (cmp.valor is Double)
                            {
                                return true;
                            }
                            formActual.imprimirSalida("Error: El campo " + cmp.id + " es de tipo text y se ha intentado ingresar un " + tipo);
                            return false;
                        case "bool":                            
                            if (cmp.valor is Int32)
                            {
                                if ((int)cmp.valor == 0 || (int)cmp.valor == 1)
                                {
                                    return true;
                                }
                                else { return false; }
                                
                            }
                            formActual.imprimirSalida("Error: El campo " + cmp.id + " es de tipo text y se ha intentado ingresar un " + tipo);
                            return false;
                        case "date":
                            if (cmp.valor is DateTime)
                            {
                                return true;
                            }
                            else
                            {
                                formActual.imprimirSalida("Error: El campo " + cmp.id + " es de tipo text y se ha intentado ingresar un " + tipo);
                                return false;
                            }                            
                        case "datetime":
                            if (cmp.valor is DateTime)
                            {
                                return true;
                            }
                            else
                            {
                                formActual.imprimirSalida("Error: El campo " + cmp.id + " es de tipo text y se ha intentado ingresar un " + tipo);
                                return false;
                            }
                    }
                    formActual.imprimirSalida("Error: El campo " + cmp.id + " es de tipo text y se ha intentado ingresar un " + tipo);
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
