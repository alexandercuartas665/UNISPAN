using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOOrderPallet
    {
        public string OrderPalletId { get; set; }

        public long OrderId { get; set; }

        public int PalletNo { get; set; }

        public string QRData { get; set; }

        public OrderPalletStatus Status { get; set; }

        public DTOZoneProduct ZoneProduct { get; set; }

        public string ZoneProductId { get; set; }

        public string ZoneName { get; set; }

        public string DispatchDate { get; set; }

        public string DispatchTime { get; set; }

        public string Works { get; set; }

        public string OrderNum { get; set; }

        public string Module { get; set; }

        public ICollection<DTOOrderPalletProduct> OrderPalletProducts { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }
    }    
}
