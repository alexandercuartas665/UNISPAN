using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOOrderReportFilter
    {
        public DTOOrderReportFilter()
        {
            this.SearchBy = "";
    }

        public string GuidFilter { get; set; }

        public int OrderType { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string SearchBy { get; set; }

        public int FilterId { get; set; }

        public long OrderId { get; set; }

        public string SubModule { get; set; }

        public IEnumerable<int> ReponsableTransIds { get; set; }
    }
}
