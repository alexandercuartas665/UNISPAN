using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.Extensions;
using adesoft.adepos.webview.Data.DAO;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class BiableController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        string ConnectionBiable = string.Empty;


        public static List<DTOFiltersCompras> filterscompras = new List<DTOFiltersCompras>();
        public BiableController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            ConnectionBiable = configuration.GetValue<string>("ConnectionStrings:ConnectionBiable");
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (connectionDB != null)
                this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public List<DTOTercero> selectAllProveedores(DTOTercero dtoTercero)
        {
            DaoSqlBiable daosql = new DaoSqlBiable(ConnectionBiable);
            List<DTOTercero> list = daosql.SelectAllProveedores();
            return list;
        }

        public List<DTOYear> SelectAnosCompras()
        {
            DaoSqlBiable daosql = new DaoSqlBiable(ConnectionBiable);
            List<DTOYear> list = daosql.SelectAnosCompras();
            return list;
        }

        public List<DTOYear> SelectAnosMovInventario(int TipoMovId)
        {
            DaoSqlBiable daosql = new DaoSqlBiable(ConnectionBiable);
            List<DTOYear> list = daosql.SelectAnosMovInventario(TipoMovId);
            return list;
        }
        public List<DTOYear> SelectAnosContable()
        {
            DaoSqlBiable daosql = new DaoSqlBiable(ConnectionBiable);
            List<DTOYear> list = daosql.ListYearsContable();
            return list;
        }

        public List<DTOYear> BuildAnosParameter()
        {
            List<DTOYear> listm = new List<DTOYear>();
            int yearend = DateTime.Now.Year;
            int yearinit = yearend - 5;
            for (int i = yearinit; i <= yearend; i++)
            {
                DTOYear a = new DTOYear();
                a.Name = i.ToString();
                a.IdYear = long.Parse(a.Name);
                listm.Add(a);
            }
            return listm;
        }


        [HttpPost("GetDataReport")]
        public List<DTOComprasReport> GetDataReport(string guidfilter)
        {
            DaoSqlBiable daosql = new DaoSqlBiable(ConnectionBiable);
            DTOFiltersCompras dtodata = filterscompras.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();
            if (dtodata != null)
            {
                if (dtodata.TypeReportId == 1)
                {
                    //Reporte x item mensual --ViewRptCompras1
                    List<DTOComprasReport> list = daosql.SelectAllCompras(dtodata);
                    filterscompras.Remove(dtodata);
                    return list;
                }
                else if (dtodata.TypeReportId == 2)
                {
                    //Reporte x proveedor mensual --ViewRptCompras1
                    List<DTOComprasReport> list = daosql.SelectAllComprasXProveedor(dtodata);
                    filterscompras.Remove(dtodata);
                    return list;
                }
                else if (dtodata.TypeReportId == 3)
                {
                    List<DTOComprasReport> list = daosql.SelectAllComprasDetallado(dtodata);
                    filterscompras.Remove(dtodata);
                    return list;
                }
                else if (dtodata.TypeReportId == 4)
                {
                    //Reporte comparativo anual proveedor --ViewRptCompras4
                    List<DTOComprasReport> list = daosql.SelectAllComprasXProveedorAnual(dtodata);
                    filterscompras.Remove(dtodata);
                    return list;
                }
                else if (dtodata.TypeReportId == 5)
                {
                    //Reporte comparativo anual item --ViewRptCompras5
                    List<DTOComprasReport> list = daosql.SelectAllComprasAnual(dtodata);
                    filterscompras.Remove(dtodata);
                    return list;
                }
                else if (dtodata.TypeReportId == 6)
                {
                    //Reporte salidas de inventario --viewrptsalidainventario
                    List<DTOComprasReport> list = daosql.SelectAllMovInventario(dtodata);
                    filterscompras.Remove(dtodata);
                    return list;
                }
                else if (dtodata.TypeReportId == 7)
                {
                    //Reporte salidas de inventario --viewrptsalidainventario
                    List<DTOComprasReport> list = daosql.SelectAllMovInventarioMensual(dtodata);
                    filterscompras.Remove(dtodata);
                    return list;
                }
                else if (dtodata.TypeReportId == 8)
                {
                    //Reporte salidas de inventario --viewrptsalidainventario
                    List<DTOComprasReport> list = daosql.SelectAllMovInventario(dtodata);
                    filterscompras.Remove(dtodata);
                    return list;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return new List<DTOComprasReport>();
            }
        }


        [HttpPost("GetDataReportContable")]
        public List<DTOReportBalanceMonth> GetDataReportContable(string guidfilter)
        {
            DaoSqlBiable daosql = new DaoSqlBiable(ConnectionBiable);
            DTOFiltersCompras dtodata = filterscompras.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();
            if (dtodata != null)
            {
                if (dtodata.TypeReportId == 1)
                {
                    //Reporte x item mensual --ViewRptCompras1
                    List<DetailReportDynamic> listdeta = _dbcontext.DetailReportDynamics.Where(x => x.ReportDynamicId == 1).ToList();
                    List<DTOReportBalanceMonth> list = daosql.SelectDTOReportBalanceMonth(listdeta, dtodata, _dbcontext);
                    filterscompras.Remove(dtodata);
                    return list;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return new List<DTOReportBalanceMonth>();
            }
        }
        public void AddFilterCompras(DTOFiltersCompras data)
        {
            filterscompras.Add(data);
        }
    }
}
