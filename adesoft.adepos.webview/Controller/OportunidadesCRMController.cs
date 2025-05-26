using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.Extensions;
using Microsoft.EntityFrameworkCore;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Bussines;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OportunidadesCRMController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        ConnectionDB connectionDB;
        public OportunidadesCRMController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public List<OportunidadesCRM> selectAll(OportunidadesCRM oportunidadesCRM)
        {
            if (oportunidadesCRM.TransOption == 1)
            {
                return _dbcontext.OportunidadesCRM.ToList();
            }
            else if (oportunidadesCRM.TransOption == 2)
            {
                if (oportunidadesCRM.FilterTipoEtapaId == 3)
                {//TODAS
                    List<OportunidadesCRM> listOports = _dbcontext.OportunidadesCRM.Where(t => t.FECHA_APERTURA_.Date >= oportunidadesCRM.FilterDateInit.Value.Date
                     && t.FECHA_APERTURA_.Date <= oportunidadesCRM.FilterDateEnd.Value.Date).ToList();
                    return listOports;
                }
                else if (oportunidadesCRM.FilterTipoEtapaId == 1)
                {//ABIERTAS
                    List<OportunidadesCRM> listOports = _dbcontext.OportunidadesCRM.Where(t => t.FECHA_APERTURA_.Date >= oportunidadesCRM.FilterDateInit.Value.Date
                     && t.FECHA_APERTURA_.Date <= oportunidadesCRM.FilterDateEnd.Value.Date && t.COD_ETAPA != "F11").ToList();
                    return listOports;
                }
                else if (oportunidadesCRM.FilterTipoEtapaId == 2)
                {//CERRADAS
                    List<OportunidadesCRM> listOports = _dbcontext.OportunidadesCRM.Where(t => t.FECHA_APERTURA_.Date >= oportunidadesCRM.FilterDateInit.Value.Date
                     && t.FECHA_APERTURA_.Date <= oportunidadesCRM.FilterDateEnd.Value.Date && t.COD_ETAPA == "F11").ToList();
                    return listOports;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return _dbcontext.OportunidadesCRM.ToList();
            }
        }


        public OportunidadesCRM selectById(OportunidadesCRM oportunidadesCRM)
        {
            if (oportunidadesCRM.TransOption == 1)
            {
                OportunidadesCRM dat = _dbcontext.OportunidadesCRM.Where(t => t.OportunidadID == oportunidadesCRM.OportunidadID).FirstOrDefault();
                return dat;
            }
            else
            {
                return new OportunidadesCRM();
            }
        }

        public OportunidadesCRM Create(OportunidadesCRM dat)
        {
            OportunidadesCRM find = _dbcontext.OportunidadesCRM.Where(x => x.CONSECUTIVO == dat.CONSECUTIVO).FirstOrDefault();
            if (find == null)
            {
                _dbcontext.OportunidadesCRM.Add(dat);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return dat;
        }

        public static List<DTOFiltersCompras> filtersReport = new List<DTOFiltersCompras>();
        [HttpPost("GetDataReport")]
        public List<OportunidadesCRM> GetDataReport(string guidfilter)
        {
            DTOFiltersCompras dtodata = filtersReport.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();
            if (dtodata != null)
            {
                if (dtodata.TypeReportId == 1)
                {
                    filtersReport.Remove(dtodata);
                    return dtodata.Oportunidades;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return new List<OportunidadesCRM>();
            }
        }

        public void AddFilterCompras(DTOFiltersCompras data)
        {
            filtersReport.Add(data);
        }
        public OportunidadesCRM Update(OportunidadesCRM dat)
        {
            OportunidadesCRM find = _dbcontext.OportunidadesCRM.Where(x => x.OportunidadID == dat.OportunidadID).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<OportunidadesCRM>(dat).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return dat;
        }

        public void StartSyncCRM()
        {
            ConnectorCRM connectCRM = new ConnectorCRM(_configuration, connectionDB);
            connectCRM.StartSyncCRM(true).Wait();
        }


    }
}
