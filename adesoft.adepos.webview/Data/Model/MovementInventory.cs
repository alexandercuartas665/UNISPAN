using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class MovementInventory
    {
        [Key]
        public long MovementInventoryId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantNow { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantMov { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantNet { get; set; }

        public long TransactionId { get; set; }

        public long TypeTransactionId { get; set; }

        public long ItemId { get; set; }

        public Item Item { get; set; }

        public DateTime DateRecord { get; set; }

        public long WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; }

        public bool LastRecord { get; set; }
    }
}
