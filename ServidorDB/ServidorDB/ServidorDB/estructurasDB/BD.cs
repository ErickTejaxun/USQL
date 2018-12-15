using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace ServidorDB.estructurasDB
{
    class BD
    {
        public String nombre;
        public String path;
        public List<Tabla> tablas;
        public List<Objeto> objetos;
        public List<Procedimiento> procedimientos;        

        public BD(String nombre, String path)
        {
            this.nombre = nombre;
            this.path = path;
            this.tablas = new List<Tabla>();
            this.objetos = new List<Objeto>();
            this.procedimientos = new List<Procedimiento>();
        }

        public String seleccionar(List<String> listaCampos, List<String> listaTablas, ParseTreeNode raiz)
        {            
            String data = "\n";
            List<tupla> cartesianoTemporal = new List<tupla>();
            List<tupla> cartesiano = new List<tupla>();
            foreach (String ntab in listaTablas)
            {
                cartesianoTemporal = productoCartesiano(cartesianoTemporal, buscarTabla(ntab) , ntab);                
            }
            foreach (tupla tp in cartesianoTemporal)
            {
                if (comprobarCondicion(tp, raiz))
                {
                    cartesiano.Add(tp);
                }
            }

            #region imprimir resultado
            foreach (campo cp in cartesiano[0].campos)
            {
                if (data.Equals("\n"))
                {
                    data = cp.id;
                }
                else
                {
                    data = data + "," + cp.id;
                }
            }
            data = data + "\n";
            foreach (tupla tpm in cartesiano)
            {
                bool flag = false;
                foreach (campo cmp in tpm.campos)
                {
                    if (!flag)
                    {
                        data = data + cmp.valor.ToString();
                        flag = true;
                    }
                    else
                    {
                        data = data + "," + cmp.valor.ToString();
                    }                    
                }
                data = data + "\n";
            }
            #endregion

            return data;
        }

        public Boolean comprobarCondicion(tupla tup, ParseTreeNode raiz)
        {
            return true;
        }


        public List<tupla> productoCartesiano(List<tupla> tab1, List<tupla> tab2 , String nombre)
        {
            List<tupla> tablaCar = new List<tupla>();
            if (tab1.Count >0 && tab2.Count > 0)
            {
                foreach (tupla tp in tab2)
                {
                    foreach (campo cp in tp.campos)
                    {
                        cp.id = nombre + "." + cp.id;
                    }
                }

                var cartesiano = from campo1 in tab1
                                 from campo2 in tab2
                                 select new { campo1, campo2 };
                foreach (var data in cartesiano)
                {
                    tupla tpm = new tupla();
                    foreach (campo cmp in data.campo1.campos)
                    {                        
                        tpm.addCampo(cmp);
                    }
                    foreach (campo cmp in data.campo2.campos)
                    {                        
                        tpm.addCampo(cmp); 
                    }
                    tablaCar.Add(tpm);
                }
            }
            else
            {
                foreach (tupla tp in tab2)
                {
                    foreach (campo cp in tp.campos)
                    {
                        cp.id = nombre +"."+ cp.id;
                    }
                }
                return tab2;
            }
            return tablaCar;
        }

        public List<tupla> buscarTabla(String id)
        {
            foreach (Tabla tab in tablas)
            {
                if (tab.nombre.Equals(id))
                {
                    return tab.tuplas;
                }
            }
            return null;
        }
    }

}
