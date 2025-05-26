using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO.Simex
{
    public class DTOSimexCarteraReport
    {
        public DTOSimexCarteraReport()
        {
            this.GroupBy1 = string.Empty;
            this.GroupBy1Label = string.Empty;
            this.GroupBy2 = string.Empty;
            this.GroupBy2Label = string.Empty;
            this.GroupBy3 = string.Empty;
            this.GroupBy2Label = string.Empty;
            this.GroupBy4 = string.Empty;
            this.GroupBy4Label = string.Empty;
            this.GroupBy5 = string.Empty;
            this.GroupBy5Label = string.Empty;
            this.Reference = string.Empty;
        }

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

        public DateTime DocumentDate { get; set; }

        public DateTime PaymentDateEstimated { get; set; }

        public string Reference { get; set; }

        public decimal AmountBalance { get; set; }

        public decimal Current { get; set; }

        public decimal Days1To30 { get; set; }

        public decimal Days31To60 { get; set; }

        public decimal Days61To90 { get; set; }

        public decimal More90 { get; set; }

        public int ExpirationDays { get; set; }

        public string Range { get; set; }
    }

    public class DTOSimexCarteraFilter
    {
        public DTOSimexCarteraFilter()
        {
            this.TypeReportId = 2;
            this.FilterById = "Range";
            this.RangeBy = new List<string>()
            {
                "Current",
                "1-30", 
                "31-60", 
                "61-90",
                "+90"
            };
            this.ToDate = DateTime.Now;
            this.GroupBy = new List<string>()
            {
                "Range"
            };
        }

        public string GuidFilter { get; set; }

        public long TypeReportId { get; set; }

        public string SearchBy { get; set; }

        public string FilterById { get; set; }

        public IEnumerable<string> GroupBy { get; set; }

        public IEnumerable<string> RangeBy { get; set; }

        public int Days { get; set; }

        public DateTime ToDate { get; set; }
    }

    public class DTORangeBy
    {
        public string RangeById { get; set; }

        public string Description { get; set; }

    }
}
