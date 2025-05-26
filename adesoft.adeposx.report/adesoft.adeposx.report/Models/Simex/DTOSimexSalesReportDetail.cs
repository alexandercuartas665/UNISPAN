using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models.Simex
{
    public class DTOSimexSalesReportDetail
    {
        public DateTime DocumentDate { get; set; }

        public string DocumentNum { get; set; }

        public string Operation1 { get; set; }

        public string Operation2 { get; set; }

        public string Operation3 { get; set; }

        public string Operation4 { get; set; }

        public int MovementType { get; set; }

        public string CustVendName { get; set; }

        public string SalesPerson { get; set; }

        public int Warehouse { get; set; }

        public string Element { get; set; }

        public string ElementName { get; set; }

        public long Qty { get; set; }

        public decimal CostUnit { get; set; }

        public decimal CostTotalAmount { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal SalesTotalAmount { get; set; }

        public string DescriptionReportType { get; set; }

        #region Filtros
        public List<long> Items { get; set; }

        public List<long> Meses { get; set; }

        public List<long> Anos { get; set; }

        public string AuxField { get; set; }

        public decimal valorunit { get; set; }

        public bool visibleyear { get; set; }
        public string IdCo { get; set; }

        public string Sede { get; set; }

        #endregion
    }
}