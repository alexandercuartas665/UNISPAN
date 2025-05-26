using adesoft.adeposx.report.BussinesLogic;
using adesoft.adeposx.report.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace adesoft.adeposx.report.API
{
    public class CertificateController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> GenerateCertificate(DTOTercero tercero)
        {
            DTOTercero Terreturn = BLTercero.GenerateCertificate(tercero);
            return Ok(Terreturn);
        }

        [HttpPost]
        public async Task<IHttpActionResult> ReadReportBalanceMonth(string pathFile)
        {//Ojo el body es para cuando viene en el header o en cualquier otra part del mensaje
         //  string g = await Request.Content.ReadAsStringAsync().ConfigureAwait(false);
            List<DTOReportBalanceMonth> trans = BLInventaryUtils.ReadConfigReportDynamic(pathFile);
            return Ok(trans);
        }

        [HttpPost]
        public async Task<IHttpActionResult> ReadParamExcel(string pathFile,string param2)
        {//Ojo el body es para cuando viene en el header o en cualquier otra part del mensaje
         //  string g = await Request.Content.ReadAsStringAsync().ConfigureAwait(false);
            List<DTOParamContable> trans = BLInventaryUtils.ReadParametersOfExcel(pathFile);
            return Ok(trans);
        }
    }
}