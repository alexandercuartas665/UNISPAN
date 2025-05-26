using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.PL
{
    public class ZoneProduct
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; }

        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(60)]
        public string AltName { get; set; }

        public ICollection<OrderPallet> OrderPallets { get; set; }

        public ICollection<Item> Items { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }

        public ICollection<OrderProductLog> OrderProductLogs { get; set; }

        public ICollection<OrderPalletProduct> OrderPalletProducts { get; set; }

        public ICollection<UserApp> Users { get; set; }
    }
}
