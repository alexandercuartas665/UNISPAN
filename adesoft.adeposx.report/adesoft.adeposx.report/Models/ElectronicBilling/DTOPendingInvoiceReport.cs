using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adesoft.adeposx.report.Models.ElectronicBilling
{
    public class DTOPendingInvoiceReport
    {
        public string GroupBy1Key { get; set; }

        public string GroupBy1Value { get; set; }

        public string GroupBy1Label { get; set; }

        public string GroupBy2Key { get; set; }

        public string GroupBy2Value { get; set; }

        public string GroupBy2Label { get; set; }

        public string GroupBy3Key { get; set; }

        public string GroupBy3Value { get; set; }

        public string GroupBy3Label { get; set; }

        public string GroupBy4Key { get; set; }

        public string GroupBy4Value { get; set; }

        public string GroupBy4Label { get; set; }

        public string GroupBy5Key { get; set; }

        public string GroupBy5Value { get; set; }

        public string GroupBy5Label { get; set; }

        public string GroupBy6Key { get; set; }

        public string GroupBy6Value { get; set; }

        public string GroupBy6Label { get; set; }

        public decimal TotalNetAmount { get; set; }

        public decimal TotalInvoiceAmount { get; set; }

        public decimal TotalPendingAmount { get; set; }

        public decimal TotalPendingAmountBalance { get; set; }

        public decimal TotalInvoiceAmountBalance { get; set; }

        public decimal PrevBalanceOutstanding { get; set; }

        public decimal ActualBalanceOutstanding { get; set; }

        public int QtyProvision { get; set; }

        public int QtyPending { get; set; }

        public int QtyInvoiced { get; set; }

        public decimal PercPending { get; set; }

        public decimal PercInvoiced { get; set; }

        public decimal TotalAmountCancelled { get; set; }

        /*Parameters*/

        public string ReportName { get; set; }

        public string PeriodTitle { get; set; }

        public int ShowBalance { get; set; }
    }
}
