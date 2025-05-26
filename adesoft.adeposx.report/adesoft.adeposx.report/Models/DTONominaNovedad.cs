using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models
{
    public class DTONominaNovedad
    {
        public DTONominaNovedad()
        {

        }

        public long NominaNovedadId { get; set; }

        public string TypeNovedadName { get; set; }

        public long CodeNovedadId { get; set; }

        public string TerceroFullName { get; set; }

        public DateTime DayInit { get; set; }

        public DateTime DayEnd { get; set; }

        public long TerceroId { get; set; }

        public bool FullDay { get; set; }

        public decimal HoursNovedad2 { get; set; }

        public string DateInitEndLabelProduction { get; set; }

        public string Observation { get; set; }
    }
}