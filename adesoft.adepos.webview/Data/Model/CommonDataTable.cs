using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class CommonDataTable
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string CategoryId { get; set; }

        public string CommonDataTitle { get; set; }

        public string ParentCommonDataTableId { get; set; }
    }
}
