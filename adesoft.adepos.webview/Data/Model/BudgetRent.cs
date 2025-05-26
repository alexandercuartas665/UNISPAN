using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class BudgetRent
    {
        public long Id { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public string CategoryId { get; set; }

        public decimal AmountBudget { get; set; }

        public string CommonDataId { get; set; }

        public string CommonDataName { get; set; }

        public string ZoneId { get; set; }

        public string ZoneName { get; set; }
    }
}
