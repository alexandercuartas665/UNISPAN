using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class ConnectionDB
    {
        public string Name { get; set; }

        public string CuentaN { get; set; }
        public string Connection { get; set; }

        public long SedeId { get; set; }

        public long MenuId { get; set; }
    }
}
