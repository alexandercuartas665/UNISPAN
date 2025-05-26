using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adeposx.report.Models
{
    public class DTOReportBalanceMonth
    {
        public DTOReportBalanceMonth()
        {

        }
        public long ReportDynamicId { get; set; }
        public long Id { get; set; }
        public long OrderNum { get; set; }
        public long PositionNum { get; set; }
        public DateTime MonthDateRecord { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Accounts { get; set; }
        public long OrderMonth { get; set; }
        public string Month { get; set; }
        public decimal? Value { get; set; }
        public decimal? ValueDolar { get; set; }
        public string Centros { get; set; }
        public string NitTercero { get; set; }
        public List<DTOParamContable> ParamsContable { get; set; }

        public decimal VarCant { get; set; }

        public decimal VarTotal { get; set; }

        public decimal VarTotal2 { get; set; }
    }
}
