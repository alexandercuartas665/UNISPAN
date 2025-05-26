using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.PL
{
    public class OrderPallet
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; }

        public long OrderId { get; set; }

        public int PalletNo { get; set; }  

        public string QRData { get; set; }

        public ZoneProduct ZoneProduct { get; set; }

        public string ZoneProductId { get; set; }

        public OrderPalletStatus Status { get; set; }

        public ICollection<OrderPalletProduct> OrderPalletProducts { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public int Version { get; set; }
    }

    public enum OrderPalletStatus
    {
        None,
        Syncronized,
        Closed,
        Deleted,
        VersionChange
    }    
}
