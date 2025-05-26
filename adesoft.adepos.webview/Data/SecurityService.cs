using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    //[Authorize]
    public class SecurityService
    {
        private readonly IConfiguration _configuration;
        private readonly SecurityController _securityController;
        //public SecurityService(IConfiguration configuration) => _configuration = configuration;
        //public SecurityService(IConfiguration configuration)
        //{

        //    _configuration = configuration;

        //}
        public SecurityService(IConfiguration configuration, SecurityController securityController)
        {
            _configuration = configuration;
            _securityController = securityController;

            // contextAccessor.HttpContext.Session.ge
        }
        //public SecurityService(IConfiguration configuration, AdeposDBContext dbcontext)
        //{
        //    _configuration = configuration;
        //    _dbcontext = dbcontext;
        //}
        //public SecurityService(AdeposDBContext dbcontext)
        //{

        //    _dbcontext = dbcontext;
        //}
        public async Task<UserApp> GetUserAppByTooken(dynamic tooken)
        {
            try
            {
                return await Task.FromResult(_securityController.GetUserAppByTooken(tooken));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TookenResult> GetTooken(string password, string username, ConnectionDB connectionDB)
        {
            try
            {
                return await Task.FromResult(_securityController.Login(new UserApp() { Password = password, Username = username, CompanyId = connectionDB.SedeId }, connectionDB.Connection));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetSessionConnection(ConnectionDB ConnectionDB)
        {
            _securityController.SetSessionConnection(ConnectionDB);
        }

        public async Task<List<ActionApp>> GetActionsPermission(string tooken)
        {
            try
            {
                return await Task.FromResult(_securityController.GetActionsPermission(tooken));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionApp> ValidatePermissionByTooken(dynamic tooken, string actionFather, string ActionChild)
        {
            try
            {
                return await Task.FromResult(_securityController.ValidatePermissionByTooken(tooken, actionFather, ActionChild));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<ActionApp>> GetActionsPermissionByRole(long roleid)
        {
            try
            {
                return await Task.FromResult(_securityController.GetActionsPermissionByRole(roleid));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ConnectionDB> GetConnections()
        {
            try
            {
                return SecurityController.GetConnections();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ConnectionDB GetConnectionDefault()
        {
            try
            {
                return SecurityController.GetConnections().Where(x => x.Name == "UnisPanPro").FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SavePermissionOfListActionApp(RoleApp role)
        {
            try
            {
                return _securityController.SavePermissionOfListActionApp(role);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> SavePermissionOfListActionAppAsync(RoleApp role)
        {
            try
            {
                return await Task.FromResult(_securityController.SavePermissionOfListActionApp(role));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RoleApp>> GetRoles()
        {
            try
            {
                return await Task.FromResult(_securityController.GetRoles());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
