using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTONovedadesProduction
    {
        public DTONovedadesProduction()
        {

        }

        public string NOVEDAD_ID { get; set; }
        public string OPERARIO_ID { get; set; }
        public string FECHA_INI { get; set; }
        public string FECHA_FIN { get; set; }
        public decimal CANTIDAD { get; set; }
        public string OBSERVACIONES { get; set; }

        public string responsetransaction { get; set; }
    }
}
