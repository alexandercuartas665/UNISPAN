using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class DetailTransactionGeneric : BaseEntity
    {
        public DetailTransactionGeneric()
        {

        }
        [Key]
        public long DetailTransactionGenericId { get; set; }

        public long TransactionGenericId { get; set; }

        public long ItemId { get; set; }

        public Item Item { get; set; }

        /// <summary>
        /// Valor unitario Costo
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceCost { get; set; }
        /// <summary>
        /// Valor unitario sin impuestos
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceUnd { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cant { get; set; }
        /// <summary>
        /// Valor totl impuestos
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; }
        /// <summary>
        /// Valor total de descuento
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }
        /// <summary>
        /// Otros descuentos .. aqui puede aplicar la merma
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal OtherDiscount { get; set; }
        /// <summary>
        /// Cant * Valor base unitario sin  impuestos
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]

        public decimal Total { get; set; }

        /// <summary>
        /// saldos inventario despues de aplicar el descuento
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoInventaryAfter { get; set; }

        /// <summary>
        /// saldos inventario antes de aplicar el descuento
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoInventaryBefore { get; set; }
        /// <summary>
        /// Faltante para cumplir
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal InventarioPendiente { get; set; }
        /// <summary>
        /// Es materia prima
        /// </summary>
        public bool IsRawMaterial { get; set; }

        [NotMapped]
        public int NumOrder { get; set; }

        [NotMapped]
        public bool HasIventory { get; set; }

        [NotMapped]
        public decimal Auxnum { get; set; }
        [NotMapped]
        public bool ChanguedView { get; set; }

        public string ObservationAuditory { get; set; }

        //[NotMapped]
        //public OrderManufacturing OrderManufacturing { get; set; }

        [StringLength(1)]
        public string TransactionState { get; set; }
    }
}
