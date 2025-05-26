using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models.Simex
{
    public class DTOSimexInventSumReport
    {
        public string GroupBy1 { get; set; }

        public string GroupBy1Label { get; set; }

        public string GroupBy2 { get; set; }

        public string GroupBy2Label { get; set; }

        public string UnitId { get; set; }

        public string ItemName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal QtyOnHand { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal QtyMinimum{ get; set; }
    }
}