using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adesoft.adeposx.report.Models.ElectronicBilling
{
    public class DTOOPInvoiced
    {
        public string Id { get; set; }

        public string OPNum { get; set; }

        public DateTime Date { get; set; }

        public string CustomerNum { get; set; }

        public string CustomerName { get; set; }

        public string WorkNo { get; set; }

        public string WorkName { get; set; }

        public decimal NetAmountValue { get; set; }

        public decimal TotalInvoiceAmount { get; set; }

        public string AdminId { get; set; }

        public string AdminName { get; set; }

        public DateTime ConfirmedDate { get; set; }

        public string Note { get; set; }

        public bool Confirmed { get; set; }

        public string Period { get; set; }

        public string EBNumber { get; set; }

        public decimal TotalOutstandingBalance { get; set; }

        public decimal TotalPendingAmountBalance { get; set; }

        public decimal TotalInvoiceAmountBalance { get; set; }

        public int QtyPending { get; set; }

        public int QtyInvoiced { get; set; }

        public string ZoneId { get; set; }

        public string ZoneName { get; set; }

        public string ZoneParentId { get; set; }

        public string ZoneParentName { get; set; }
    }
}
