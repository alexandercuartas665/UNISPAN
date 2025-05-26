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
    public partial class RptCompras : System.Web.UI.Page
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

            string parambodega = Request.Params.Get("Bodega");
            string urlbase = ConfigurationManager.AppSettings["UrlWebAPI"];
            HttpAPIClient Http = new HttpAPIClient(urlbase);
            UtilAPI.SetSessionConnection(Http, CuentaN);

            //DTOComprasReport dtotra = new DTOComprasReport();
            //string jsonparam = JsonConvert.SerializeObject(dtotra);
            if (RptOpt == "1")
            {
                string AddFieldDynam = Request.Params.Get("AddFieldDynam");
                string jsonparam = "";
                string apiurl = "/api/Biable/GetDataReport?guidfilter=" + filterobj;
                var tas = Task.Run<List<DTOComprasReport>>(async () => await Http.PostGenericAsync<List<DTOComprasReport>>(apiurl, jsonparam));
                var resu = tas.Result;


                if (AddFieldDynam.ToUpper() == "TRUE")
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiable2.rdlc";
                else
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiable.rdlc";

                string yearsselect = "";
                if (resu.Count > 0)
                    yearsselect = resu.First().AuxField;
                List<ReportParameter> paramters = new List<ReportParameter>();
                paramters.Add(new ReportParameter("ParamYears", yearsselect));
                paramters.Add(new ReportParameter("parambodega", parambodega));
                ReportViewer1.LocalReport.SetParameters(paramters);

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
            }
            else if (RptOpt == "2")
            {
                string AddFieldDynam = Request.Params.Get("AddFieldDynam");
                string jsonparam = "";
                string apiurl = "/api/Biable/GetDataReport?guidfilter=" + filterobj;
                var tas = Task.Run<List<DTOComprasReport>>(async () => await Http.PostGenericAsync<List<DTOComprasReport>>(apiurl, jsonparam));
                var resu = tas.Result;
                if (AddFieldDynam.ToUpper() == "TRUE")
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasXProveedorBiable2.rdlc";
                else
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasXProveedorBiable.rdlc";
                string yearsselect = "";
                if (resu.Count > 0)
                    yearsselect = resu.First().AuxField;
                List<ReportParameter> paramters = new List<ReportParameter>();
                paramters.Add(new ReportParameter("ParamYears", yearsselect));
                paramters.Add(new ReportParameter("parambodega", parambodega));
                ReportViewer1.LocalReport.SetParameters(paramters);

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
            }
            else if (RptOpt == "3")
            {
                string jsonparam = "";
                string apiurl = "/api/Biable/GetDataReport?guidfilter=" + filterobj;
                var tas = Task.Run<List<DTOComprasReport>>(async () => await Http.PostGenericAsync<List<DTOComprasReport>>(apiurl, jsonparam));
                var resu = tas.Result;

                ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiableDetalle.rdlc";

                List<ReportParameter> paramters = new List<ReportParameter>();
                paramters.Add(new ReportParameter("parambodega", parambodega));
                ReportViewer1.LocalReport.SetParameters(paramters);

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
            }
            else if (RptOpt == "4")
            {
                string AddFieldDynam = Request.Params.Get("AddFieldDynam");
                string jsonparam = "";
                string apiurl = "/api/Biable/GetDataReport?guidfilter=" + filterobj;
                var tas = Task.Run<List<DTOComprasReport>>(async () => await Http.PostGenericAsync<List<DTOComprasReport>>(apiurl, jsonparam));
                var resu = tas.Result;
                if (AddFieldDynam.ToUpper() == "TRUE")
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasAnualProveedor2.rdlc";
                else
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasAnualProveedor.rdlc";
                string yearsselect = "";
                if (resu.Count > 0)
                    yearsselect = resu.First().AuxField;
                List<ReportParameter> paramters = new List<ReportParameter>();
                paramters.Add(new ReportParameter("ParamYears", yearsselect));
                paramters.Add(new ReportParameter("parambodega", parambodega));
                ReportViewer1.LocalReport.SetParameters(paramters);

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
            }
            else if (RptOpt == "5")
            {
                string AddFieldDynam = Request.Params.Get("AddFieldDynam");
                string jsonparam = "";
                string apiurl = "/api/Biable/GetDataReport?guidfilter=" + filterobj;
                var tas = Task.Run<List<DTOComprasReport>>(async () => await Http.PostGenericAsync<List<DTOComprasReport>>(apiurl, jsonparam));
                var resu = tas.Result;


                if (AddFieldDynam.ToUpper() == "TRUE")
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasAnualtem2.rdlc";
                else
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasAnualtem.rdlc";

                string yearsselect = "";
                if (resu.Count > 0)
                    yearsselect = resu.First().AuxField;
                List<ReportParameter> paramters = new List<ReportParameter>();
                paramters.Add(new ReportParameter("ParamYears", yearsselect));
                paramters.Add(new ReportParameter("parambodega", parambodega));
                ReportViewer1.LocalReport.SetParameters(paramters);

                ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
            }
            ReportViewer1.LocalReport.Refresh();
        }
    }
}