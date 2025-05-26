using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.Simex
{
    public class InventSum
    {
        public string InventLocationId { get; set; }

        public string ItemId { get; set; }

        public string UnitId { get; set; }

        public string ItemName { get; set; }

        public decimal QtyOnHand { get; set; }
    }
}
