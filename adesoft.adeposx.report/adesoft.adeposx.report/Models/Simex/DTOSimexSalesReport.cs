using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models.Simex
{
    public class DTOSimexSalesReport
    {
        public long Index { get; set; }

        public long ReportTypeId { get; set; }

        public string GroupBy1 { get; set; }

        public string GroupBy1Label { get; set; }

        public string GroupBy2 { get; set; }

        public string GroupBy2Label { get; set; }

        public string GroupBy3 { get; set; }

        public string GroupBy3Label { get; set; }

        public string GroupBy4 { get; set; }

        public string GroupBy4Label { get; set; }

        public string GroupBy5 { get; set; }

        public string GroupBy5Label { get; set; }

        public string GroupBy6 { get; set; }

        public string GroupBy6Label { get; set; }

        public string Year { get; set; }

        public string Month { get; set; }

        public int Mth { get; set; }

        public long QtyAcumulate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceUnitPromedio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalesTotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ParticipationPercent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PresupuestoNext { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PercentComplianceNext { get; set; }

        public string RowTotalHidden { get; set; }
    }
}