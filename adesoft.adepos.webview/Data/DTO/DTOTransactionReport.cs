using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOTransactionReport
    {
        //encabezado
        public long TransactionGenericId { get; set; }

        /// <summary>
        /// Cliente o proovedor
        /// </summary>
        public string TerceroName { get; set; }

        public string AuxTerceroName { get; set; }

        /// <summary>
        /// Documento
        /// </summary>
        public string TerceroDoc { get; set; }
        public long Consecutive { get; set; }

        public string ConsecutiveChar { get; set; }

        public DateTime DateInit { get; set; }

        public DateTime DateEnd { get; set; }

        public string MethodPayment { get; set; }

        public string PaymentCondition { get; set; }

        public DateTime DatePayInit { get; set; }

        public decimal TotalBuy { get; set; }

        public decimal TotalCost { get; set; }

        public decimal Cash { get; set; }

        public decimal Change { get; set; }

        public decimal TotalTax { get; set; }

        public decimal TotalDiscount { get; set; }
        public decimal TotalOtherDiscount { get; set; }
        public decimal HSubtotal { get; set; }

        //DETALLE
        public long ItemId { get; set; }
        public string ItemBarcode { get; set; }
        public string ItemDescription { get; set; }
        /// <summary>
        /// Valor unitario Costo
        /// </summary>
        public decimal PriceCost { get; set; }
        /// <summary>
        /// Valor unitario sin impuestos
        /// </summary>
        public decimal PriceUnd { get; set; }
        public decimal Cant { get; set; }
        /// <summary>
        /// Valor totl impuestos
        /// </summary>
        public decimal Tax { get; set; }
        /// <summary>
        /// Valor total de descuento
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// Cant * Valor base unitario sin  impuestos
        /// </summary>
        public decimal DSubtotal { get; set; }
        public decimal Total { get; set; }
        public int NumOrder { get; set; }

        public decimal OtherDiscount { get; set; }

        public long MermaId { get; set; }
        public string TypeMerma { get; set; }

        public string Description { get; set; }

        public decimal ValueMerma { get; set; }

        public decimal TCostXItem { get; set; }

        public long TransOption { get; set; }

        public long Warehouseid { get; set; }

        #region Distpatch
        public decimal SaldoInventaryAfter { get; set; }
        public decimal SaldoInventaryBefore { get; set; }
        public decimal InventarioPendiente { get; set; }

        public long TurnId { get; set; }

        public string DocumentExtern { get; set; }

        public decimal StockActual { get; set; }

        public bool FilterOnlyPendient { get; set; }

        public string Note { get; set; }

        public string NameWork { get; set; }

        #endregion
    }
}
