using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOParamContable
    {
        public DTOParamContable()
        {

        }

        public long year { get; set; }
        public DTOMonth monthObj { get; set; }
        public long month { get; set; }

        public decimal ValueDolar { get; set; }

        public decimal ImptRenta { get; set; }

        public decimal Iva { get; set; }

        public decimal InflaMens { get; set; }

        public decimal cierremesanterior { get; set; }

        public decimal devaluacionmes { get; set; }

        public decimal devaluacionanual { get; set; }

        [JsonIgnore]
        public long Yearmonth
        {
            get
            {
                string m = month.ToString().PadLeft(2, '0');
                return long.Parse(year + m);
            }
        }
    }
}
