using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using adesoft.adepos.Extensions;
using System.IO;
using Radzen.Blazor;
using adesoft.adepos.webview.Util;
using Microsoft.AspNetCore.Mvc;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerceroController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private AdeposDBContext _dbcontext;

        private readonly IHttpContextAccessor httpContextAccessor;
        public TerceroController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
          
            if (connectionDB == null)
                connectionDB = SecurityService.GetConnectionDefault();
            _dbcontext = new AdeposDBContext(connectionDB.Connection);

        }

        public void ReadPhotos(Tercero tercero)
        {
            string pathapp = Directory.GetCurrentDirectory();
            if (!string.IsNullOrEmpty(tercero.Photo))
            {
                var base64Img = new Base64Image
                {
                    FileContents = System.IO.File.ReadAllBytes(pathapp + tercero.Photo),
                    ContentType = "image/jpg"
                };
                tercero.PhotoBase64 = base64Img.ToString();
                //byte[] bytes = File.ReadAllBytes(pathapp + tercero.Photo);
                //tercero.PhotoBase64 = Base64UrlEncoder.Encode(bytes);
            }
            if (!string.IsNullOrEmpty(tercero.Photo1))
            {
                var base64Img = new Base64Image
                {
                    FileContents = System.IO.File.ReadAllBytes(pathapp + tercero.Photo1),
                    ContentType = "image/jpg"
                };
                tercero.Photo1Base64 = base64Img.ToString();

                //byte[] bytes = File.ReadAllBytes(pathapp + tercero.Photo1);
                //tercero.Photo1Base64 = Base64UrlEncoder.Encode(bytes);
            }
            if (!string.IsNullOrEmpty(tercero.Photo2))
            {
                var base64Img = new Base64Image
                {
                    FileContents = System.IO.File.ReadAllBytes(pathapp + tercero.Photo2),
                    ContentType = "image/jpg"
                };
                tercero.Photo2Base64 = base64Img.ToString();

                //byte[] bytes = File.ReadAllBytes(pathapp + tercero.Photo2);
                //tercero.Photo2Base64 = Base64UrlEncoder.Encode(bytes);
            }
        }
        public void CreateOrUpdatePhoto(Tercero tercero)
        {
            string pathapp = Directory.GetCurrentDirectory();
            string directory = "/wwwroot/FilesApp/" + tercero.TerceroId;
            if (!Directory.Exists(pathapp+directory))
            {
                Directory.CreateDirectory(pathapp+directory);
            }
            if (!string.IsNullOrEmpty(tercero.PhotoBase64))
            {
                tercero.Photo = directory + "/Foto.jpg";
                string baseimage = tercero.PhotoBase64.Split(",")[1];
                FileStream stream = System.IO.File.Create(pathapp + tercero.Photo);
                byte[] imagebytes = Convert.FromBase64String(baseimage);
                stream.Write(imagebytes, 0, imagebytes.Length);
                stream.Close();
            }
            if (!string.IsNullOrEmpty(tercero.Photo1Base64))
            {
                tercero.Photo1 = directory + "/Cedula1.jpg";

                FileStream stream = System.IO.File.Create(pathapp + tercero.Photo1);
                string baseimage = tercero.Photo1Base64.Split(",")[1];
                byte[] imagebytes = Convert.FromBase64String(baseimage);
                stream.Write(imagebytes, 0, imagebytes.Length);
                stream.Close();
            }
            if (!string.IsNullOrEmpty(tercero.Photo2Base64))
            {
                tercero.Photo2 = directory + "/Cedula2.jpg";

                FileStream stream = System.IO.File.Create(pathapp + tercero.Photo2);
                string baseimage = tercero.Photo2Base64.Split(",")[1];
                byte[] imagebytes = Convert.FromBase64String(baseimage);
                stream.Write(imagebytes, 0, imagebytes.Length);
                stream.Close();
            }
        }

        public Tercero Create(Tercero tercero)
        {
            Tercero find = _dbcontext.Terceros.Where(x => (x.NumDocument == tercero.NumDocument || x.CodeEnterprise==tercero.CodeEnterprise) && x.TypeTerceroId == tercero.TypeTerceroId).FirstOrDefault();

            if (find == null)
            {
                if (tercero.TransOption == 0 || tercero.TransOption == 1)
                {
                    _dbcontext.Terceros.Add(tercero);
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                }
                else if (tercero.TransOption == 2)
                {
                    _dbcontext.Terceros.Add(tercero);
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                    CreateOrUpdatePhoto(tercero);
                    _dbcontext.Entry<Tercero>(tercero).State = EntityState.Modified;
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                }
                tercero.TransactionIsOk = true;
                tercero.MessageResponse = "Guardado correctamente.";
            }
            else
            {
                tercero.TransactionIsOk = false;
                tercero.MessageResponse = "ya existe una persona con el mismo codigo o numero de documento.";
            }
            return tercero;
        }


        public Tercero Update(Tercero tercero)
        {
            Tercero find = _dbcontext.Terceros.Where(x => x.TerceroId == tercero.TerceroId).FirstOrDefault();
            if (find != null)
            {
                if (tercero.TransOption == 0 || tercero.TransOption == 1)
                {
                    Tercero findrepet = _dbcontext.Terceros.Where(x => (x.NumDocument == tercero.NumDocument || x.CodeEnterprise == tercero.CodeEnterprise) && x.TerceroId != tercero.TerceroId).FirstOrDefault();
                    if (findrepet != null)
                    {
                        tercero.TransactionIsOk = false;
                        tercero.MessageResponse = "ya existe una persona con el mismo codigo o numero de documento.";
                        return tercero;
                    }
                    _dbcontext.Entry<Tercero>(tercero).State = EntityState.Modified;
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                }
                else if (tercero.TransOption == 2)
                {
                    CreateOrUpdatePhoto(tercero);
                    _dbcontext.Entry<Tercero>(tercero).State = EntityState.Modified;
                    _dbcontext.SaveChanges(); _dbcontext.DetachAll();
                }
            }
            tercero.TransactionIsOk = true;
            tercero.MessageResponse = "Guardado correctamente.";
            return tercero;
        }

        public Tercero SelectById(Tercero tercero)
        {
            Tercero find;
            if (tercero.TransOption == 0 && tercero.TransOption == 1)
            {
                find = _dbcontext.Terceros.Where(x => x.TerceroId == tercero.TerceroId).FirstOrDefault();
            }
            else if (tercero.TransOption == 2)
            {
                find = _dbcontext.Terceros.Where(x => x.NumDocument == tercero.NumDocument).FirstOrDefault();
            }
            else if (tercero.TransOption == 3)
            {
                find = _dbcontext.Terceros.Where(x => x.TerceroId == tercero.TerceroId).FirstOrDefault();
                ReadPhotos(find);
            }
            else
            {
                find = _dbcontext.Terceros.Where(x => x.TerceroId == tercero.TerceroId).FirstOrDefault();
            }
            return find;
        }

        public Tercero SelectById2(Tercero tercero)
        {
            Tercero find = _dbcontext.Terceros.Where(x => x.NumDocument == tercero.NumDocument ).OrderByDescending(x => x.DateContractStart).FirstOrDefault();            
            return find;
        }



        [HttpPost("selectAllTerceros")]
        public List<Tercero> selectAll(Tercero tercero)
        {
            if (tercero.TransOption == 2)//seleccionar cliente y generico
            {
                return _dbcontext.Terceros.Include(x => x.TypeTercero).Where(x => (x.TypeTerceroId == 4 || x.TypeTerceroId == 1)).ToList();
            }
            else if (tercero.TransOption == 3)//seleccionar empleado y generico
            {
                return _dbcontext.Terceros.Include(x => x.TypeTercero).Where(x => (x.TypeTerceroId == 3 || x.TypeTerceroId == 4)).ToList();
            }
            else if (tercero.TransOption == 4)//seleccionar proveedor y generico
            {
                return _dbcontext.Terceros.Include(x => x.TypeTercero).Where(x => (x.TypeTerceroId == 2 || x.TypeTerceroId == 4)).ToList();
            }
            else if (tercero.TransOption == 5)//seleccionar Operario de Producción 
            {
                return _dbcontext.Terceros.Include(x => x.TypeTercero).Where(x => (x.TypeTerceroId == 5)).ToList();
            }
            else
            {
                return _dbcontext.Terceros.Include(x => x.TypeTercero).Where(x => x.TypeTerceroId != 4).ToList();
            }

        }

        public List<Tercero> GetEmpleados()
        {            
            return _dbcontext.Terceros.Include(x => x.TypeTercero).Where(x => (x.TypeTerceroId == 3 || x.TypeTerceroId == 4  || x.TypeTerceroId == 5)).ToList();            
        }

        public List<TypeTercero> selectAllTypeTercero(TypeTercero typetercero)
        {
            if (typetercero.TransOption == 0)
            {
                return _dbcontext.TypeTerceros.ToList();
            }
            else if (typetercero.TransOption == 2)
            {
                return _dbcontext.TypeTerceros.Where(x => x.TypeTerceroId != 4).ToList();
            }
            else
            {
                return _dbcontext.TypeTerceros.ToList();
            }
        }

    }
}
