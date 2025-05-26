using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class SnapshotInventoryQuantify
    {
        public SnapshotInventoryQuantify()
        {

        }
       [Key]
        public long SnapshotInventoryQuantifyId { get; set; }
        public string SyncodeItem { get; set; }
        /// <summary>
        /// 1 palmira , 0 bogota
        /// </summary>
        public bool ReceiveRequireInspection { get; set; }
        /// <summary>
        /// 0 palmira , 1 bogota ... sospecho q es asi para bodega arriendo
        /// </summary>
        public bool HideZeroInvoiceItems { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CantInven { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantRent { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantInvenTon { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantRentTon { get; set; }
        public int YearInve { get; set; }
        public int MonthInve { get; set; }
        public DateTime DateInventary { get; set; }
    }
}
