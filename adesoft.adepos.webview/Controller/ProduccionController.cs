using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.webview.Bussines;
using adesoft.adepos.Extensions;
using Microsoft.EntityFrameworkCore;
using adesoft.adepos.webview.Data.DTO;
using System.Globalization;
using System.IO;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduccionController : ControllerBase
    {
        private AdeposDBContext _dbcontext;
        private readonly IConfiguration _configuration;
        private readonly TokenAuthenticationStateProvider _tookenState;
        public static List<DTOFiltersCompras> filtersreports = new List<DTOFiltersCompras>();
        public ProduccionController(IConfiguration configuration, TokenAuthenticationStateProvider tookenState, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _tookenState = tookenState;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (connectionDB == null)
                connectionDB = SecurityService.GetConnectionDefault();
            if (connectionDB != null)
                _dbcontext = new AdeposDBContext(connectionDB.Connection);

        }

        [HttpPost("RecibirMediciones")]
        //  public Production RecibirMediciones(Production production)
        public DTOSendMediciones RecibirMediciones(DTOSendMediciones medicion)
        {
            try
            {
                Production proddel = _dbcontext.Productions.Where(x => x.ConsecutiveMobileId == medicion.GUID).FirstOrDefault();
                if (proddel != null)
                {
                    List<DetailProductionTercero> detter = _dbcontext.DetailProductionTerceros.Where(x => x.ProductionId == proddel.ProductionId).ToList();
                    _dbcontext.DetailProductionTerceros.RemoveRange(detter);

                    List<DetailProduction> detItems = _dbcontext.DetailProductions.Where(x => x.ProductionId == proddel.ProductionId).ToList();
                    _dbcontext.DetailProductions.RemoveRange(detItems);

                    _dbcontext.Productions.Remove(proddel);
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();
                    DeletePhotos(proddel);
                }

                Production prod = new Production();
                prod.ConsecutiveMobileId = medicion.GUID;
                prod.DateProduction = DateTime.ParseExact(medicion.DateProduction, "yyyy-MM-dd", CultureInfo.GetCultureInfo("Es-co"));
                prod.TypeActivityId = medicion.TypeActivity;
                prod.UserAppId = medicion.UserAppId;
                prod.Photo1Base64 = medicion.photo;
                List<Tercero> terceros = _dbcontext.Terceros.Where(x => x.TypeTerceroId == 5).ToList();

                foreach (DTOSendDetailTerceros det in medicion.detailTerceros)
                {
                    DetailProductionTercero detTercer = new DetailProductionTercero();
                    Tercero terc = terceros.Where(x => x.CodeEnterprise == det.CodeEnterprise).First();
                    detTercer.TerceroId = terc.TerceroId;
                    prod.DetailTerceros.Add(detTercer);
                }

                foreach (DTOSendDetailItems det in medicion.detailItems)
                {
                    DetailProduction detpro = new DetailProduction();
                    detpro.ItemId = det.itemId;
                    detpro.Cant = det.count;
                    detpro.Photo1Base64 = det.url_photo1;
                    detpro.Observation = det.observations;
                    prod.DetailProductions.Add(detpro);
                }
                prod.TransOption = 1;
                Create(prod);
                CreateOrUpdatePhoto(prod);
                Update(prod);
                //DTOFiltersCompras filterdto = filtersreports.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();
                //if (filterdto != null)
                //{
                //    if (filterdto.TypeReportId == 1)
                //    {
                //        filtersreports.Remove(filterdto);
                //        return filterdto.Rendimientos;
                //    }
                //}
                DTOSendMediciones dto = new DTOSendMediciones() { ResponseTransaction = "OK", GUID = "OK" };
                return dto;
            }
            catch (Exception ex)
            {
                DTOSendMediciones dto = new DTOSendMediciones() { ResponseTransaction = "FALLO", GUID = "FALLO" };
                return dto;
            }
        }

        [HttpPost("RecibirNovedades")]
        public DTONovedadesProduction RecibirNovedades(DTONovedadesProduction DtoNove)
        {
            try
            {
                if (_dbcontext.NominaNovedads.Where(x => x.ConsecutiveMobileId == DtoNove.NOVEDAD_ID).Count() > 0)
                {
                    DTONovedadesProduction dto3 = new DTONovedadesProduction() { responsetransaction = "OK" };
                    return dto3;
                }
                Tercero terc = _dbcontext.Terceros.Where(x => x.CodeEnterprise == DtoNove.OPERARIO_ID).First();

                NominaNovedad nom = new NominaNovedad();
                nom.DayInit = DateTime.ParseExact(DtoNove.FECHA_INI, "yyyy-MM-dd", CultureInfo.GetCultureInfo("Es-co"));
                nom.DayEnd = DateTime.ParseExact(DtoNove.FECHA_FIN, "yyyy-MM-dd", CultureInfo.GetCultureInfo("Es-co"));
                nom.HoursNovedad2 = DtoNove.CANTIDAD;
                nom.CodeNovedadId = 16;
                nom.NominaProgramationId = 0;
                nom.TerceroId = terc.TerceroId;
                nom.Observation = DtoNove.OBSERVACIONES;
                nom.TypeNovedadName = "DEDUCCION";
                nom.StateNovedad = 2; nom.ConsecutiveMobileId = DtoNove.NOVEDAD_ID;
                _dbcontext.NominaNovedads.Add(nom);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();

                DTONovedadesProduction dto = new DTONovedadesProduction() { responsetransaction = "OK" };
                return dto;
                //if (DtoNove.NOVEDAD_ID != null)
                //{
                //    DTONovedadesProduction dto = new DTONovedadesProduction() { responsetransaction = "OK" };
                //    return dto;
                //}
                //else
                //{
                //    DTONovedadesProduction dto = new DTONovedadesProduction() { responsetransaction = "FALLO" };
                //    return dto;
                //}
            }
            catch (Exception ex)
            {
                DTONovedadesProduction dto = new DTONovedadesProduction() { responsetransaction = "FALLO" };
                return dto;
            }
        }

        public void DeletePhotos(Production production)
        {
            string pathapp = Directory.GetCurrentDirectory();
            string directory = "/wwwroot/FilesApp/ImgMedicion/" + production.ProductionId;
            if (Directory.Exists(pathapp + directory))
            {
                Directory.Delete(pathapp + directory, true);
            }
        }
        public void CreateOrUpdatePhoto(Production production)
        {
            string pathapp = Directory.GetCurrentDirectory();
            string directory = "/wwwroot/FilesApp/ImgMedicion/" + production.ProductionId;
            if (!Directory.Exists(pathapp + directory))
            {
                Directory.CreateDirectory(pathapp + directory);
            }
            if (!string.IsNullOrEmpty(production.Photo1Base64))
            {//tener en cuenta q -1 es el numero de la foto
                production.Photo1Name = directory + "/" + "H" + production.ProductionId + "-1" + ".jpg";
                string baseimage = production.Photo1Base64.Replace("\n", "");
                FileStream stream = System.IO.File.Create(pathapp + production.Photo1Name);
                byte[] imagebytes = Convert.FromBase64String(baseimage);
                stream.Write(imagebytes, 0, imagebytes.Length);
                stream.Close();
            }

            foreach (DetailProduction det in production.DetailProductions)
            {
                if (!string.IsNullOrEmpty(det.Photo1Base64))
                {//tener en cuenta q -1 es el numero de la foto
                    det.Photo1Name = directory + "/" + "H" + det.ProductionId + "-D" + det.DetailProductionId + "-1" + ".jpg";
                    string baseimage = det.Photo1Base64.Replace("\n", "");
                    FileStream stream = System.IO.File.Create(pathapp + det.Photo1Name);
                    byte[] imagebytes = Convert.FromBase64String(baseimage);
                    stream.Write(imagebytes, 0, imagebytes.Length);
                    stream.Close();
                }
            }

        }




        [HttpPost("GetDataReportRendimiento")]
        public DTOWrapperReport GetDataReportRendimiento(string guidfilter)
        {
            DTOFiltersCompras filterdto = filtersreports.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();
            if (filterdto != null)
            {
                if (filterdto.TypeReportId == 1)
                {
                    DTOWrapperReport dtowraper = new DTOWrapperReport();
                    dtowraper.Rendimientos = filterdto.Rendimientos;
                    List<Tercero> terceros = filterdto.Rendimientos.Select(x => x.Tercero).Distinct().ToList();
                    foreach (Tercero ter in terceros)
                    {
                        if (ter.ListNovedades.Count > 0)
                        {
                            ter.ListNovedades.ForEach(x => x.TerceroFullName = ter.FullNameCode);
                            dtowraper.NominaNovedades.AddRange(ter.ListNovedades);
                        }
                    }

                    filtersreports.Remove(filterdto);
                    return dtowraper;
                }
                else if (filterdto.TypeReportId == 2)
                {
                    DTOWrapperReport dtowraper = new DTOWrapperReport();
                    dtowraper.Group1Active = filterdto.Group1Active;
                    dtowraper.Group2Active = filterdto.Group2Active;

                    if (filterdto.Group1Active)//Grupo actividad
                    {
                        filterdto.Rendimientos.ForEach(x => {
                            x.GroupTypeActivityId = x.TypeActivityId;
                        });
                    }
                    else
                    {
                        filterdto.Rendimientos.ForEach(x => {
                            x.GroupTypeActivityId = 0;
                        });
                    }
                    if (filterdto.Group2Active)//Grupo Tercero
                    {
                        filterdto.Rendimientos.ForEach(x => {
                            x.GroupTerceroId = x.TerceroId;
                        });
                    }
                    else
                    {
                        filterdto.Rendimientos.ForEach(x => {
                            x.GroupTerceroId = 0;
                        });
                    }

                    dtowraper.Rendimientos = filterdto.Rendimientos;
                    filtersreports.Remove(filterdto);
                    return dtowraper;
                }
            }
            return new DTOWrapperReport();
        }

        public Production SelectProductionById(Production prodc)
        {
            if (prodc.TransOption == 1)
            {
                Production pro = _dbcontext.Productions.Include(x => x.DetailProductions)
                    .Include(x => x.DetailTerceros).Where(x => x.ProductionId == prodc.ProductionId).FirstOrDefault();
                pro.TypeActivity = _dbcontext.TypeActivitys.Where(x => x.TypeActivityId == pro.TypeActivityId).First();
                pro.CategoryMedicionId = pro.TypeActivity.CategoryId;
                foreach (DetailProductionTercero detTerc in pro.DetailTerceros)
                {
                    detTerc.Tercero = _dbcontext.Terceros.Where(x => x.TerceroId == detTerc.TerceroId).First();
                }
                foreach (DetailProduction detpro in pro.DetailProductions)
                {
                    detpro.Item = _dbcontext.Items.Where(x => x.ItemId == detpro.ItemId).First();
                }
                return pro;
            }
            else if (prodc.TransOption == 2)
            {//producccion mas antigua
                try
                {
                    DateTime dateproduct = _dbcontext.Productions.Min(x => x.DateProduction);
                    return new Production() { DateProduction = dateproduct };
                }
                catch
                {
                    return new Production();
                }
            }
            else
            {
                return new Production();
            }
        }

        public Production Update(Production dat)
        {
            if (dat.TransOption == 1)
            {
                List<DetailProductionTercero> detTerce = dat.DetailTerceros;
                List<DetailProduction> detProduct = dat.DetailProductions;
                dat.DetailTerceros = null; dat.DetailProductions = null;
                // dat.Consecutive = _dbcontext.Productions.Max(x => x.Consecutive) + 1;
                _dbcontext.Entry<Production>(dat).State = EntityState.Modified;
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();

                //eliminar
                if (dat.DetailProductionsDetele.Count > 0 || dat.DetailTercerosDetele.Count > 0)
                {
                    foreach (DetailProduction detDelete in dat.DetailProductionsDetele)
                    {
                        _dbcontext.DetailProductions.Remove(detDelete);
                    }
                    foreach (DetailProductionTercero detTercer in dat.DetailTercerosDetele)
                    {
                        _dbcontext.DetailProductionTerceros.Remove(detTercer);
                    }
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                }

                foreach (DetailProduction det in detProduct)
                {
                    if (det.DetailProductionId == 0)
                    {
                        det.ProductionId = dat.ProductionId;
                        _dbcontext.DetailProductions.Add(det);
                    }
                    else
                    {
                        _dbcontext.Entry<DetailProduction>(det).State = EntityState.Modified;
                    }
                }
                foreach (DetailProductionTercero det in detTerce)
                {
                    if (det.DetailProductionTerceroId == 0)
                    {
                        det.ProductionId = dat.ProductionId;
                        _dbcontext.DetailProductionTerceros.Add(det);
                    }
                    else
                    {
                        _dbcontext.Entry<DetailProductionTercero>(det).State = EntityState.Modified;
                    }
                }
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();

                dat.DetailProductions = detProduct;
                dat.DetailTerceros = detTerce;
                dat.DetailProductionsDetele = new List<DetailProduction>();
                dat.DetailTercerosDetele = new List<DetailProductionTercero>();
            }
            else
            {

            }
            return dat;
        }

        public Production Create(Production dat)
        {
            try
            {
                if (dat.TransOption == 1)
                {
                    List<DetailProductionTercero> detTerce = dat.DetailTerceros;
                    List<DetailProduction> detProduct = dat.DetailProductions;
                    dat.DetailTerceros = null; dat.DetailProductions = null;
                    // dat.Consecutive = _dbcontext.Productions.Max(x => x.Consecutive) + 1;
                    _dbcontext.Productions.Add(dat);
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();

                    //eliminar
                    if (dat.DetailProductionsDetele.Count > 0 || dat.DetailTercerosDetele.Count > 0)
                    {
                        foreach (DetailProduction detDelete in dat.DetailProductionsDetele)
                        {
                            _dbcontext.DetailProductions.Remove(detDelete);
                        }
                        foreach (DetailProductionTercero detTercer in dat.DetailTercerosDetele)
                        {
                            _dbcontext.DetailProductionTerceros.Remove(detTercer);
                        }
                        _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                    }

                    foreach (DetailProduction det in detProduct)
                    {
                        if (det.DetailProductionId == 0)
                        {
                            det.ProductionId = dat.ProductionId;
                            _dbcontext.DetailProductions.Add(det);
                        }
                        else
                        {
                            _dbcontext.Entry<DetailProduction>(det).State = EntityState.Modified;
                        }
                    }
                    foreach (DetailProductionTercero det in detTerce)
                    {
                        if (det.DetailProductionTerceroId == 0)
                        {
                            det.ProductionId = dat.ProductionId;
                            _dbcontext.DetailProductionTerceros.Add(det);
                        }
                        else
                        {
                            _dbcontext.Entry<DetailProductionTercero>(det).State = EntityState.Modified;
                        }
                    }
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                    dat.DetailProductions = detProduct;
                    dat.DetailTerceros = detTerce;
                    dat.DetailProductionsDetele = new List<DetailProduction>();
                    dat.DetailTercerosDetele = new List<DetailProductionTercero>();
                }
                else
                {

                }
                return dat;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Production> selectAll(Production prodc)
        {
            if (prodc.TransOption == 1 || prodc.TransOption == 0)
            {
                return _dbcontext.Productions.ToList();
            }
            else if (prodc.TransOption == 2)
            {
                List<TypeActivity> TypeActivitys = _dbcontext.TypeActivitys.ToList();
                List<Production> listprod = _dbcontext.Productions.Include(x => x.DetailTerceros).Where(x => x.DateProduction.Date == prodc.DateProduction.Date).ToList();
                foreach (Production pro in listprod)
                {
                    pro.TypeActivity = TypeActivitys.Where(x => x.TypeActivityId == pro.TypeActivityId).First();
                    foreach (DetailProductionTercero detTerc in pro.DetailTerceros)
                    {
                        detTerc.Tercero = _dbcontext.Terceros.Where(x => x.TerceroId == detTerc.TerceroId).First();
                    }
                }
                return listprod;
            }
            else
            {
                return _dbcontext.Productions.ToList();
            }

        }

        public List<Rendimiento> GenerarRendimiento(Rendimiento rend)
        {
            if (rend.TransOption == 0 || rend.TransOption == 1)
            {
                RendimientoBussines rendimiBS = new RendimientoBussines(_dbcontext, _configuration);
                return rendimiBS.GenerateRendimiento(rend);
            }
            else if (rend.TransOption == 2)
            {//RENDIMIENTO POR ITEM
                RendimientoBussines rendimiBS = new RendimientoBussines(_dbcontext, _configuration);
                return rendimiBS.GenerateRendimientoByItem(rend);
            }
            else
            {
                RendimientoBussines rendimiBS = new RendimientoBussines(_dbcontext, _configuration);
                return rendimiBS.GenerateRendimiento(rend);
            }
        }


        [HttpGet("selectAllTypeActivitys")]
        public List<TypeActivity> selectAllTypeActivitys()
        {
            List<Category> listcategor = _dbcontext.Categorys.Where(x => x.TypeCategoryId == 2).ToList();
            List<TypeActivity> returns = _dbcontext.TypeActivitys.Where(x => x.IsActive).ToList();
            foreach (TypeActivity typ in returns)
            {
                typ.Category = listcategor.Where(x => x.CategoryId == typ.CategoryId).First();
                typ.CategoryName = typ.Category.Name;
            }
            return returns;
        }

        public TypeActivity Create(TypeActivity TypeActivity)
        {
            TypeActivity find = _dbcontext.TypeActivitys.Where(x => x.Name == TypeActivity.Name).FirstOrDefault();
            if (find == null)
            {
                _dbcontext.TypeActivitys.Add(TypeActivity);
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();
            }
            else
            {

            }
            return TypeActivity;
        }

        public TypeActivity Update(TypeActivity TypeActivity)
        {
            TypeActivity find = _dbcontext.TypeActivitys.Where(x => x.TypeActivityId == TypeActivity.TypeActivityId).FirstOrDefault();
            if (find != null)
            {
                find.Name = TypeActivity.Name;
                _dbcontext.Entry<TypeActivity>(TypeActivity).State = EntityState.Modified;
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();
            }
            else
            {

            }
            return TypeActivity;
        }

        public TypeActivity SelectById(TypeActivity TypeActivity)
        {
            TypeActivity find = _dbcontext.TypeActivitys.Where(x => x.TypeActivityId == TypeActivity.TypeActivityId).FirstOrDefault();

            return find;
        }
        public List<TypeActivity> selectAll(TypeActivity TypeActivity)
        {
            List<Category> listcategor = _dbcontext.Categorys.Where(x => x.TypeCategoryId == 2).ToList();
            List<TypeActivity> returns = _dbcontext.TypeActivitys.ToList();
            foreach (TypeActivity typ in returns)
            {
                typ.Category = listcategor.Where(x => x.CategoryId == typ.CategoryId).First();
                typ.CategoryName = typ.Category.Name;
            }
            return returns;
        }
    }
}
