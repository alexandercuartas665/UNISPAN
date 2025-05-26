using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class RendimientoObject : BaseEntity
    {
        public RendimientoObject()
        {

        }
        [Key]
        public long Id { get; set; }

        public long MonthId { get; set; }

        public long YearId { get; set; }

        public string RendimientoJsonObj { get; set; }
    }
}
