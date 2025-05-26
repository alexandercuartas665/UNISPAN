using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOOrderProductVersion
    {
        public string OrderProductVersionId { get; set; }

        public long OrderId { get; set; }

        public int Version { get; set; }

        public string VersionCode { get; set; }

        public DateTime CreatedOn { get; set; }

        public List<DTOOrderProductLog> OrderProductLogs { get; set; }
    }
}
