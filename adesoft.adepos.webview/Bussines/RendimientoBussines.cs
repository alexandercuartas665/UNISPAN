using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using adesoft.adepos.webview.Data.DTO;

namespace adesoft.adepos.webview.Bussines
{
    public class RendimientoBussines
    {
        public AdeposDBContext context { get; set; }

        public IConfiguration configuration;
        public RendimientoBussines(AdeposDBContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }
        /// <summary>
        /// metodo para generacion de rendimiento
        /// </summary>
        /// <param name="rend"></param>
        /// <returns></returns>
        public List<Rendimiento> GenerateRendimiento(Rendimiento rend)
        {
            DateTime dateinit = DateTime.MinValue;
            DateTime dateend = DateTime.MinValue;


            List<Parameter> parameters = new List<Parameter>();
            parameters = context.Parameters.Where(x => x.Module == "PRODUCCION").ToList();
            DTOParamRango metaMinimaModel = new DTOParamRango();
            Parameter ParamMetasMinimas = parameters.Where(x => x.NameIdentify == "DailyMinimunMeta").FirstOrDefault();
            if (!string.IsNullOrEmpty(ParamMetasMinimas.Value2))
                metaMinimaModel = JsonConvert.DeserializeObject<DTOParamRango>(ParamMetasMinimas.Value2);

            if (rend.DateActivity != DateTime.MinValue)
            {
                dateinit = rend.DateActivity.Date;
                dateend = rend.DateActivity.Date;
            }
            else
            {
                dateinit = new DateTime((int)rend.YearId, (int)rend.MonthId, 1);
                int lastDayOfMonth = DateTime.DaysInMonth((int)rend.YearId, (int)rend.MonthId);
                dateend = new DateTime((int)rend.YearId, (int)rend.MonthId, lastDayOfMonth);
            }

            Expression<Func<Production, bool>> expre = (x => x.DateProduction.Date >= dateinit && x.DateProduction.Date <= dateend);
            Expression<Func<Production, bool>> expreActiv = (x => true);
            Expression<Func<Production, bool>> expreTercer = (x => true);
            if (rend.TypeActivityId != 0)
            {
                expreActiv = (x => x.TypeActivityId == rend.TypeActivityId);
            }
            if (rend.TerceroId != 0)
            {
                expreTercer = (x => x.DetailTerceros.Where(t => t.TerceroId == rend.TerceroId).Count() > 0);
            }
            List<Production> listprod = context.Productions.Include(x => x.DetailTerceros).Include(x => x.DetailProductions).Where(expre).Where(expreActiv).Where(expreTercer).ToList();
            List<long> listeterceros = new List<long>();

            listprod.ForEach(x => x.DetailTerceros.ForEach(t => listeterceros.Add(t.TerceroId)));
            List<Tercero> terceros = context.Terceros.Where(x => listeterceros.Contains(x.TerceroId)).ToList();

            List<Item> listitems = context.Items.ToList();

            List<Category> listcategor = context.Categorys.Where(x => x.TypeCategoryId == 2).ToList();
            List<TypeActivity> TypeActivitys = context.TypeActivitys.ToList();
            foreach (TypeActivity typ in TypeActivitys)
            {
                typ.Category = listcategor.Where(x => x.CategoryId == typ.CategoryId).First();
                typ.CategoryName = typ.Category.Name;
            }

            context.DetachAll();

            List<Rendimiento> lisreturns = new List<Rendimiento>();
            foreach (Production p in listprod)
            {
                foreach (DetailProductionTercero detTerc in p.DetailTerceros)
                {

                    Rendimiento rendi = lisreturns.Where(x => x.DateActivity == p.DateProduction.Date && x.TerceroId == detTerc.TerceroId && x.TypeActivityId == p.TypeActivityId).FirstOrDefault();
                    if (rendi != null)
                    {
                        //si ya existe
                    }
                    else
                    {

                        rendi = new Rendimiento();
                        rendi.Tercero = terceros.Where(x => x.TerceroId == detTerc.TerceroId).FirstOrDefault();
                        rendi.DateActivity = p.DateProduction.Date;
                        rendi.TerceroId = detTerc.TerceroId;
                        rendi.TypeActivityId = p.TypeActivityId;
                        rendi.TypeActivity = TypeActivitys.Where(x => x.TypeActivityId == rendi.TypeActivityId).First();
                        rendi.YearId = rendi.DateActivity.Year;
                        rendi.MonthId = rendi.DateActivity.Month; rendi.IdDay = rendi.DateActivity.Day;
                        rendi.TerceroName = rendi.Tercero.FullName;
                        rendi.CodeTercero = long.Parse(rendi.Tercero.CodeEnterprise);
                        rendi.TypeActivityName = rendi.TypeActivity.Name;
                        rendi.CategoryActivityName = rendi.TypeActivity.CategoryName;
                        lisreturns.Add(rendi);
                    }
                    foreach (DetailProduction dp in p.DetailProductions)
                    {
                        Item item = listitems.Where(x => x.ItemId == dp.ItemId).FirstOrDefault();
                        decimal cantmedi = 0;
                        if (item.categoryMedicionId == 3)//ENCOFRADOS M2
                        {
                            cantmedi = item.Area;
                            rendi.UndMedida = "m2";
                            DTOParamRangoDetail metaMinimaDet = metaMinimaModel.Details.Where(x => x.ActividadId == p.TypeActivityId).FirstOrDefault();
                            if (metaMinimaDet != null)
                            {
                                rendi.MinimumGoal = metaMinimaDet.MetaMinimaUnd;
                                rendi.MinimumGoalTon = metaMinimaDet.MetaMinimaTon;
                            }
                        }
                        else if (item.categoryMedicionId == 4)//ACCESORIOS KG
                        {
                            cantmedi = item.Weight;
                            rendi.UndMedida = "kg";
                            DTOParamRangoDetail metaMinimaDet = metaMinimaModel.Details.Where(x => x.ActividadId == p.TypeActivityId).FirstOrDefault();
                            if (metaMinimaDet != null)
                            {
                                rendi.MinimumGoal = metaMinimaDet.MetaMinimaUnd;
                                rendi.MinimumGoalTon = metaMinimaDet.MetaMinimaTon;
                            }
                        }
                        rendi.Cant += decimal.Round(((dp.Cant * cantmedi) / ((decimal)p.DetailTerceros.Count)), 2);
                        //Se divide en 1000 para que sean toneladas
                        rendi.CantTon += decimal.Round(((dp.Cant * item.Weight) / ((decimal)p.DetailTerceros.Count)) / (decimal)1000, 4);
                    }
                    if (lisreturns.Where(x => x.DateActivity == p.DateProduction.Date && x.TerceroId == detTerc.TerceroId && x.TypeActivityId == p.TypeActivityId
                    && x.Cant > 0).Count() == 1)
                    {
                        rendi.AcumOperario = 1;
                    }

                }
            }
            if (listprod.Count > 0)
            {
                List<Rendimiento> listgroup = lisreturns.GroupBy(x => new { x.TerceroId, x.TypeActivityId }).Select(x => x.FirstOrDefault()).ToList();
                //llenar ausentismos
                foreach (Tercero ter in terceros)
                {
                    //terner en cuenta que si metes regstros de movedades esto puede fallar
                    //probar bien lo de las novedades del mes
                    List<NominaNovedad> listausentis = context.NominaNovedads.Where(x => ((x.DayInit >= dateinit && x.DayInit <= dateend) || (x.DayEnd >= dateinit && x.DayEnd <= dateend)
                    || (x.DayInit <= dateinit && x.DayEnd >= dateend))
                    && x.TerceroId == ter.TerceroId && x.StateNovedad != 3 && x.CodeNovedadId == 15 && x.NominaProgramationId == 0).ToList();
                    foreach (NominaNovedad nm in listausentis)
                    {
                        if (nm.DayInit < dateinit && dateend >= nm.DayEnd)
                        {
                            ter.AuxCant += nm.DayEnd.Subtract(dateinit).Days + 1;
                        }
                        else if (nm.DayInit < dateinit && dateend <= nm.DayEnd)
                        {
                            ter.AuxCant += dateend.Subtract(dateinit).Days + 1;
                        }
                        else if (nm.DayInit > dateinit && nm.DayInit < dateend && nm.DayEnd > dateend)
                        {
                            ter.AuxCant += dateend.Subtract(nm.DayInit).Days + 1;
                        }
                        else
                        {
                            ter.AuxCant += nm.DayEnd.Subtract(nm.DayInit).Days + 1;
                        }
                    }

                    List<NominaNovedad> listnoveds = context.NominaNovedads.Where(x => ((x.DayInit >= dateinit && x.DayInit <= dateend) || (x.DayEnd >= dateinit && x.DayEnd <= dateend)
                  || (x.DayInit <= dateinit && x.DayEnd >= dateend))
                  && x.TerceroId == ter.TerceroId && x.StateNovedad != 3 && x.CodeNovedadId == 16 && x.NominaProgramationId == 0).ToList();
                    ter.ListNovedades = listnoveds;
                }


                foreach (Rendimiento ren in listgroup)
                {
                    List<Rendimiento> listux = lisreturns.Where(x => x.TypeActivityId == ren.TypeActivityId && x.TerceroId == ren.TerceroId).ToList();
                    decimal TotalMedicion = listux.Sum(x => x.Cant);
                    //aqui puede ir lo de ausentismo
                    string tipomedid = string.Empty;
                    if (ren.TypeActivity.CategoryId == 3)//ENCOFRADOS M2
                    {
                        tipomedid = "ParametrosM2";
                    }
                    else if (ren.TypeActivity.CategoryId == 4)//ACCESORIOS KG
                    {
                        tipomedid = "ParametrosKG";
                    }
                    decimal daysausent = ren.Tercero.AuxCant;//dias de ausentismo
                    //bonificacion x medida
                    decimal bonificacionMed = returnBonus(ren.DateActivity, ren.TypeActivityId, tipomedid, TotalMedicion, parameters);
                    //bonificacion x ausentismo y tipo de actividad para ausentismo siempre es 1
                    decimal bonificacionAus = returnBonus(ren.DateActivity, 1, "ParametrosAusen", daysausent, parameters);
                    //RELLENAR CON 0s
                    listux.ForEach(x =>
                    {
                        x.TotalMedicion = TotalMedicion; x.TotalBonificacion = bonificacionMed;
                        x.PorcentajeBonificacion = bonificacionAus; x.DiasAusentismo = daysausent;
                        x.ValorAPagar = decimal.Round(x.TotalBonificacion * x.PorcentajeBonificacion / (decimal)100, 2);
                    });
                    //
                    //Esto para totalizar sin repetir valores
                    Rendimiento rendvalueunique = listux.First();
                    rendvalueunique.TotalMedicionNotRepeat = TotalMedicion;
                    rendvalueunique.TotalBonificacionNotRepeat = bonificacionMed;
                    rendvalueunique.ValorAPagarNotRepeat = decimal.Round(bonificacionMed * bonificacionAus / (decimal)100, 2);
                    //recorro para llenar vacios
                    for (int i = dateinit.Day; i <= dateend.Day; i++)
                    {
                        Rendimiento rendvacio = listux.Where(x => x.IdDay == i).FirstOrDefault();
                        if (rendvacio == null)
                        {
                            rendvacio = new Rendimiento();
                            rendvacio.CodeTercero = ren.CodeTercero; rendvacio.TerceroId = ren.TerceroId;
                            rendvacio.DateActivity = new DateTime(dateinit.Year, dateinit.Month, i).Date;
                            rendvacio.Tercero = ren.Tercero; rendvacio.TypeActivityId = ren.TypeActivityId;
                            rendvacio.TypeActivity = ren.TypeActivity;
                            rendvacio.TypeActivityName = ren.TypeActivityName;
                            rendvacio.TerceroName = ren.TerceroName;
                            rendvacio.UndMedida = ren.UndMedida;
                            rendvacio.IdDay = i; rendvacio.YearId = ren.YearId;
                            rendvacio.MonthId = ren.MonthId;
                            rendvacio.TotalMedicion = TotalMedicion; rendvacio.TotalBonificacion = bonificacionMed;
                            rendvacio.PorcentajeBonificacion = bonificacionAus; rendvacio.DiasAusentismo = daysausent;
                            rendvacio.ValorAPagar = rend.ValorAPagar;
                            //METAS MINIMAS
                            DTOParamRangoDetail metaMinimaDet = metaMinimaModel.Details.Where(x => x.ActividadId == ren.TypeActivityId).FirstOrDefault();
                            if (metaMinimaDet != null)
                            {
                                rendvacio.MinimumGoal = metaMinimaDet.MetaMinimaUnd;
                                rendvacio.MinimumGoalTon = metaMinimaDet.MetaMinimaTon;
                            }
                            lisreturns.Add(rendvacio);
                        }

                        rendvacio.ReportNovedades = ren.Tercero.ListNovedades.Where(x => rendvacio.DateActivity >= x.DayInit && rendvacio.DateActivity <= x.DayEnd).ToList();
                        rendvacio.ReportaNovedad = rendvacio.ReportNovedades.Count() > 0;
                    }

                }


            }
            return lisreturns;
        }


