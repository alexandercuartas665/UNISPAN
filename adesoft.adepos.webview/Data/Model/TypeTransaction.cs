using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{

    public struct CodTypeTransaction
    {
        public static long FACTURAVENTA = 1;
        public static long PEDIDOVENTA = 2;
        public static long ORDENCOMPRA = 3;
        public static long ENTRADADIRECTA = 4;
        public static long CREDITOCLIENTE = 5;
        public static long FACTURACOMPRA = 6;
        public static long INGRESO = 7;
        public static long GATOS = 8;
        public static long ORDENTRABAJO = 9;
        public static long VENTAPUENTE = 10;
        public static long ORDENDESPACHO = 11;
    }
    public class TypeTransaction
    {

        [Key]
        public long TypeTransactionId { get; set; }

        public string Name { get; set; }

        public string Codigo { get; set; }

        public long ConecutiveInit { get; set; }

        public long ConecutiveEnd { get; set; }
    }
}
