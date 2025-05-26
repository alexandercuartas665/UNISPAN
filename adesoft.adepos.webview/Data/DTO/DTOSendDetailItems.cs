using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOSendDetailItems
    {
        public DTOSendDetailItems()
        {

        }

        public long code { get; set; }
        public long itemId { get; set; }
        public long consecutive { get; set; }
        public long count { get; set; }

        public string url_photo1 { get; set; }
        public string observations { get; set; }
    }
}
