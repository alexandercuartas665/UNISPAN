using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.PL;
using System.Collections.Generic;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOUserApp
    {
        public string Username { get; set; }
        
        public string Password { get; set; }

        public string PassworNotCry { get; set; }

        public List<ActionApp> ActionApps { get; set; }

        public string ZoneProductId { get; set; }
    }
}
