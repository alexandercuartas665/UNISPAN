using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class NominaProgramation : BaseEntity
    {

        public NominaProgramation()
        {

        }
        public long NominaProgramationId { get; set; }
        public DateTime DayInit { get; set; }
        public DateTime DayEnd { get; set; }
        public DateTime DayClose { get; set; }
        /// <summary>
        /// ABIERTO,CERRADO 
        /// </summary>
        public string StateProgramation { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public string DayInitLabel
        {
            get
            {
                return DayInit.ToString("dd-MMM-yyyy", CultureInfo.GetCultureInfo("ES-co"));
            }
        }
        [NotMapped]
        public string DayEndLabel
        {
            get
            {
                return DayEnd.ToString("dd-MMM-yyyy", CultureInfo.GetCultureInfo("ES-co"));
            }
        }

        [NotMapped]
        public string PathFileTnlDownload { get; set; }

        [NotMapped]
        public string PathFileNmbatchDownload { get; set; }

    }
}
