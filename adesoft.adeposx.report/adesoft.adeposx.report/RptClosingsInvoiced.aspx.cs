using adesoft.adeposx.report.Models.ElectronicBilling;
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
    public partial class RptClosingsInvoiced : System.Web.UI.Page
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
            //string RptOpt = Request.Params.Get("RptOpt");

            string urlbase = ConfigurationManager.AppSettings["UrlWebAPI"];
            HttpAPIClient Http = new HttpAPIClient(urlbase);
            UtilAPI.SetSessionConnection(Http, CuentaN);

            string apiurl = "api/ElectronicBilling/GetClosingsInvoiced?guidfilter=" + filterobj;
            List<DTOClosingInvoiced> result = Task.Run(async () => await Http.GetGenericAsync<List<DTOClosingInvoiced>>(apiurl)).Result;

            foreach(var c in result)
            {
                c.OthersWorkNo = $"{c.WorkNo}"+(!string.IsNullOrEmpty(c.OthersWorkNo) ? $"-{c.OthersWorkNo}" : "");
            }

            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", result));

            ReportViewer1.LocalReport.ReportEmbeddedResource = $"adesoft.adeposx.report.Reports.ElectronicBilling.RptClosingsInvoiced.rdlc";

            ReportViewer1.LocalReport.Refresh();
        }
    }
}