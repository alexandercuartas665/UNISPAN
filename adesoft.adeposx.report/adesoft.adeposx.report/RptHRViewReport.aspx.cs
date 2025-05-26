using adesoft.adeposx.report.Models;
using adesoft.adeposx.report.WebAPIClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace adesoft.adeposx.report
{
    public partial class RptHRViewReport : System.Web.UI.Page
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
            string CuentaN = Request.Params.Get("CuentaN");
            if (string.IsNullOrEmpty(CuentaN))
                return;
            string filterobj = Request.Params.Get("filterobj");
            string RptOpt = Request.Params.Get("RptOpt");

            string urlbase = ConfigurationManager.AppSettings["UrlWebAPI"];
            HttpAPIClient Http = new HttpAPIClient(urlbase);
            UtilAPI.SetSessionConnection(Http, CuentaN);

            if (RptOpt.Equals("Dashboard"))
            {
                string apiurl = "api/Nomina/GetChartData?guidfilter=" + filterobj+ "&valueType=Count";
                List<DTOChartData> result = Task.Run(async () => await Http.GetGenericAsync<List<DTOChartData>>(apiurl)).Result;
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", result));

                apiurl = "api/Nomina/GetChartData?guidfilter=" + filterobj + "&valueType=Salary";
                result = Task.Run(async () => await Http.GetGenericAsync<List<DTOChartData>>(apiurl)).Result;
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", result));
            }
            else 
            {
                string apiurl = "api/Nomina/GetEmployes?guidfilter=" + filterobj;
                List<DTOHREmployReport> result = Task.Run(async () => await Http.GetGenericAsync<List<DTOHREmployReport>>(apiurl)).Result;
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", result));
            }            

            ReportViewer1.LocalReport.ReportEmbeddedResource = string.Format("adesoft.adeposx.report.Reports.RptHR{0}.rdlc", RptOpt);

            ReportViewer1.LocalReport.Refresh();
        }
    }
}