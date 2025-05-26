using adesoft.adeposx.report.Models;
using adesoft.adeposx.report.WebAPIClient;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace adesoft.adeposx.report
{
    public partial class RptSummaryRents : System.Web.UI.Page
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

            string AddFieldDynam = Request.Params.Get("AddFieldDynam");
            string apiurl = "api/ledgerBalance/GetSummaryRents?guidfilter=" + filterobj;
            List<DTOSummaryRentReport> result = Task.Run(async () => await Http.GetGenericAsync<List<DTOSummaryRentReport>>(apiurl)).Result;            

            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", result));

            if (RptOpt.Contains("S"))
            {
                result = result.AsQueryable()
                    .OrderByDescending(s => s.TotBalanceAmt)
                    .Take(5)
                    .ToList();
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", result));
            }

            ReportViewer1.LocalReport.ReportEmbeddedResource = string.Format("adesoft.adeposx.report.Reports.RptSummaryRents{0}.rdlc", RptOpt);

            ReportViewer1.LocalReport.Refresh();
        }
    }
}