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
using Microsoft.AspNetCore.Identity;
using adesoft.adepos.Extensions;
namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAppController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        ConnectionDB connectionDB;
        public UserAppController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            this.connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (connectionDB == null)
                connectionDB = SecurityService.GetConnectionDefault();
            _dbcontext = new AdeposDBContext(connectionDB.Connection);

        }


        public UserApp Create(UserApp userapp)
        {
            UserApp find = _dbcontext.UserApps.Where(x => x.Username == userapp.Username && x.CompanyId == connectionDB.SedeId).FirstOrDefault();
            if (find == null)
            {
                userapp.CompanyId = connectionDB.SedeId;
                var passwordHasher = new PasswordHasher<string>();
                userapp.Password = passwordHasher.HashPassword(null, userapp.PassworNotCry);
                _dbcontext.UserApps.Add(userapp);
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();
            }
            else
            {

            }
            return userapp;
        }


        public UserApp Update(UserApp userapp)
        {
            UserApp find = _dbcontext.UserApps.Where(x => x.UserAppId == userapp.UserAppId).FirstOrDefault();
            if (find != null)
            {
                var passwordHasher = new PasswordHasher<string>();
                userapp.Password = passwordHasher.HashPassword(null, userapp.PassworNotCry);
                userapp.Username = userapp.Username;
                _dbcontext.Entry<UserApp>(userapp).State = EntityState.Modified;
                _dbcontext.SaveChanges(); _dbcontext.DetachAll();
            }
            else
            {

            }
            return userapp;
        }
        [HttpGet("SelectById")]
        public UserApp SelectById(UserApp userapp)
        {
            UserApp find = _dbcontext.UserApps.Where(x => x.UserAppId == userapp.UserAppId).FirstOrDefault();

            return find;
        }

        public List<UserApp> selectAll(UserApp userapp)
        {
            return _dbcontext.UserApps.Where(x => x.CompanyId == connectionDB.SedeId).Include(x => x.RoleApp).ToList();
        }

        [HttpGet("SelectUsersMobile")]
        public List<UserApp> SelectUsersMobile()
        {
            List<RoleApp> listrole = _dbcontext.RoleApps.Where(x => x.Permissions.Where(x => x.ActionAppId == 146).Count() > 0).ToList();
            List<long> listrolepermisi = listrole.Select(x => x.RoleAppId).ToList();
            List<UserApp> users = _dbcontext.UserApps.Where(x => listrolepermisi.Contains(x.RoleAppId)).Include(x => x.RoleApp).ToList();
            return users;
        }
    }
}