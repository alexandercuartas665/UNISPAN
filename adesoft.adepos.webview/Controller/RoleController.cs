using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

using adesoft.adepos.Extensions;
namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        ConnectionDB connectionDB;
        public RoleController(IConfiguration configuration,  IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            this.connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            _dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public RoleApp Create(RoleApp roleapp)
        {
            RoleApp find = _dbcontext.RoleApps.Where(x => x.Name == roleapp.Name && x.CompanyId == connectionDB.SedeId).FirstOrDefault();
            if (find == null)
            {
                roleapp.CompanyId = connectionDB.SedeId;
                _dbcontext.RoleApps.Add(roleapp);
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();
            }
            else
            {

            }
            return roleapp;
        } 


        public RoleApp Update(RoleApp roleapp)
        {
            RoleApp find = _dbcontext.RoleApps.Where(x => x.RoleAppId == roleapp.RoleAppId).FirstOrDefault();
            if (find != null)
            {
                find.Name = roleapp.Name;
                _dbcontext.Entry<RoleApp>(roleapp).State = EntityState.Modified;
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();
            }
            else
            {

            }
            return roleapp;
        }

        public RoleApp SelectById(RoleApp roleapp)
        {
            RoleApp find = _dbcontext.RoleApps.Where(x => x.RoleAppId == roleapp.RoleAppId).FirstOrDefault();

            return find;
        }
        public List<RoleApp> selectAll(RoleApp roleapp)
        {
            return _dbcontext.RoleApps.Where(x=> x.CompanyId == connectionDB.SedeId).ToList();
        }
    }
}