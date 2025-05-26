using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class DetailProduction : BaseEntity
    {
        public DetailProduction()
        {

        }
        [Key]
        public long DetailProductionId { get; set; }

        public long ProductionId { get; set; }

        public long ItemId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cant { get; set; }

        [NotMapped]
        public Item Item { get; set; }

        [NotMapped]
        public string ObservacionesCambio { get; set; }


        public string Photo1Name { get; set; }

        public string Photo2Name { get; set; }

        public string Photo3Name { get; set; }


        [NotMapped]
        public string Photo1Base64 { get; set; }
        [NotMapped]
        public string Photo2Base64 { get; set; }
        [NotMapped]
        public string Photo3Base64 { get; set; }


        public string Observation { get; set; }


    }
}
