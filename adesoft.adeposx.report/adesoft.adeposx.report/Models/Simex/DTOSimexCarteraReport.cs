using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models.Simex
{
    public class DTOSimexCarteraReport
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
    }
}