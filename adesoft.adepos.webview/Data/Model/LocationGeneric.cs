using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public struct TypesLocations
    {
        public static string EMPRESA = "EMPRESA";
        public static string AREA = "AREA";
        public static string AREAHM = "AREAHM";
        public static string SUCURSAL = "SUCURSAL";
        public static string EPS = "EPS";
        public static string ARL = "ARL";
        public static string AFP = "AFP";
        public static string AFC = "AFC";
        public static string CAJA = "CAJA";
        public static string CARGO = "CARGO";
        public static string CARGOHM = "CARGOHM";
    }


    public class LocationGeneric : BaseEntity
    {
        [Key]
        public long LocationGenericId { get; set; }
        public string Description { get; set; }
        public string SyncCode { get; set; }

        /// <summary>
        /// EMPRESA,AREA,SUCURSAL,CARGO
        /// </summary>
        public string TypeLocation { get; set; }

        public string LongDescription { get; set; }

        public bool IsPlanta { get; set; }

        public string ChartDescription { get; set; }


    }
}
