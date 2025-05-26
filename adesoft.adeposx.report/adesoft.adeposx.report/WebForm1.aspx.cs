using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using adesoft.adeposx.report.Models;
using adesoft.adeposx.report.WebAPIClient;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Text; // <-- Agregar esto
using System.IO; // ← Nueva línea

namespace adesoft.adeposx.report
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            GenerateReportAsHtml();
        }


        public void GenerateReportAsHtml()
        {
            try
            {
                ReportViewer ReportViewer1 = new ReportViewer();

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

                var exts = ReportViewer1.LocalReport.ListRenderingExtensions();
                foreach (var e in exts)
                {
                    Console.WriteLine($"Name={e.Name}, LocalizedName={e.LocalizedName}");
                }

                string mimeType, encoding, ext;
                string[] streams;
                Warning[] warnings;

                byte[] mhtmlBytes = ReportViewer1.LocalReport.Render(
                    "MHTML",
                    null, // o puedes especificar <DeviceInfo> si quieres controlar anchos/márgenes
                    out mimeType,
                    out encoding,
                    out ext,
                    out streams,
                    out warnings);




            }
            catch (Exception ex)
            {
                throw new Exception($"Error generando el reporte: {ex.Message}", ex);
            }
        }

    }
}