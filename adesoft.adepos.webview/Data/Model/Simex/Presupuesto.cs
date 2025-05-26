using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.Simex
{
    public class Presupuesto
    {
        public Presupuesto()
        {
            this.ZoneId = string.Empty;
            this.CategoryId = string.Empty;
        }

        public long YearId { get; set; }

        public long MonthId { get; set; }

        public string CategoryId { get; set; }

        public string ZoneId { get; set; }

        public decimal Value { get; set; }

    }
}
