using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO.Simex
{
    public class DTOSimexInventSumReport
    {
        public DTOSimexInventSumReport()
        {
            this.GroupBy1 = "";
            this.GroupBy1Label = "";
            this.GroupBy2 = "";
            this.GroupBy2Label = "";
            this.UnitId = "";
            this.ItemName = "";
            this.QtyOnHand = 0;
        }

        public string GroupBy1 { get; set; }

        public string GroupBy1Label { get; set; }

        public string GroupBy2 { get; set; }

        public string GroupBy2Label { get; set; }

        public string UnitId { get; set; }

        public string ItemName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal QtyOnHand { get; set; }

        public decimal QtyMinimum { get; set; }
    }

    public class DTOSimexInventSumFilter
    {
        public DTOSimexInventSumFilter()
        {
            this.GroupBy = new List<string>();
        }

        public string GuidFilter { get; set; }

        public string SearchBy { get; set; }

        public IEnumerable<string> GroupBy { get; set; }
    }
}
