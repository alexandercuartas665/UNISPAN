using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class DistributionAdminTable
    {
        public string AdminId { get; set; }

        public string ZoneId { get; set; }

        public string SectorId { get; set; }

        public string ZoneParentId { get; set; }

        public int Year { get; set; }

        public bool Active { get; set; }
    }
}
