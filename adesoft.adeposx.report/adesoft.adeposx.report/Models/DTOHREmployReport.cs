using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models
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

    public class DTOChartData
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public decimal Value { get; set; }
    }
}