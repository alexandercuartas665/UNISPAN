using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.PL
{
    public class OrderProduct
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; }

        public long OrderId { get; set; }

        public long ItemId { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal Weight { get; set; }

        public decimal Qty { get; set; }

        public decimal QtyPending { get; set; }

        [StringLength(60)]
        public string Reference { get; set; }

        [StringLength(36)]
        public string? ZoneProductId { get; set; }

        public ZoneProduct? ZoneProduct { get; set; }
    }
}
