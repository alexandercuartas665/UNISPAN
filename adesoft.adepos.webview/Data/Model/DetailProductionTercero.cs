using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class DetailProductionTercero : BaseEntity
    {
        public DetailProductionTercero()
        {

        }
        [Key]
        public long DetailProductionTerceroId { get; set; }

        public long ProductionId { get; set; }

        public long TerceroId { get; set; }

        [NotMapped]
        public Tercero Tercero { get; set; }
    }
}
