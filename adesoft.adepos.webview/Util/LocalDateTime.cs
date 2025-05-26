using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adesoft.adepos.webview.Util
{
    public class LocalDateTime
    {
        public static DateTime Now
        {
            get
            { 
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzi);
            }
        }

    }
}