using adesoft.adepos.webview.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOParamRango : ICloneable
    {
        public DTOParamRango()
        {
            Details = new List<DTOParamRangoDetail>();
        }

        public string ParaModule { get; set; }
        public long TypeActivityId { get; set; }
        public long YearId { get; set; }

        public long MonthId { get; set; }
        public string MonthName { get; set; }

        [JsonIgnore]
        public TypeActivity TypeActivity { get; set; }

        public List<DTOParamRangoDetail> Details { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
