using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOReportBalanceMonth
    {
        public DTOReportBalanceMonth()
        {

        }

        public long Id { get; set; }

        public long PositionNum { get; set; }
        public long OrderNum { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Accounts { get; set; }
        public long OrderMonth { get; set; }
        public string Month { get; set; }

        public DateTime MonthDateRecord { get; set; }
        public List<DTOParamContable> ParamsContable { get; set; }
        public decimal? Value { get; set; }

        public decimal? ValueDolar { get; set; }

        public string Monthyear { get; set; }

        public int IdMonth { get; set; }

        public int IdYear { get; set; }
    }
}
