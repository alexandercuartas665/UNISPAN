using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace adesoft.adeposx.report.WebAPIClient
{
    public class UtilAPI
    {
        public UtilAPI()
        {

        }

        public static void SetSessionConnection(HttpAPIClient httpcli, string CuentaN)
        {
            //var tasr = Task.Run<List<ConnectionDB>>(async () => await Http.GetGenericAsync<List<ConnectionDB>>("/api/Security/GetConnections"));
            //tasr.Wait();
            //var listconect = tasr.Result;
            // string jsonConecctionDB = JsonConvert.SerializeObject(listconect.Where(x => x.CuentaN == CuentaN).First());
            var tasr2 = Task.Run(async () => await httpcli.PostGenericAsync("/api/Security/SetSessionByID?CuentaN=" + CuentaN, string.Empty));
            tasr2.Wait();

        }


    }
}