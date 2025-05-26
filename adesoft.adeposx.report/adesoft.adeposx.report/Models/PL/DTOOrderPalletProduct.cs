using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adesoft.adeposx.report.Models.PL
{
    public class DTOOrderPalletProduct
    {
        public string OrderPalletProductId { get; set; }

        public string OrderPalletId { get; set; }

        public long ItemId { get; set; }

        public string Description { get; set; }

        public decimal Qty { get; set; }

        public decimal Weight { get; set; }

        public string Reference { get; set; }

        public decimal SelectedQty { get; set; }

        public decimal PendingQty { get; set; }


    }
}