        public List<Rendimiento> GenerateRendimientoByItem(Rendimiento rend)
        {
            DateTime dateinit = DateTime.MinValue;
            DateTime dateend = DateTime.MinValue;
            //List<Parameter> parameters = new List<Parameter>();
            //parameters = context.Parameters.Where(x => x.Module == "PRODUCCION").ToList();
            if (rend.DateActivity != DateTime.MinValue)
            {
                dateinit = rend.DateActivity.Date;
                dateend = rend.DateActivity.Date;
            }
            else
            {
                dateinit = new DateTime((int)rend.YearId, (int)rend.MonthId, 1);
                int lastDayOfMonth = DateTime.DaysInMonth((int)rend.YearId, (int)rend.MonthId);
                dateend = new DateTime((int)rend.YearId, (int)rend.MonthId, lastDayOfMonth);
            }

            Expression<Func<Production, bool>> expre = (x => x.DateProduction.Date >= dateinit && x.DateProduction.Date <= dateend);
            Expression<Func<Production, bool>> expreActiv = (x => true);
            Expression<Func<Production, bool>> expreTercer = (x => true);
            if (rend.TypeActivityId != 0)
            {
                expreActiv = (x => x.TypeActivityId == rend.TypeActivityId);
            }
            if (rend.TerceroId != 0)
            {
                expreTercer = (x => x.DetailTerceros.Where(t => t.TerceroId == rend.TerceroId).Count() > 0);
            }
            List<Production> listprod = context.Productions.Include(x => x.DetailTerceros).Include(x => x.DetailProductions).Where(expre).Where(expreActiv).Where(expreTercer).ToList();
            List<long> listeterceros = new List<long>();

            listprod.ForEach(x => x.DetailTerceros.ForEach(t => listeterceros.Add(t.TerceroId)));
            List<Tercero> terceros = context.Terceros.Where(x => listeterceros.Contains(x.TerceroId)).ToList();

            List<Item> listitems = context.Items.ToList();

            List<Category> listcategor = context.Categorys.Where(x => x.TypeCategoryId == 2).ToList();
            List<TypeActivity> TypeActivitys = context.TypeActivitys.ToList();
            foreach (TypeActivity typ in TypeActivitys)
            {
                typ.Category = listcategor.Where(x => x.CategoryId == typ.CategoryId).First();
                typ.CategoryName = typ.Category.Name;
            }

            context.DetachAll();

            List<Rendimiento> lisreturns = new List<Rendimiento>();
            foreach (Production p in listprod)
            {
                foreach (DetailProductionTercero detTerc in p.DetailTerceros)
                {
                    //Rendimiento rendi = lisreturns.Where(x => x.DateActivity == p.DateProduction.Date && x.TerceroId == detTerc.TerceroId && x.TypeActivityId == p.TypeActivityId).FirstOrDefault();
                    //if (rendi != null)
                    //{
                    //    //si ya existe
                    //}
                    //else
                    //{
                    //}
                    foreach (DetailProduction dp in p.DetailProductions)
                    {
                        Rendimiento rendi = new Rendimiento();
                        rendi.Tercero = terceros.Where(x => x.TerceroId == detTerc.TerceroId).FirstOrDefault();
                        rendi.DateActivity = p.DateProduction.Date;
                        rendi.TerceroId = detTerc.TerceroId;
                        rendi.TypeActivityId = p.TypeActivityId;
                        rendi.TypeActivity = TypeActivitys.Where(x => x.TypeActivityId == rendi.TypeActivityId).First();
                        rendi.YearId = rendi.DateActivity.Year;
                        rendi.MonthId = rendi.DateActivity.Month; rendi.IdDay = rendi.DateActivity.Day;
                        rendi.TerceroName = rendi.Tercero.FullName;
                        rendi.CodeTercero = long.Parse(rendi.Tercero.CodeEnterprise);
                        rendi.TypeActivityName = rendi.TypeActivity.Name;
                        rendi.CategoryActivityName = rendi.TypeActivity.CategoryName;
                        if (lisreturns.Where(x => x.DateActivity == p.DateProduction.Date && x.TerceroId == detTerc.TerceroId).Count() == 0)
                        {
                            rendi.AcumOperario = 1;
                        }
                        lisreturns.Add(rendi);

                        Item item = listitems.Where(x => x.ItemId == dp.ItemId).FirstOrDefault();
                        decimal cantmedi = 0;
                        rendi.ItemBarcode = item.Barcode;
                        rendi.ItemName = item.Description;
                        if (item.categoryMedicionId == 3)//ENCOFRADOS M2
                        {
                            cantmedi = item.Area;
                            rendi.UndMedida = "m2";
                        }
                        else if (item.categoryMedicionId == 4)//ACCESORIOS KG
                        {
                            cantmedi = item.Weight;
                            rendi.UndMedida = "kg";
                        }
                        rendi.Cant = dp.Cant / p.DetailTerceros.Count;  // decimal.Round(((dp.Cant * cantmedi) / ((decimal)p.DetailTerceros.Count)), 2);
                        //Se divide en 1000 para que sean toneladas
                        //rendi.CantTon = decimal.Round(((dp.Cant * item.Weight) / ((decimal)p.DetailTerceros.Count)) / (decimal)1000, 4);
                    }
                }
            }
            return lisreturns;
        }

        public decimal returnBonus(DateTime dateref, long TypeActivityId, string tipomedid, decimal valor, List<Parameter> parameters)
        {
            Parameter parammodel = parameters.Where(x => x.NameIdentify == tipomedid).FirstOrDefault();
            if (!string.IsNullOrEmpty(parammodel.Value2))
            {
                List<DTOParamRango> Listparams = JsonConvert.DeserializeObject<List<DTOParamRango>>(parammodel.Value2);
                DTOParamRango dto = Listparams.Where(x => x.YearId == dateref.Year && x.MonthId == dateref.Month &&
                 x.TypeActivityId == TypeActivityId).FirstOrDefault();
                if (dto != null)
                {
                    DTOParamRangoDetail det = dto.Details.Where(x => valor >= x.RangoInit && valor <= x.RangoEnd).FirstOrDefault();
                    if (det != null)
                    {
                        return det.Bono;
                    }
                    else
                    {
                        return 0;
                    }

                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }


    }
}
