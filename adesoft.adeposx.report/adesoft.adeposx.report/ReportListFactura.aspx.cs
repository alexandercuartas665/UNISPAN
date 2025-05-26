using adesoft.adeposx.report.Models;
using adesoft.adeposx.report.WebAPIClient;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace adesoft.adeposx.report
{


    public partial class ReportListFactura : System.Web.UI.Page
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
            string RptOpt = Request.Params.Get("RptOpt");

            string urlbase = ConfigurationManager.AppSettings["UrlWebAPI"];
            HttpAPIClient Http = new HttpAPIClient(urlbase);
            UtilAPI.SetSessionConnection(Http, CuentaN);
            if (RptOpt == "1")
            {
                #region reporte facturacion detallado

                string DateInit = Request.Params.Get("DateInit");
                string DateEnd = Request.Params.Get("DateEnd");

                string apiurl = "/api/TransactionGeneric/selectAllReport?" + "SDateInit=" + DateInit + "&SDateEnd=" + DateEnd + "&TransOption=2";
                var tas = Task.Run<List<DTOTransactionReport>>(async () => await Http.GetGenericAsync<List<DTOTransactionReport>>(apiurl));
                // var tas = Task.Run<List<dynamic>>(async () => await Http.GetGenericAsync<List<dynamic>>(apiurl));
                tas.Wait();
                var resu = tas.Result;

                DTOTransactionReport totales = new DTOTransactionReport();
                totales.TotalDiscount = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalDiscount).Sum();
                totales.TotalBuy = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalBuy).Sum();
                totales.HSubtotal = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().HSubtotal).Sum();
                totales.TotalTax = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalTax).Sum();
                totales.TotalOtherDiscount = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalOtherDiscount).Sum();
                totales.TotalCost = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalCost).Sum();


                List<DTOTransactionReport> Lsittotal = new List<DTOTransactionReport>();
                Lsittotal.Add(totales);

                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptListFacturaVenta.rdlc";
                List<ReportParameter> paramters = new List<ReportParameter>();
                ReportParameter PDateInit = new ReportParameter("DateInit", DateInit);
                paramters.Add(PDateInit);
                ReportParameter PDateEnd = new ReportParameter("DateEnd", DateEnd, true);
                paramters.Add(PDateEnd);

                ReportViewer1.LocalReport.SetParameters(paramters);

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", Lsittotal));
                #endregion
            }
            else if (RptOpt == "2")
            {
                #region reporte compras detallado

                string DateInit = Request.Params.Get("DateInit");
                string DateEnd = Request.Params.Get("DateEnd");

                string apiurl = "/api/TransactionGeneric/selectAllReport?" + "SDateInit=" + DateInit + "&SDateEnd=" + DateEnd + "&TransOption=3";
                var tas = Task.Run<List<DTOTransactionReport>>(async () => await Http.GetGenericAsync<List<DTOTransactionReport>>(apiurl));
                // var tas = Task.Run<List<dynamic>>(async () => await Http.GetGenericAsync<List<dynamic>>(apiurl));
                tas.Wait();
                var resu = tas.Result;

                DTOTransactionReport totales = new DTOTransactionReport();

                totales.TotalBuy = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalBuy).Sum();


                List<DTOTransactionReport> Lsittotal = new List<DTOTransactionReport>();
                Lsittotal.Add(totales);

                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasDetallado.rdlc";
                List<ReportParameter> paramters = new List<ReportParameter>();
                ReportParameter PDateInit = new ReportParameter("DateInit", DateInit);
                paramters.Add(PDateInit);
                ReportParameter PDateEnd = new ReportParameter("DateEnd", DateEnd, true);
                paramters.Add(PDateEnd);

                ReportViewer1.LocalReport.SetParameters(paramters);

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", Lsittotal));
                #endregion
            }
            else if (RptOpt == "3")
            {
                #region reporte de mermas

                string DateInit = Request.Params.Get("DateInit");
                string DateEnd = Request.Params.Get("DateEnd");

                string apiurl = "/api/TransactionGeneric/selectAllReport?" + "SDateInit=" + DateInit + "&SDateEnd=" + DateEnd + "&TransOption=4";
                var tas = Task.Run<List<DTOTransactionReport>>(async () => await Http.GetGenericAsync<List<DTOTransactionReport>>(apiurl));
                // var tas = Task.Run<List<dynamic>>(async () => await Http.GetGenericAsync<List<dynamic>>(apiurl));
                tas.Wait();
                var resu = tas.Result;

                DTOTransactionReport totales = new DTOTransactionReport();
                totales.TotalDiscount = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalDiscount).Sum();
                totales.TotalBuy = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalBuy).Sum();
                totales.HSubtotal = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().HSubtotal).Sum();
                totales.TotalTax = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalTax).Sum();
                totales.TotalOtherDiscount = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalOtherDiscount).Sum();
                totales.TotalCost = resu.GroupBy(x => x.TransactionGenericId).Select(x => x.First().TotalCost).Sum();
                List<DTOTransactionReport> Lsittotal = new List<DTOTransactionReport>();
                Lsittotal.Add(totales);

                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptMermas.rdlc";
                List<ReportParameter> paramters = new List<ReportParameter>();
                ReportParameter PDateInit = new ReportParameter("DateInit", DateInit);
                paramters.Add(PDateInit);
                ReportParameter PDateEnd = new ReportParameter("DateEnd", DateEnd, true);
                paramters.Add(PDateEnd);

                ReportViewer1.LocalReport.SetParameters(paramters);

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", Lsittotal));
                #endregion
            }
            else if (RptOpt == "4")
            {
                #region reporte ventas puente

                string DateInit = Request.Params.Get("DateInit");
                string DateEnd = Request.Params.Get("DateEnd");

                string apiurl = "/api/TransactionGeneric/selectAllReport?" + "SDateInit=" + DateInit + "&SDateEnd=" + DateEnd + "&TransOption=5";
                var tas = Task.Run<List<DTOTransactionReport>>(async () => await Http.GetGenericAsync<List<DTOTransactionReport>>(apiurl));
             
                tas.Wait();
                var resu = tas.Result;
                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptFacturasPuente.rdlc";
                List<ReportParameter> paramters = new List<ReportParameter>();
                ReportParameter PDateInit = new ReportParameter("DateInit", DateInit);
                paramters.Add(PDateInit);
                ReportParameter PDateEnd = new ReportParameter("DateEnd", DateEnd, true);
                paramters.Add(PDateEnd);
                ReportViewer1.LocalReport.SetParameters(paramters);
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
             
                #endregion
            }
            ReportViewer1.LocalReport.Refresh();
        }
    }
}