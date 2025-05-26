using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOSendMediciones
    {
        public DTOSendMediciones()
        {
            detailItems = new List<DTOSendDetailItems>();
            detailTerceros = new List<DTOSendDetailTerceros>();
        }

        public string DateProduction { get; set; }

        public string GUID { get; set; }

        public string photo { get; set; }

        public long TypeActivity { get; set; }

        public long UserAppId { get; set; }

        public List<DTOSendDetailItems> detailItems { get; set; }

        public List<DTOSendDetailTerceros> detailTerceros { get; set; }

        public string ResponseTransaction { get; set; }

    }
}
