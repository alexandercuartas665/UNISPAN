using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    /// <summary>
    /// Inventario X Transaccion y Item
    /// </summary>
    public class InventoryXTransaction
    {
        [Key]
        public long InventoryXTransactionId { get; set; }

        public long ItemId { get; set; }

        /// <summary>
        /// Cantidad que entra
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantEntry { get; set; }
        /// <summary>
        /// Cantidad que sale
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]

        public decimal CantOut { get; set; }

        /// <summary>
        /// Cantidad existente
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]

        public decimal CantNet { get; set; }

        public DateTime DateTransaction { get; set; }
        /// <summary>
        /// Tipo de transacion
        /// </summary>
        public long TransactionId { get; set; }

        public long TypeTransactionId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Costo { get; set; }
        
        public long WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; }
    }
}
