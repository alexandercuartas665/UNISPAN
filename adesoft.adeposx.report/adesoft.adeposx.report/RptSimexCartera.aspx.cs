using adesoft.adeposx.report.Models.Simex;
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
    public partial class RptSimexCartera : System.Web.UI.Page
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

            string apiurl = "api/simex/GetCartera?guidfilter=" + filterobj;
            List<DTOSimexCarteraReport> result = Task.Run(async () => await Http.GetGenericAsync<List<DTOSimexCarteraReport>>(apiurl)).Result;
            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", result));

            result = result.AsQueryable()
                .OrderByDescending(s => s.AmountBalance)
                .Take(10)
                .ToList();

            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", result));

            ReportViewer1.LocalReport.ReportEmbeddedResource = string.Format("adesoft.adeposx.report.Reports.Simex.RptSimexCartera{0}.rdlc", RptOpt);

            ReportViewer1.LocalReport.Refresh();
        }
    }
}