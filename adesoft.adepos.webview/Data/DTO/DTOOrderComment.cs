using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOOrderComment
    {
        public int OrderCommentId { get; set; }

        public long OrderId { get; set; }

        public OrderType OrderType { get; set; }

        public DateTime CreatedDatetTime { get; set; }

        public string Comment { get; set; }
    }
}
