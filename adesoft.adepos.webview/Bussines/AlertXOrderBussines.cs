using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Bussines
{
    public class AlertXOrderBussines
    {

        public AdeposDBContext context { get; set; }
        public AlertXOrderBussines(AdeposDBContext context)
        {
            this.context = context;
        }

    }
}
