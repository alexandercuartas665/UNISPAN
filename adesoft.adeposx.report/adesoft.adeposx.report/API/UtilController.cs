using adesoft.adeposx.report.BussinesLogic;
using adesoft.adeposx.report.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace adesoft.adeposx.report.API
{

    public class UtilController : ApiController
    {

        /// <summary>
        /// Metodo para leer documentos desde archivo para el cliente unispan
        /// </summary>
        /// <param name="pathFile"></param>
        /// <param name="bodega"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> ReadInventaryOfDocumentFormFile(string pathFile, string bodega)
        {//Ojo el body es para cuando viene en el header o en cualquier otra part del mensaje

            List<DTOInventary> list = BLInventaryUtils.ReadInventaryOfDocumentOleDB(pathFile, bodega);
            return Ok(list);
        }


        [HttpPost]
        public async Task<IHttpActionResult> ReadOrderOfFile(DTOTransaction transaction)
        {//Ojo el body es para cuando viene en el header o en cualquier otra part del mensaje
         //  string g = await Request.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (transaction.TransactionGenericId == 10)//importar orden de despacho desde archivo
            {
                DTOTransaction trans = BLInventaryUtils.CreateFileAndReadOrderDistpatch(transaction);
                return Ok(trans);
            }
            else if (transaction.TransactionGenericId == 11)//importar Items archivo
            {
                DTOTransaction trans = BLInventaryUtils.CreateFileAnReaderItems(transaction);
                return Ok(trans);
            }
            return BadRequest();   
        }

        [HttpPost]
        public async Task<IHttpActionResult> ReadDocumentPersonOfFile(string pathFile)
        {
            List<DTOTercero> list = BLInventaryUtils.ReadPersonOfDocumentOleDB(pathFile);
            return Ok(list);
        }


        [HttpPost]
        public async Task<IHttpActionResult> ReadEquivalence85([FromBody] string FilBase64, string para1,string para2)
        {//Ojo el body es para cuando viene en el header o en cualquier otra part del mensaje

            List<DTOInventary> list = BLInventaryUtils.ReadEquivalence85(FilBase64);
            return Ok(list);
        }



    }
}
