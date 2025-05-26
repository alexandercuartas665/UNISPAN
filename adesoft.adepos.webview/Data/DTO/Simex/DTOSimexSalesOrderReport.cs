using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO.Simex
{
    public class DTOSimexSalesOrderReport
    {
        public string GroupBy1 { get; set; }

        public string GroupBy1Label { get; set; }

        public string GroupBy2 { get; set; }

        public string GroupBy2Label { get; set; }

        public string GroupBy3 { get; set; }

        public string GroupBy3Label { get; set; }

        public string GroupBy4 { get; set; }

        public string GroupBy4Label { get; set; }

        public string GroupBy5 { get; set; }

        public string GroupBy5Label { get; set; }

        public string GroupBy6 { get; set; }

        public string GroupBy6Label { get; set; }

        public string Year { get; set; }

        public string Month { get; set; }

        public int Mth { get; set; }

        public long QtyOrdered { get; set; }

        public decimal TotalAmountLine { get; set; }

        public decimal UnitPrice { get; set; }

        public long QtyPendingReceived { get; set; }

        public long QtyPendingInvoiced { get; set; }

        public decimal AmountPendingRecived { get; set; }

        public decimal AmountPendingInvoiced { get; set; }
    }

    public class DTOSimexSalesOrderFilter
    {
        public DTOSimexSalesOrderFilter()
        {
            this.MultipleValuesYear = new List<long>();
            this.MultipleValuesMonth = new List<long>();
            this.GroupBy = new List<string>();
        }

        public string GuidFilter { get; set; }

        public long TypeReportId { get; set; }

        public DateTime? DateInit { get; set; }

        public DateTime? DateEnd { get; set; }

        public IEnumerable<long> MultipleValuesYear { get; set; }

        public IEnumerable<long> MultipleValuesMonth { get; set; }

        public IEnumerable<string> GroupBy { get; set; }
    }
}
