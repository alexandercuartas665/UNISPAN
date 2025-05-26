using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using adesoft.adepos.Extensions;
namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationGenericController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;

        public LocationGenericController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public LocationGeneric Create(LocationGeneric locationGeneric)
        {
            LocationGeneric find = _dbcontext.LocationGenerics.Where(x => x.Description == locationGeneric.Description).FirstOrDefault();
            if (find == null)
            {
                _dbcontext.LocationGenerics.Add(locationGeneric);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return locationGeneric;
        }


        public LocationGeneric Update(LocationGeneric locationGeneric)
        {
            LocationGeneric find = _dbcontext.LocationGenerics.Where(x => x.LocationGenericId == locationGeneric.LocationGenericId).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<LocationGeneric>(locationGeneric).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return locationGeneric;
        }

        public LocationGeneric SelectById(LocationGeneric locationGeneric)
        {
            LocationGeneric find = _dbcontext.LocationGenerics.Where(x => x.LocationGenericId == locationGeneric.LocationGenericId).FirstOrDefault();

            return find;
        }
        public List<LocationGeneric> selectAll(LocationGeneric LocationGeneric)
        {
            if (LocationGeneric.TransOption == 1 || LocationGeneric.TransOption == 0)
            {
                return _dbcontext.LocationGenerics.ToList();
            }
            else if (LocationGeneric.TransOption == 2)
            {
                return _dbcontext.LocationGenerics.Where(x=>x.TypeLocation== "CARGO").ToList();
            }
            else
            {
                return _dbcontext.LocationGenerics.ToList();
            }
        }

        public List<LocationGeneric> GetLocations(string locationTypeId)
        {
            return _dbcontext.LocationGenerics.Where(x => x.TypeLocation == locationTypeId).ToList();            
        }


    }
}