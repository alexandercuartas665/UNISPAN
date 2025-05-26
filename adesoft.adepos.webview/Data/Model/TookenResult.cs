using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class TookenResult
    {
        public string Tooken { get; set; }

        public DateTime Expiry { get; set; }

        public bool ConnectApp { get; set; }

        public List<ActionApp> ActionApps { get; set; }

        public string ZoneProductId { get; set; }
    }
}
