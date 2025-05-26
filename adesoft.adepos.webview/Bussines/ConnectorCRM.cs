using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Util;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Bussines
{
    public class ConnectorCRM
    {
        IConfiguration configuration_;
        ConnectionDB connection_;     
        EventLog evento;
        static DTOParamCRM param;
        public ConnectorCRM(IConfiguration configuration, ConnectionDB connection)
        {

            this.configuration_ = configuration;
            connection_ = connection;

            //rocampo
            //if (!EventLog.SourceExists("appUnispanLog"))
            //    EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";
        }





        static bool sincroniced = false;
        public async Task StartSyncCRM(bool ToDemand)
        {
            try
            {
                if (param == null)
                {
                    AdeposDBContext adeposC = new AdeposDBContext(connection_.Connection);
                    string parameter = adeposC.Parameters.Where(x => x.NameIdentify == "ParamCRMIntegration").First().Value2;
                    param = JsonConvert.DeserializeObject<DTOParamCRM>(parameter);
                    param.ReadTimes();
                    adeposC.DetachAll(); adeposC.Dispose();
                }
                //string Response = await HttpAPIClient.PostSendRequest("", param.CRMUrlIntegration, "");
                //List<OportunidadesCRM> list = JsonConvert.DeserializeObject<List<OportunidadesCRM>>(Response);
                if (param.IsEnable == 1 || ToDemand)
                {
                    TimeSpan timenow = DateTime.Now.TimeOfDay;

                    if ((param.ListTimes.Count > 0 && param.ListTimes.Where(x => timenow >= x && timenow < x.Add(new TimeSpan(0, 2, 0))).Count() > 0) || ToDemand)
                    {
                        if (!sincroniced || ToDemand)
                        {
                            sincroniced = true;
                            evento.WriteEntry("Sincronizacion con CRM Iniciada. ", EventLogEntryType.Information);
                            AdeposDBContext adepos = new AdeposDBContext(connection_.Connection);
                            OportunidadesCRMBussines bussine = new OportunidadesCRMBussines(adepos);
                            await bussine.Sync();
                            adepos.Dispose();
                            evento.WriteEntry("Sincronizacion con CRM Finalizada. ", EventLogEntryType.Information);
                        }
                    }
                    else
                    {
                        sincroniced = false;
                    }

                }
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Sincronizacion de oportunidades genero error . " + ex.ToString(), EventLogEntryType.Error);
            }
        }


    }
}
