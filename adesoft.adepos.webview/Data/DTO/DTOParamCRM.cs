using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{

    public class DTOParamCRM
    {
        public DTOParamCRM()
        {
            ListTimes = new List<TimeSpan>();
        }
        public int IsEnable { get; set; }


        public string Horas { get; set; }

        public string CRMUrlIntegration { get; set; }

        public List<TimeSpan> ListTimes { get; set; }

        public string Authorization { get; set; }

        public string Cookie { get; set; }
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

    }
}
