using Radzen;
using System;
using System.Collections.Generic;

namespace adesoft.adepos.webview.Data.DTO.ElectronicBilling
{
    public class DTOElectronicBillingFilter
    {
        public DTOElectronicBillingFilter()
        {
            this.Filters = new List<object>()
            {
                new { Id = 0, Name = "Ninguno" },
                new { Id = 1, Name = "Cliente" },
                new { Id = 2, Name = "Zona" },
                new { Id = 3, Name = "Administrador" },
            };

            this.SalesInvoiceStatus = new List<object>
            {
                new { Id = 0, Name = "Pendiente" },
                new { Id = 1, Name = "Facturado"},
                new { Id = 2, Name = "Dado de baja"}
            };
        }

        public string GuidFilter { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Year { get; set; }

        public IEnumerable<int> MonthsSelected { get; set; }

        public int MonthSelected { get; set; }

        public string Period { get; set; }

        public int FilterId { get; set; }

        public List<object> Filters { get; set; }

        public List<object> SalesInvoiceStatus { get; set; }

        public int Status { get; set; }

        public int Option { get; set; }

        public IEnumerable<string> GroupBy { get; set; }

        public bool CalcOutsBalance { get; set; }

        public bool ShowBalance { get; set; }

        public Query GridQuery { get; set; }

        public string AdminId { get; set; }

        public IEnumerable<string> POs { get; set; }
    }
}
