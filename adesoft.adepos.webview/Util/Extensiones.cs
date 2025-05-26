using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Util
{
    public static class Extensiones
    {

        /// <summary>
        /// Clona una clase List
        /// </summary>
        public static List<T> Clone<T>(this List<T> lista)
        {
            return lista.ToList();
        }


    }
}
