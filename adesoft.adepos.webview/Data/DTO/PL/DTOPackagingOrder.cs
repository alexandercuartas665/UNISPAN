using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOPackagingOrder
    {
        public long OrderId { get; set; }

        public string OrderNum { get; set; }

        public OrderType OrderType { get; set; }

        public string Works { get; set; }

        public DateTime DispatchDateTime { get; set; }
    }
}
