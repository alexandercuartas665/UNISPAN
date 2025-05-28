using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOOrderAttachment
    {
        public long OrderId { get; set; }
        public OrderType OrderType { get; set; }
       
        public string FileName { get; set; }
        
        public byte[] FileBytes { get; set; }
    }
}
