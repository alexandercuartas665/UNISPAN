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
    public class QuantifyController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        string ConnectionBiable = string.Empty;
        string ConnectionQuantify = string.Empty;

        public static List<DTOFiltersCompras> filterscompras = new List<DTOFiltersCompras>();
        public QuantifyController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            ConnectionBiable = configuration.GetValue<string>("ConnectionStrings:ConnectionBiable");
            ConnectionQuantify = configuration.GetValue<string>("ConnectionStrings:ConnectionQuantify");
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (connectionDB != null)
                this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public List<DTOYear> SelectAnosMovEquiposAlquiler()
        {
            DaoQuantify daosql = new DaoQuantify(ConnectionQuantify);
            List<DTOYear> list = daosql.ListYearsMovAlquilerEquipos();
            return list;
        }

        //TEST



        [HttpPost("GetDataReportMovAlquilerQuantify")]
        public List<DTORptMovEquipo> GetDataReportMovAlquilerQuantify(string guidfilter)
        {
            try
            {
                DTOFiltersCompras dtodata = filterscompras.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();
                if (dtodata != null)
                {
                    DaoQuantify daosql = new DaoQuantify(ConnectionQuantify);
                    DaoSqlBiable daobiable = new DaoSqlBiable(ConnectionBiable);
                    if (dtodata.TypeReportId == 2)
                    {//movimiento de equipos en alquiler mensual...
                        List<DTORptMovEquipo> list = daosql.SelectMovEquipos(dtodata);
                        List<DTOReportBalanceMonth> listdtos = daobiable.GetIncomeRental(dtodata, _dbcontext);
                        foreach (DTORptMovEquipo rt in list)
                        {//ingresos por renta de biable
                            rt.TonEnAlquiler = GetTonRentByPeriod(dtodata.TypeReportId, DateTime.MinValue, rt.Mes, rt.Ano);
                            rt.CantInvenTon = GetTonInventarioByPeriod(dtodata.TypeReportId, DateTime.MinValue, rt.Mes, rt.Ano);
                            DTOReportBalanceMonth ingre = listdtos.Where(x => x.IdYear == rt.Ano && x.IdMonth == rt.Mes).FirstOrDefault();
                            if (ingre != null)
                                rt.IngAlquiler = decimal.Round(ingre.Value.Value / (decimal)1000, 2);

                            if (rt.TonEnAlquiler > 0 && rt.IngAlquiler > 0)
                            {
                                rt.VrTon = decimal.Round(rt.IngAlquiler / rt.TonEnAlquiler, 2);
                            }

                        }

                        filterscompras.Remove(dtodata);
                        return list;
                    }
                    else if (dtodata.TypeReportId == 1)
                    {//movimiento de equipos en alquiler diario...
                        List<DTORptMovEquipo> list = daosql.SelectMovEquiposDiario(dtodata);
                        foreach (DTORptMovEquipo rt in list)
                        {//ingresos por renta de biable
                            rt.TonEnAlquiler = GetTonRentByPeriod(dtodata.TypeReportId, rt.DateMov, rt.Mes, rt.Ano);
                            rt.CantInvenTon = GetTonInventarioByPeriod(dtodata.TypeReportId, rt.DateMov, rt.Mes, rt.Ano);
                        }
                        filterscompras.Remove(dtodata);
                        return list;
                    }
                    if (dtodata.TypeReportId == 3)
                    {//movimiento de equipos en alquiler anual...
                        dtodata.multipleValuesMonth = new List<long>();
                        DTOViewRptCompra.GetMonths().Select(x => x.IdMonth).ToList().ForEach(t => ((IList<long>)dtodata.multipleValuesMonth).Add(t));
                        List<DTORptMovEquipo> list = daosql.SelectMovEquiposAnual(dtodata);
                        List<DTOReportBalanceMonth> listdtos = daobiable.GetIncomeRental(dtodata, _dbcontext);
                        foreach (DTORptMovEquipo rt in list)
                        {//ingresos por renta de biable
                            rt.TonEnAlquiler = GetTonRentByPeriod(dtodata.TypeReportId, DateTime.MinValue, rt.Mes, rt.Ano);
                            rt.CantInvenTon = GetTonInventarioByPeriod(dtodata.TypeReportId, DateTime.MinValue, rt.Mes, rt.Ano);
                            DTOReportBalanceMonth ingre = listdtos.Where(x => x.IdYear == rt.Ano && x.IdMonth == rt.Mes).FirstOrDefault();
                            if (ingre != null)
                                rt.IngAlquiler = decimal.Round(ingre.Value.Value / (decimal)1000, 2);

                            if (rt.TonEnAlquiler > 0 && rt.IngAlquiler > 0)
                            {
                                rt.VrTon = decimal.Round(rt.IngAlquiler / rt.TonEnAlquiler, 2);
                            }

                        }

                        filterscompras.Remove(dtodata);
                        return list;
                    }
                    else if (dtodata.TypeReportId == 4 || dtodata.TypeReportId == 5 || dtodata.TypeReportId == 6)
                    {//movimiento de equipos en alquiler agrupado por item...
                        List<DTORptMovEquipo> list = daosql.SelectMovEquiposDetalladoXItem(dtodata);
                        filterscompras.Remove(dtodata);
                        return list;
                    }
                    else if (dtodata.TypeReportId == 7 || dtodata.TypeReportId == 8 || dtodata.TypeReportId == 9)
                    {//movimiento de equipos en alquiler agrupado por item y fecha...
                        List<DTORptMovEquipo> list = daosql.SelectMovEquiposConItemDetalladoPorFecha(dtodata);
                        filterscompras.Remove(dtodata);
                        return list;
                    }
                    else if (dtodata.TypeReportId == 0)
                    {       ///REPORTE ANUAL ANTERIOR
                        //movimiento de equipos en alquiler año...
                        List<DTORptMovEquipo> list = daosql.SelectMovEquiposAnual(dtodata);
                        List<DTOReportBalanceMonth> listdtos = daobiable.GetIncomeRental(dtodata, _dbcontext);
                        foreach (DTORptMovEquipo rt in list)
                        {//ingresos por renta de biable
                            rt.TonEnAlquiler = GetTonRentByPeriod(dtodata.TypeReportId, DateTime.MinValue, rt.Mes, rt.Ano);
                            DTOReportBalanceMonth ingre = listdtos.Where(x => x.IdYear == rt.Ano).FirstOrDefault();
                            if (ingre != null)
                                rt.IngAlquiler = decimal.Round(ingre.Value.Value / (decimal)1000, 2);
                            if (rt.TonEnAlquiler > 0 && rt.IngAlquiler > 0)
                            {
                                rt.VrTon = decimal.Round(rt.IngAlquiler / rt.TonEnAlquiler, 2);
                            }
                        }

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
                    return new List<DTORptMovEquipo>();
                }
            }
            catch (Exception ex)
            {
                return new List<DTORptMovEquipo>();
            }
        }





        public decimal GetTonRentByPeriod(long TypeReportId, DateTime Dateday, int month, int year)
        {
            if (TypeReportId == 1)
            {//bodega arriendo diario....

                decimal totalrent = _dbcontext.SnapshotInventoryQuantifys.Where(x => x.DateInventary == Dateday)
                     .Sum(x => x.CantRentTon);
                return totalrent;
            }
            else if (TypeReportId == 2)
            {//bodega arriendo mensual....
                var quer = _dbcontext.SnapshotInventoryQuantifys.Where(x => x.MonthInve == month && x.YearInve == year);
                DateTime datelasrecord = DateTime.MinValue;
                if (quer.Count() > 0)
                    datelasrecord = quer.Max(x => x.DateInventary);
                if (datelasrecord != DateTime.MinValue)
                {
                    decimal totalrent = quer.Where(x => x.DateInventary == datelasrecord)
              .Sum(x => x.CantRentTon);
                    return totalrent;
                }
                else
                {
                    return 0;
                }
            }
            else if (TypeReportId == 3)
            {//bodega arriendo anual....

                var quer = _dbcontext.SnapshotInventoryQuantifys.Where(x => x.MonthInve == month && x.YearInve == year);
                DateTime datelasrecord = DateTime.MinValue;
                if (quer.Count() > 0)
                    datelasrecord = quer.Max(x => x.DateInventary);
                if (datelasrecord != DateTime.MinValue)
                {
                    decimal totalrent = quer.Where(x => x.DateInventary == datelasrecord)
              .Sum(x => x.CantRentTon);
                    return totalrent;
                }
                else
                {
                    return 0;
                }
                // var quer = _dbcontext.SnapshotInventoryQuantifys.Where(x => x.YearInve == year);
                // DateTime datelasrecord = DateTime.MinValue;
                // if (quer.Count() > 0)
                //     datelasrecord = quer.Max(x => x.DateInventary);
                // if (datelasrecord != DateTime.MinValue)
                // {
                //     decimal totalrent = quer.Where(x => x.DateInventary == datelasrecord)
                //.Sum(x => x.CantRentTon);
                //     return totalrent;
                // }
                // else
                // {
                //     return 0;
                // }
            }
            return 0;
        }


        public decimal GetTonInventarioByPeriod(long TypeReportId, DateTime Dateday, int month, int year)
        {
            if (TypeReportId == 1)
            {//bodega arriendo diario....

                decimal totalrent = _dbcontext.SnapshotInventoryQuantifys.Where(x => x.DateInventary == Dateday)
                     .Sum(x => x.CantInvenTon);
                return totalrent;
            }
            else if (TypeReportId == 2)
            {//bodega arriendo mensual....
                var quer = _dbcontext.SnapshotInventoryQuantifys.Where(x => x.MonthInve == month && x.YearInve == year);
                DateTime datelasrecord = DateTime.MinValue;
                if (quer.Count() > 0)
                    datelasrecord = quer.Max(x => x.DateInventary);
                if (datelasrecord != DateTime.MinValue)
                {
                    decimal totalrent = quer.Where(x => x.DateInventary == datelasrecord)
              .Sum(x => x.CantInvenTon);
                    return totalrent;
                }
                else
                {
                    return 0;
                }
            }
            else if (TypeReportId == 3)
            {//bodega arriendo anual....

                var quer = _dbcontext.SnapshotInventoryQuantifys.Where(x => x.MonthInve == month && x.YearInve == year);
                DateTime datelasrecord = DateTime.MinValue;
                if (quer.Count() > 0)
                    datelasrecord = quer.Max(x => x.DateInventary);
                if (datelasrecord != DateTime.MinValue)
                {
                    decimal totalrent = quer.Where(x => x.DateInventary == datelasrecord)
              .Sum(x => x.CantInvenTon);
                    return totalrent;
                }
                else
                {
                    return 0;
                }
                // var quer = _dbcontext.SnapshotInventoryQuantifys.Where(x => x.YearInve == year);
                // DateTime datelasrecord = DateTime.MinValue;
                // if (quer.Count() > 0)
                //     datelasrecord = quer.Max(x => x.DateInventary);
                // if (datelasrecord != DateTime.MinValue)
                // {
                //     decimal totalrent = quer.Where(x => x.DateInventary == datelasrecord)
                //.Sum(x => x.CantRentTon);
                //     return totalrent;
                // }
                // else
                // {
                //     return 0;
                // }
            }
            return 0;
        }

        public void AddFilterCompras(DTOFiltersCompras data)
        {
            filterscompras.Add(data);
        }
    }
}
