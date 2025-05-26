using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.PL
{
    public class OrderUploadControl
    {
        [Key]
        public string Id { get; set; }

        public int OrderId { get; set; }

        public OrderType OrderType { get; set; }

        public DateTime SyncDateTime { get; set; }

        public int ImagesPending { get; set; }

        public int ImagesUploaded { get; set; }
    }
}
