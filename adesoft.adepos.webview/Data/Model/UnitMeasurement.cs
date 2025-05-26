using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class UnitMeasurement
    {
        public UnitMeasurement() { }

        [Key]
        public long UnitMeasurementId { get; set; }
        public string Name { get; set; }

        public string Abreviature { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ConversionFactor { get; set; }
    }
}
