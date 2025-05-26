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
    public partial class RptQuantify : System.Web.UI.Page
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
            try
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
                    string apiurl = "/api/Quantify/GetDataReportMovAlquilerQuantify?guidfilter=" + filterobj;
                    var tas = Task.Run<List<DTORptMovEquipo>>(async () => await Http.PostGenericAsync<List<DTORptMovEquipo>>(apiurl, jsonparam));
                    var resu = tas.Result;
                    //List<DTORendimiento> rendis = resu.Rendimientos;
                    //List<DTONominaNovedad> noveds = resu.NominaNovedades;
                    //if (AddFieldDynam.ToUpper() == "TRUE")
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptMovEquiposAlquilerQuantifyDiario.rdlc";
                    //else
                    //    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiable.rdlc";

                    string TitleFilterReport = "";
                    if (resu.Count > 0)
                        TitleFilterReport = resu.First().TitleFilterReport;
                    List<ReportParameter> paramters = new List<ReportParameter>();
                    paramters.Add(new ReportParameter("FilterTitle", TitleFilterReport));


                    ReportViewer1.LocalReport.SetParameters(paramters);

                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                }
                else if (RptOpt == "2")
                {

                    string jsonparam = "";
                    string apiurl = "/api/Quantify/GetDataReportMovAlquilerQuantify?guidfilter=" + filterobj;
                    var tas = Task.Run<List<DTORptMovEquipo>>(async () => await Http.PostGenericAsync<List<DTORptMovEquipo>>(apiurl, jsonparam));
                    var resu = tas.Result;
                    //List<DTORendimiento> rendis = resu.Rendimientos;
                    //List<DTONominaNovedad> noveds = resu.NominaNovedades;
                    //if (AddFieldDynam.ToUpper() == "TRUE")
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptMovEquiposAlquilerQuantifyMensual.rdlc";
                    //else
                    //    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiable.rdlc";

                    string TitleFilterReport = "";
                    if (resu.Count > 0)
                        TitleFilterReport = resu.First().TitleFilterReport;
                    List<ReportParameter> paramters = new List<ReportParameter>();
                    paramters.Add(new ReportParameter("FilterTitle", TitleFilterReport));


                    ReportViewer1.LocalReport.SetParameters(paramters);

                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                }
                else if (RptOpt == "3")
                {

                    string jsonparam = "";
                    string apiurl = "/api/Quantify/GetDataReportMovAlquilerQuantify?guidfilter=" + filterobj;
                    var tas = Task.Run<List<DTORptMovEquipo>>(async () => await Http.PostGenericAsync<List<DTORptMovEquipo>>(apiurl, jsonparam));
                    var resu = tas.Result;
                    //List<DTORendimiento> rendis = resu.Rendimientos;
                    //List<DTONominaNovedad> noveds = resu.NominaNovedades;
                    //if (AddFieldDynam.ToUpper() == "TRUE")
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptMovEquiposAlquilerQuantifyAnual.rdlc";
                    //else
                    //    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiable.rdlc";

                    string TitleFilterReport = "";
                    if (resu.Count > 0)
                        TitleFilterReport = resu.First().TitleFilterReport;
                    List<ReportParameter> paramters = new List<ReportParameter>();
                    paramters.Add(new ReportParameter("FilterTitle", TitleFilterReport));

                    ReportViewer1.LocalReport.SetParameters(paramters);

                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                }
                else if (RptOpt == "4" || RptOpt == "5" || RptOpt == "6")
                {

                    string jsonparam = "";
                    string apiurl = "/api/Quantify/GetDataReportMovAlquilerQuantify?guidfilter=" + filterobj;
                    var tas = Task.Run<List<DTORptMovEquipo>>(async () => await Http.PostGenericAsync<List<DTORptMovEquipo>>(apiurl, jsonparam));
                    var resu = tas.Result;
                    //List<DTORendimiento> rendis = resu.Rendimientos;
                    //List<DTONominaNovedad> noveds = resu.NominaNovedades;
                    //if (AddFieldDynam.ToUpper() == "TRUE")
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptMovEquiposAlquilerQuantifyDetalladoItem.rdlc";
                    //else
                    //    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiable.rdlc";

                    string TitleFilterReport = "";
                    if (resu.Count > 0)
                        TitleFilterReport = resu.First().TitleFilterReport;
                    List<ReportParameter> paramters = new List<ReportParameter>();
                    paramters.Add(new ReportParameter("FilterTitle", TitleFilterReport));


                    ReportViewer1.LocalReport.SetParameters(paramters);

                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                }
                else if (RptOpt == "7" || RptOpt == "8" || RptOpt == "9")
                {

                    string jsonparam = "";
                    string apiurl = "/api/Quantify/GetDataReportMovAlquilerQuantify?guidfilter=" + filterobj;
                    var tas = Task.Run<List<DTORptMovEquipo>>(async () => await Http.PostGenericAsync<List<DTORptMovEquipo>>(apiurl, jsonparam));
                    var resu = tas.Result;
                    //List<DTORendimiento> rendis = resu.Rendimientos;
                    //List<DTONominaNovedad> noveds = resu.NominaNovedades;
                    //if (AddFieldDynam.ToUpper() == "TRUE")
                    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptMovEquiposAlquilerQuantifyDetalladoItemFechas.rdlc";
                    //else
                    //    ReportViewer1.LocalReport.ReportEmbeddedResource = "adesoft.adeposx.report.Reports.RptComprasBiable.rdlc";
                    string titleDate = "";
                    string TitleFilterReport = "";

                    if (RptOpt == "7")
                    {
                        titleDate = "DIA";
                    }
                    else if (RptOpt == "8")
                    {
                        titleDate = "MES";
                    }
                    else if (RptOpt == "9")
                    {
                        titleDate = "AÑO";
                    }
                    if (resu.Count > 0)
                        TitleFilterReport = resu.First().TitleFilterReport;
                    List<ReportParameter> paramters = new List<ReportParameter>();
                    paramters.Add(new ReportParameter("FilterTitle", TitleFilterReport));
                    paramters.Add(new ReportParameter("TitleDate", titleDate));

                    ReportViewer1.LocalReport.SetParameters(paramters);

                    ReportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", resu));
                }
                ReportViewer1.LocalReport.Refresh();
            }
            catch
            {

            }
        }
    }
}