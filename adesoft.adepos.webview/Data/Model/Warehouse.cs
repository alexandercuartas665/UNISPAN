using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public struct CodWareouse
    {
        public static long NA = 1;
        public static long PRINCIPAL = 2;
    }

    public class Warehouse : BaseEntity
    {

        public Warehouse()
        {

        }
        [Key]
        public long WarehouseId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public bool IsConsignataria { get; set; }

        public long QuantifyId { get; set; }
    }
}
