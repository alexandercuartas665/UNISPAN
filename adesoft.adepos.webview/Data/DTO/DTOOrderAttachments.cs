using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOOrderAttachments
    {
        public string UploadOrderId { get; set; }

        public long OrderId { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string DataBase64 { get; set; }

        public int Counter { get; set; }
    }
}
