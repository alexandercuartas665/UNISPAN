using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.Simex
{
    public class Cartera
    {
        public Cartera()
        {
            this.Range = string.Empty;
            this.Current = 0;
            this.Days1To30 = 0;
            this.Days31To60 = 0;
            this.Days61To90 = 0;
            this.More90 = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string LedgerAccount { get; set; }

        public string VendorName { get; set; }

        public string ThirdAccount { get; set; }

        public string ThirdName { get; set; }

        public string Operation2 { get; set; }

        public string Operation3 { get; set; }

        public string Operation4 { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime PaymentDateEstimated { get; set; }

        public string Reference { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Current { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Days1To30 { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Days31To60 { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Days61To90 { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal More90 { get; set; }

        public int ExpirationDays { get; set; }

        public string Range { get; set; }
    }
}
