using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models
{
    public class DTOParamContable
    {
        public DTOParamContable()
        {

        }

        public long year { get; set; }

        public long month { get; set; }

        public decimal ValueDolar { get; set; }

        public decimal ImptRenta { get; set; }

        public DTOMonth monthObj { get; set; }

        public decimal Iva { get; set; }

        public decimal InflaMens { get; set; }

        public decimal cierremesanterior { get; set; }

        public decimal devaluacionmes { get; set; }

        public decimal devaluacionanual { get; set; }


        public DateTime MonthDateRecord
        {
            get
            {
                if (Yearmonth > 0)
                {
                    return DateTime.ParseExact(Yearmonth + "01", "yyyyMMdd", CultureInfo.GetCultureInfo("ES-co"));
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }

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