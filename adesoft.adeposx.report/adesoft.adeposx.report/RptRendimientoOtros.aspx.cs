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
    public partial class RptRendimientoOtros : System.Web.UI.Page
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
            //string messelec= Request.Params.Get("messelec");
            //string yearselec = Request.Params.Get("yearselec");
            //string paramsede = Request.Params.Get("Sedes");
            //string Bodega = Request.Params.Get("Bodega");
            string urlbase = ConfigurationManager.AppSettings["UrlWebAPI"];
            HttpAPIClient Http = new HttpAPIClient(urlbase);
            UtilAPI.SetSessionConnection(Http, CuentaN);

            if (RptOpt == "1")
            {

                string jsonparam = "";
                string apiurl = "/api/Produccion/GetDataReportRendimiento?guidfilter=" + filterobj;
                var tas = Task.Run<DTOWrapperReport>(async () => await Http.PostGenericAsync<DTOWrapperReport>(apiurl, jsonparam));
                var resu = tas.Result;
                List<DTORendimiento> rendis = resu.Rendimientos;
                //List<DTONominaNovedad> noveds = resu.NominaNovedades;
                //if (AddFieldDynam.ToUpper() == "TRUE")
                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.ReportRendimientoAcumPorItem.rdlc";
                //else
                //    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiable.rdlc";

                //string yearsselect = "";
                //if (resu.Count > 0)
                //    yearsselect = resu.First().AuxField;
                List<ReportParameter> paramters = new List<ReportParameter>();
                paramters.Add(new ReportParameter("Group1Active", (!resu.Group1Active).ToString()));
                paramters.Add(new ReportParameter("Group2Active", (!resu.Group2Active).ToString()));
                if (rendis.Count > 0)
                    paramters.Add(new ReportParameter("DateActivity", rendis.First().DateActivity.ToString("MMM dd, yyyy", CultureInfo.GetCultureInfo("ES-co"))));
                else
                    paramters.Add(new ReportParameter("DateActivity", ""));
                //paramters.Add(new ReportParameter("parambodega", Bodega));

                ReportViewer1.LocalReport.SetParameters(paramters);

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", rendis));
            }
            ReportViewer1.LocalReport.Refresh();
        }
    }
}