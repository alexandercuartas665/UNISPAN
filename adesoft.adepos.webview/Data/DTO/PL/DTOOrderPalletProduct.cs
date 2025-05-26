using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOOrderPalletProduct
    {
        public DTOOrderPalletProduct()
        {

        }

        public DTOOrderPalletProduct(DTOOrderPallet orderPallet)
        {
            this.OrderPallet = orderPallet;
        }

        public string OrderPalletProductId { get; set; }

        public string OrderPalletId { get; set; }        

        public long ItemId { get; set; }

        public string Description { get; set; }

        public decimal Qty { get; set; }

        public decimal Weight { get; set; }

        public string Reference { get; set; }

        public decimal SelectedQty { get; set; }

        public decimal PendingQty { get; set; }

        public bool Selected { get; set; }

        public string ZoneProductId { get; set; }

        public long OrderId { get; set; }

        public DTOZoneProduct ZoneProduct { get; set; }

        public DTOOrderPallet OrderPallet { get; set; }
    }
}
