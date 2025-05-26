using adesoft.adeposx.report.WebAPIClient;
using adesoft.adeposx.report.Models.PL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;

namespace adesoft.adeposx.report
{
    public partial class RptPlantReport : System.Web.UI.Page
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
                string filterobj = Request.Params.Get("filterObj");
                string rptOpt = Request.Params.Get("rptOpt");

                string urlbase = ConfigurationManager.AppSettings["UrlWebAPI"];
                HttpAPIClient Http = new HttpAPIClient(urlbase);
                UtilAPI.SetSessionConnection(Http, CuentaN);

                //DTOComprasReport dtotra = new DTOComprasReport();
                //string jsonparam = JsonConvert.SerializeObject(dtotra);{
                string AddFieldDynam = Request.Params.Get("AddFieldDynam");

                if(rptOpt.Equals("1"))
                {
                    string apiurl = "api/plant/GetOrderReport?guidfilter=" + filterobj;
                    DTOOrder result = Task.Run(async () => await Http.GetGenericAsync<DTOOrder>(apiurl)).Result;
                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", new List<DTOOrder>() { result }));

                    var products = result.Products.ToList();
                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", products));

                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.PL.RptOrderDetails.rdlc";

                }
                else if(rptOpt.Equals("2"))
                {
                    string apiurl = "api/plant/GetOrderPalletReport?guidfilter=" + filterobj;
                    DTOOrderPallet result = Task.Run(async () => await Http.GetGenericAsync<DTOOrderPallet>(apiurl)).Result;

                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", new List<DTOOrderPallet>() { result }));

                    var products = result.OrderPalletProducts.ToList();
                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", products));

                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.PL.RptPalletInfo.rdlc";
                }
                else if (rptOpt.Equals("3"))
                {
                    string apiurl = "api/plant/GetPickingStatus?guidfilter=" + filterobj;
                    DTOOrder result = Task.Run(async () => await Http.GetGenericAsync<DTOOrder>(apiurl)).Result;
                    result.ProgresString = string.Format("{0} %", result.Progress.ToString("0"));

                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", new List<DTOOrder>() { result }));

                    var pickingStatusDetails = result.PickingStatusDetails.ToList();
                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", pickingStatusDetails));

                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.PL.RptPickingStatus.rdlc";
                }

                ReportViewer1.ShowPrintButton = true;

                ReportViewer1.LocalReport.Refresh();
            }
            catch (Exception ex)
            {
                evento.WriteEntry("Error " + ex.ToString(), EventLogEntryType.Error);
            }
        }
    }
}