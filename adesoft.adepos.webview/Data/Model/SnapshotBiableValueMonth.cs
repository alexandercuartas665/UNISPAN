using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class SnapshotBiableValueMonth
    {
        public SnapshotBiableValueMonth()
        {

        }
        [Key]
        public long SnapshotBiableValueMonthId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValueBalanceMonth { get; set; }

        public int MonthId { get; set; }

        public int YearId { get; set; }

        public string MonthYear { get; set; }
    }
}
