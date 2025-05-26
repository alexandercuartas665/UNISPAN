using adesoft.adeposx.report.Models;
using adesoft.adeposx.report.WebAPIClient;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
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
    public partial class ReportOrderDispatch : System.Web.UI.Page
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
                #region reporte de ordenes de despacho
                DTOTransactionReport dtotra = new DTOTransactionReport();
                dtotra.TransOption = 1;
                string DatInit = Request.Params.Get("DateInit");
                if (DatInit != null)
                    dtotra.DateInit = DateTime.ParseExact(DatInit, "MM/dd/yyyy", null);
                string DatEnd = Request.Params.Get("DateEnd");
                if (DatEnd != null)
                    dtotra.DateEnd = DateTime.ParseExact(DatEnd, "MM/dd/yyyy", null);
                string ItemId = Request.Params.Get("ItemId");
                if (ItemId != null)
                    dtotra.ItemId = long.Parse(ItemId);
                string Warehouseid = Request.Params.Get("Warehouseid");
                if (Warehouseid != null)
                    dtotra.Warehouseid = long.Parse(Warehouseid);
                string FilterOnlyPendient = Request.Params.Get("FilterOnlyPendient");
                if (FilterOnlyPendient != null)
                    dtotra.FilterOnlyPendient = bool.Parse(FilterOnlyPendient);
                string TransactionId = Request.Params.Get("TransactionId");
                if (TransactionId != null)
                    dtotra.TransactionGenericId = long.Parse(TransactionId);

                string jsonparam = JsonConvert.SerializeObject(dtotra);
                string apiurl = "/api/TransactionGeneric/GetDataReportDispatch";
                var tas = Task.Run<List<DTOTransactionReport>>(async () => await Http.PostGenericAsync<List<DTOTransactionReport>>(apiurl, jsonparam));
                // var tas = Task.Run<List<dynamic>>(async () => await Http.GetGenericAsync<List<dynamic>>(apiurl));
                tas.Wait();
                var resu = tas.Result;

                if (resu.Count > 0)
                {//aqui puedo rrecorrer todas las ordenes y luego empezar a agregar a todas los items que falten

                }

                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptListDispatch.rdlc";
                //List<ReportParameter> paramters = new List<ReportParameter>();
                //ReportViewer1.LocalReport.SetParameters(paramters);
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                #endregion
            }
            else if (RptOpt == "2")
            {
                #region reporte de inventario
                DTOInventary dtotra = new DTOInventary();
                dtotra.TransOption = 2;
                string Warehouseid = Request.Params.Get("Warehouseid");
                if (Warehouseid != null)
                    dtotra.Warehouseid = long.Parse(Warehouseid);

                string jsonparam = JsonConvert.SerializeObject(dtotra);
                string apiurl = "/api/MovementInventory/selectAll";
                var tas = Task.Run<List<DTOInventary>>(async () => await Http.PostGenericAsync<List<DTOInventary>>(apiurl, jsonparam));
                // var tas = Task.Run<List<dynamic>>(async () => await Http.GetGenericAsync<List<dynamic>>(apiurl));
                tas.Wait();
                var resu = tas.Result;

                if (resu.Count > 0)
                {//aqui puedo rrecorrer todas las ordenes y luego empezar a agregar a todas los items que falten

                }

                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptInventory.rdlc";
                //List<ReportParameter> paramters = new List<ReportParameter>();
                //ReportViewer1.LocalReport.SetParameters(paramters);
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                #endregion  
            }
            else if (RptOpt == "3")
            {
                #region reporte de ordenes de fabricacion
                DTOTransactionReport dtotra = new DTOTransactionReport();
                dtotra.TransOption = 2;
                //string DatInit = Request.Params.Get("DateInit");
                //if (DatInit != null)
                //    dtotra.DateInit = DateTime.ParseExact(DatInit, "MM/dd/yyyy", null);
                //string DatEnd = Request.Params.Get("DateEnd");
                //if (DatEnd != null)
                //    dtotra.DateEnd = DateTime.ParseExact(DatEnd, "MM/dd/yyyy", null);
                string Warehouseid = Request.Params.Get("Warehouseid");
                if (Warehouseid != null)
                    dtotra.Warehouseid = long.Parse(Warehouseid);
            
                string jsonparam = JsonConvert.SerializeObject(dtotra);
                string apiurl = "/api/TransactionGeneric/GetDataReportDispatch";
                var tas = Task.Run<List<DTOTransactionReport>>(async () => await Http.PostGenericAsync<List<DTOTransactionReport>>(apiurl, jsonparam));
                // var tas = Task.Run<List<dynamic>>(async () => await Http.GetGenericAsync<List<dynamic>>(apiurl));
                tas.Wait();
                var resu = tas.Result;

                if (resu.Count > 0)
                {//aqui puedo rrecorrer todas las ordenes y luego empezar a agregar a todas los items que falten

                }

                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptFabricacion.rdlc";
                //List<ReportParameter> paramters = new List<ReportParameter>();
                //ReportViewer1.LocalReport.SetParameters(paramters);
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                #endregion
            }

            ReportViewer1.LocalReport.Refresh();
        }
    }
}