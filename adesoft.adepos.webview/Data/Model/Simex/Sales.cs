using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.Simex
{
    public class Sales : BaseEntity
    {
        public Sales()
        {

        }

        public DateTime DocumentDate { get; set; }

        public string DocumentNum { get; set; }

        public string Operation1 { get; set; }

        public string Operation2 { get; set; }

        public string Operation3 { get; set; }

        public string Operation4 { get; set; }

        public string Operation5 { get; set; }

        public string Operation6 { get; set; }

        public int MovementType { get; set; }

        public string CustVendName { get; set; }

        public string CustVendClasification { get; set; }

        public string SalesPerson { get; set; }

        public string SalesPersonClasification { get; set; }

        public int Warehouse { get; set; }

        public string Element { get; set; }

        public string ElementName { get; set; }

        public string ElementData2 { get; set; }

        public long Qty { get; set; }

        public string CountryRegionId { get; set; }

        public long Idx { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostUnit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostTotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceUnit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalesTotalAmount { get; set; }

        public bool Modified { get; set; }
    }
}
