using adesoft.adeposx.report.Models;
using adesoft.adeposx.report.Models.Simex;
using adesoft.adeposx.report.WebAPIClient;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adeposx.report
{
    public partial class RptSimexSalesOrder : System.Web.UI.Page
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
            string apiurl = "api/simex/GetSalesOrder?guidfilter=" + filterobj;
            List<DTOSimexSalesOrderReport> result = Task.Run(async () => await Http.GetGenericAsync<List<DTOSimexSalesOrderReport>>(apiurl)).Result;
            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", result));

            result = Task.Run(async () => await Http.GetGenericAsync<List<DTOSimexSalesOrderReport>>(apiurl)).Result;
            result = result.GroupBy(r => new { r.GroupBy1 })
                .Select(rs => new DTOSimexSalesOrderReport { GroupBy1 = rs.Max(rsx => rsx.GroupBy1), AmountPendingRecived = rs.Sum(rsx => rsx.AmountPendingRecived) })
                .ToList();

            result = result.OrderByDescending(r => r.AmountPendingRecived).Take(10).ToList();

            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", result));

            result = Task.Run(async () => await Http.GetGenericAsync<List<DTOSimexSalesOrderReport>>(apiurl)).Result;
            result = result.GroupBy(r => new { r.GroupBy1 })
                .Select(rs => new DTOSimexSalesOrderReport { GroupBy1 = rs.Max(rsx => rsx.GroupBy1), AmountPendingInvoiced = rs.Sum(rsx => rsx.AmountPendingInvoiced) })
                .ToList();

            result = result.OrderByDescending(r => r.AmountPendingInvoiced).Take(10).ToList();

            ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet3", result));

            ReportViewer1.LocalReport.ReportEmbeddedResource = string.Format("adesoft.adeposx.report.Reports.Simex.RptSimexSalesOrder{0}.rdlc", RptOpt);

            ReportViewer1.LocalReport.Refresh();
        }
    }
}