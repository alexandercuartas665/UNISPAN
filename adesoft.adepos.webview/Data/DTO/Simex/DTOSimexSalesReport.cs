using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO.Simex
{
    public class DTOSimexSalesReport
    {
        public DTOSimexSalesReport()
        {
            this.GroupBy1 = "";
            this.GroupBy1Label = "";
            this.GroupBy2 = "";
            this.GroupBy2Label = "";
            this.GroupBy3 = "";
            this.GroupBy3Label = "";
            this.GroupBy4 = "";
            this.GroupBy4Label = "";
            this.GroupBy5 = "";
            this.GroupBy5Label = "";
            this.GroupBy6 = "";
            this.GroupBy6Label = "";
            this.PresupuestoNext = 0;
        }

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

    public class DTOSimexSalesFilter
    {
        public DTOSimexSalesFilter()
        {
            this.MultipleValuesYear = new List<long>();
            this.MultipleValuesMonth = new List<long>();
            this.GroupBy = new List<string>();
        }

        public string GuidFilter { get; set; }

        public long TypeReportId { get; set; }

        public DateTime? DateInit { get; set; }

        public DateTime? DateEnd { get; set; }

        public IEnumerable<long> MultipleValuesYear { get; set; }

        public IEnumerable<long> MultipleValuesMonth { get; set; }

        public IEnumerable<string> GroupBy { get; set; }
    }

    public class DTOGroupBy
    {
        public string GroupById { get; set; }

        public string Description { get; set; }

    }

    public class DTOZone
    {
        public string ZoneId { get; set; }

        public string Description { get; set; }
    }
}
