using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{


    public class DTOParamInventaryQuanty
    {
        public DTOParamInventaryQuanty()
        {
            ListTimes = new List<TimeSpan>();
        }
        public int IsEnable { get; set; }


        public string Horas { get; set; }


        public List<TimeSpan> ListTimes { get; set; }


        public void ReadTimes()
        {
            if (Horas != null)
            {
                foreach (string hou in Horas.Split(","))
                {
                    string[] houmin = hou.Split(":");
                    TimeSpan time = new TimeSpan(int.Parse(houmin[0]), int.Parse(houmin[1]), 0);
                    ListTimes.Add(time);
                }
            }
        }

        public DateTime LastSync { get; set; }

        [JsonIgnore]
        public string LastSyncText
        {
            get
            {
                if (LastSync != DateTime.MinValue)
                {
                    return LastSync.ToString("dd/MMM/yyyy hh:mm tt", CultureInfo.GetCultureInfo("ES-co"));
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
