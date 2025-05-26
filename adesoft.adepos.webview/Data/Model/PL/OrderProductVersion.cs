using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace adesoft.adepos.webview.Data.Model.PL
{
    public class OrderProductVersion
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; }
        
        public long OrderId { get; set; }

        public int Version { get; set; }

        [StringLength(20)]
        public string VersionCode { get; set; }

        public DateTime CreatedOn { get; set; }

        public ICollection<OrderProductLog> OrderProductLogs { get; set; }

    }
}
