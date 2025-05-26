using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class ReportDynamic
    {
        public ReportDynamic()
        {

        }
        [Key]
        public long ReportDynamicId { get; set; }

        public string ReportName { get; set; }

        public string ReportDescription { get; set; }

    }
}
