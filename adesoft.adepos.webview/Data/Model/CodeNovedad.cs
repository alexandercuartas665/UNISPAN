using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class CodeNovedad
    {
        public CodeNovedad()
        {

        }
        [Key]
        public long CodeNovedadId { get; set; }

        public string Syncode { get; set; }

        public string NovedadName { get; set; }

        public string NovedadAbrev { get; set; }
        /// <summary>
        /// DEDUCCION , .....
        /// </summary>
        public string TypeNovedadName { get; set; }

        public string PlaneType { get; set; }
    }
}
