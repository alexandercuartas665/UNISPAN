using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class ItemKit
    {
        [Key, Column(Order = 0)]
        public long ItemFatherId { get; set; }
        [Key, Column(Order = 1)]
        public long ItemId { get; set; } //Item hijo
        public Item Item { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cant { get; set; }
    }
}
