using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOTransaction
    {

        public DTOTransaction()
        {
            ListItems = new List<DTOInventary>();
        }
        public long TransactionGenericId { get; set; }

        public long Consecutive { get; set; }

        public string ConsecutiveChar { get; set; }

        public DateTime DateInit { get; set; }

        public DateTime DateEnd { get; set; }

        public string MethodPayment { get; set; }

        public string PaymentCondition { get; set; }

        public DateTime DatePayInit { get; set; }
        /// <summary>
        /// Proovedor , Cliente
        /// </summary>
        public long TerceroId { get; set; }

        public long VendedorId { get; set; }

        public long TypeTransactionId { get; set; }

        public string TypeTransaction { get; set; }


        public List<DTOTransactionDetail> Details { get; set; }
        /// <summary>
        /// Total Compra
        /// </summary>
        public decimal TotalBuy { get; set; }

        public decimal Cash { get; set; }

        public decimal Change { get; set; }

        public decimal TotalTax { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal Subtotal { get; set; }

        public long WarehouseOriginId { get; set; }

        public long WarehouseDestinId { get; set; }

        public string SDateInit { get; set; }

        public string SDateEnd { get; set; }

        public string AuxTest { get; set; }

        public bool TransactionIsOk { get; set; }

        public string Message { get; set; }

        public List<DTOInventary> ListItems { get; set; }
    }
}
