using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.Simex
{
    public class SalesOrder
    {
        public string SalesId { get; set; }

        public DateTime DocumentDate { get; set; }

        public string Operation1 { get; set; }

        public string Operation3 { get; set; }

        public string Customer { get; set; }

        public string Operation4 { get; set; }

        public string Operation2 { get; set; }

        public int Warehouse { get; set; }

        public string ItemId { get; set; }

        public string ItemName { get; set; }

        public long QtyOrdered { get; set; }

        public decimal TotalAmountLine { get; set; }

        public decimal UnitPrice { get; set; }

        public long QtyPendingReceived{ get; set; }

        public long QtyPendingInvoiced { get; set; }

        public decimal AmountPendingRecived { get; set; }

        public decimal AmountPendingInvoiced { get; set; }

        public bool Modified { get; set; }

        public string CountryRegionId { get; set; }

    }
}
