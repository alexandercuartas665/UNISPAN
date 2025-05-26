using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOOrderProduct
    {
        public DTOOrderProduct()
        {
            //this.Description = string.Empty;
        }

        public string OrderProductId { get; set; }

        public long OrderId { get; set; }

        public long ItemId { get; set; }

        public string Description { get; set; }

        public decimal Weight { get; set; }

        public decimal Qty { get; set; }

        public decimal LastQty { get; set; }        

        public decimal QtyPending { get; set; }

        public string Reference { get; set; }

        public string? ZoneProductId { get; set; }

        public DTOZoneProduct? ZoneProduct { get; set; }

        public int Version { get; set; }

        public string VersionCode { get; set; }

        public string LastVersionCode { get; set; }
    }
}
