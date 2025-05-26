using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models
{
    public class DTOWrapperReport
    {
        public DTOWrapperReport()
        {
            Rendimientos = new List<DTORendimiento>();
            NominaNovedades = new List<DTONominaNovedad>();
        }
        public bool Group1Active { get; set; }

        public bool Group2Active { get; set; }
        public List<DTORendimiento> Rendimientos { get; set; }

        public List<DTONominaNovedad> NominaNovedades { get; set; }
    }
}