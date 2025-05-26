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
    public partial class RptContabilidad : System.Web.UI.Page
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
            //string jsonparam = JsonConvert.SerializeObject(dtotra);
            if (RptOpt == "1")
            {
                string AddFieldDynam = Request.Params.Get("AddFieldDynam");
                string jsonparam = "";
                string apiurl = "/api/Biable/GetDataReportContable?guidfilter=" + filterobj;
                var tas = Task.Run<List<DTOReportBalanceMonth>>(async () => await Http.PostGenericAsync<List<DTOReportBalanceMonth>>(apiurl, jsonparam));
                var resu = tas.Result;


                //if (AddFieldDynam.ToUpper() == "TRUE")
                //    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiable2.rdlc";
                //else
                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptBalanceMonth.rdlc";

                //string yearsselect = "";
                //if (resu.Count > 0)
                //    yearsselect = resu.First().AuxField;k
                List<ReportParameter> paramters = new List<ReportParameter>();
                //paramters.Add(new ReportParameter("ParamYears", yearsselect));
                //paramters.Add(new ReportParameter("parambodega", parambodega));
                //ReportViewer1.LocalReport.SetParameters(paramters);
                List<DTOParamContable> lisparm = new List<DTOParamContable>();
                DTOReportBalanceMonth rptpa = resu.Where(x => x.ParamsContable != null).FirstOrDefault();
                if (rptpa != null)
                    lisparm = rptpa.ParamsContable;

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet2", lisparm));
            }

            ReportViewer1.LocalReport.Refresh();
        }
    }
}