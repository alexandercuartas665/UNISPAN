using adesoft.adeposx.report.Models;
using adesoft.adeposx.report.WebAPIClient;
using Microsoft.Reporting.WebForms;
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
    public partial class RptOrderRequisition : System.Web.UI.Page
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
            //string paramsede = Request.Params.Get("Sedes");
            //string Bodega = Request.Params.Get("Bodega");
            string urlbase = ConfigurationManager.AppSettings["UrlWebAPI"];
            HttpAPIClient Http = new HttpAPIClient(urlbase);
            UtilAPI.SetSessionConnection(Http, CuentaN);

            if (RptOpt == "1")
            {

                string jsonparam = "";
                string apiurl = "/api/TransactionGeneric/GetDataReport?guidfilter=" + filterobj;
                var tas = Task.Run<List<DTOInventary>>(async () => await Http.PostGenericAsync<List<DTOInventary>>(apiurl, jsonparam));
                var resu = tas.Result;


                //if (AddFieldDynam.ToUpper() == "TRUE")
                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptOrderRequisition.rdlc";
                //else
                //    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiable.rdlc";

                //string yearsselect = "";
                //if (resu.Count > 0)
                //    yearsselect = resu.First().AuxField;
                List<ReportParameter> paramters = new List<ReportParameter>();
                //paramters.Add(new ReportParameter("ParamYears", yearsselect));
                //paramters.Add(new ReportParameter("paramsede", paramsede));
                //paramters.Add(new ReportParameter("parambodega", Bodega));

                ReportViewer1.LocalReport.SetParameters(paramters);

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
            }
        }
    }
}