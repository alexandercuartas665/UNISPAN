using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOHREmployReport
    {
        public string IdentificationNum { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        public string CostCenter { get; set; }

        public string Position { get; set; }

        public string CostCenterHomologate { get; set; }

        public string PositionHomologate { get; set; }

        public decimal SalaryLastYear { get; set; }

        public decimal SalaryCurrent { get; set; }

        public DateTime InitDate { get; set; }

        public int Old { get; set; }

        public DateTime EndDate { get; set; }

        public string EndDateStr { get; set; }

        public string Observations { get; set; }

        public string Sex { get; set; }

        public string EPS { get; set; }

        public string AFP { get; set; }

        public string AFC { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string MovilNumber { get; set; }

        public DateTime DateBirth { get; set; }

        public string PlaceBirth { get; set; }

        public DateTime DateExpedition { get; set; }

        public string PlaceExpedition { get; set; }

        public string State { get; set; }

        public DateTime DateRetirement { get; set; }

        public string ReasonRetirement { get; set; }

        public bool HideSubtotal { get; set; }
    }

    public class DTOHREmployFilter
    {
        public DTOHREmployFilter()
        {
            this.EnterpriceIds = new List<long>();
            this.ToDate = DateTime.Now;
            this.FilterId = 0;
            this.AreaIds = new List<long>();
            this.AreaIdsHomologate = new List<long>();
            this.CargoIdsHomologate = new List<long>();
            this.CargoIds = new List<long>();
            this.HideSubtotal = false;
            this.State = "Todos";
            this.States = new List<string>
            {
                "Todos",
                "Activos",
                "Retirados"
            };
            
        }

        public string State { get; set; }

        public List<string> States { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IEnumerable<long> EnterpriceIds { get; set; }

        public IEnumerable<long> AreaIds { get; set; }

        public IEnumerable<long> AreaIdsHomologate { get; set; }

        public IEnumerable<long> CargoIdsHomologate { get; set; }

        public IEnumerable<long> CargoIds { get; set; }

        public int FilterId { get; set; }

        public string ReportName { get; set; }

        public string GuidFilter { get; set; }

        public bool HideSubtotal { get; set; }
    }

    public class DTOHRGroup
    {
        public int Id { get; set; }

        public string Description { get; set; }
    }

    public class DTOChartData
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public decimal Value { get; set; }
    }
}
