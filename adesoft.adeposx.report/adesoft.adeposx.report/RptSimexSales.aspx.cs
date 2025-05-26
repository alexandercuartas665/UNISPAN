
using adesoft.adeposx.report.Models;
using adesoft.adeposx.report.Models.Simex;
using adesoft.adeposx.report.WebAPIClient;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adeposx.report
{
    public partial class RptSimexSales : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RendeReport();
            }
        }

        private void RendeReport()
        {
            EventLog evento;            
            if (!EventLog.SourceExists("appUnispanLog"))
                EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
            evento = new EventLog("appUnispanLog");
            evento.Source = "appUnispanLog";
            try
            {
                evento.WriteEntry("Info Entro 1", EventLogEntryType.Information);
                string CuentaN = Request.Params.Get("CuentaN");
                if (string.IsNullOrEmpty(CuentaN))
                    return;
                string filterobj = Request.Params.Get("filterobj");
                string RptOpt = Request.Params.Get("RptOpt");

                string urlbase = ConfigurationManager.AppSettings["UrlWebAPI"];
                HttpAPIClient Http = new HttpAPIClient(urlbase);
                UtilAPI.SetSessionConnection(Http, CuentaN);

                //DTOComprasReport dtotra = new DTOComprasReport();
                //string jsonparam = JsonConvert.SerializeObject(dtotra);{
                string AddFieldDynam = Request.Params.Get("AddFieldDynam");
                string apiurl = "api/simex/GetSales?guidfilter=" + filterobj;
                List<DTOSimexSalesReport> result = Task.Run(async () => await Http.GetGenericAsync<List<DTOSimexSalesReport>>(apiurl)).Result;
                evento.WriteEntry("Info Entro 2", EventLogEntryType.Information);
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", result));

                result = result.AsQueryable()
                    .OrderByDescending(s => s.SalesTotalAmount)
                    .Take(10)
                    .ToList();

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", result));

                ReportViewer1.LocalReport.ReportEmbeddedResource = string.Format("adesoft.adeposx.report.Reports.Simex.RptSimexSales{0}.rdlc", RptOpt);

                ReportViewer1.LocalReport.Refresh();
            }catch(Exception ex)
            {
                evento.WriteEntry("Error " + ex.ToString(), EventLogEntryType.Error);
            }
        }
    }
}