using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class DetailReportDynamic
    {
        public DetailReportDynamic()
        {

        }

        [Key]
        public long DetailReportDynamicId { get; set; }
        public long OrderNum { get; set; }
        public long PositionNum { get; set; }
        public long ReportDynamicId { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public string Centros { get; set; }

        public string Accounts { get; set; }

        public string NitTercero { get; set; }
        [NotMapped]
        public string[] ArrayAccounts
        {
            get
            {
                if (Accounts != null)
                {
                    return Accounts.Trim().Replace(" ", "").Split(',');
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
