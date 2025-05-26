using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adeposx.report.Models
{
    public class DTOTransactionDetail
    {
        public DTOTransactionDetail()
        {

        }
        public long DetailTransactionGenericId { get; set; }

        public long TransactionGenericId { get; set; }

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
        public decimal Subtotal { get; set; }


        public decimal Total { get; set; }

        public int NumOrder { get; set; }

        public bool HasIventory { get; set; }

    }
}
