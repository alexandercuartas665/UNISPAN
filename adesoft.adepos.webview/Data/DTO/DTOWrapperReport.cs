using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOWrapperReport
    {

        public DTOWrapperReport()
        {
            Rendimientos = new List<Rendimiento>();
            NominaNovedades = new List<NominaNovedad>();

        }
        public bool Group1Active { get; set; }

        public bool Group2Active { get; set; }
        public List<Rendimiento> Rendimientos { get; set; }

        public List<NominaNovedad> NominaNovedades { get; set; }
    }
}
