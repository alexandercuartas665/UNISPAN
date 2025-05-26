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
    public partial class RptLogisticsOrders : System.Web.UI.Page
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

            //DTOComprasReport dtotra = new DTOComprasReport();
            //string jsonparam = JsonConvert.SerializeObject(dtotra);{
            string AddFieldDynam = Request.Params.Get("AddFieldDynam");
            string apiurl = "api/logistics/GetOrdersReport?guidfilter=" + filterobj;
            List<DTOOrderReport> result = Task.Run(async () => await Http.GetGenericAsync<List<DTOOrderReport>>(apiurl)).Result;

            var orders = result.OrderBy(o => o.DispatchId).ToList();

            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", orders));

            ReportViewer1.LocalReport.ReportEmbeddedResource = string.Format("adesoft.adeposx.report.Reports.RptLogisticsOrder{0}.rdlc", RptOpt);

            ReportViewer1.LocalReport.Refresh();
            //ReportViewer1.ExportDialog();
        }
    }
}