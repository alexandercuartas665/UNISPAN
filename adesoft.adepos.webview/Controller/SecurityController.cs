using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using adesoft.adepos.Extensions;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.DTO.Simex;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.PL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    //  [Authorize]
    public class SecurityController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private AdeposDBContext _dbcontext;
        private readonly TokenAuthenticationStateProvider _tookenState;
        private readonly IHttpContextAccessor httpContextAccessor;
        ConnectionDB connectionDB;
        public SecurityController(IConfiguration configuration, TokenAuthenticationStateProvider tookenState, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            _tookenState = tookenState;
            //this.connectionProvider = connectionProvider;
            this.httpContextAccessor.HttpContext.Session.SetString("Inicio", "adesoft");

            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (connectionDB == null)
                connectionDB = SecurityService.GetConnectionDefault();
            this.connectionDB = connectionDB;
            if (connectionDB != null && connectionDB.Connection != null)
                _dbcontext = new AdeposDBContext(connectionDB.Connection);
        }


        //public async Task InvokeAsync(HttpContext httpContext)
        //{
        //    try
        //    {
        //        await _next(httpContext); ht
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        [HttpGet("get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("prueba2")]
        public IEnumerable<string> prueba2()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost("login")]

        public TookenResult Login(UserApp credentials, string connectionstring)
        {
            _dbcontext = new AdeposDBContext(connectionstring);
            UserApp userapp = _dbcontext.UserApps.Where(x => x.Username == credentials.Username && x.CompanyId == credentials.CompanyId).FirstOrDefault();
            if (userapp != null)
            {
                var expiry = DateTime.Now.AddMinutes(50000);

                return ValidateCredentials(credentials, userapp) ? new TookenResult { Tooken = GenerateJWT(credentials.Username, expiry, userapp.RoleAppId.ToString()), Expiry = expiry } : new TookenResult();
            }
            else
            {
                return new TookenResult();
            }
        }

        [HttpPost("LoginUserApp")]
        public IActionResult LoginUserApp(DTOUserLogin userLogin)
        {
            try
            {
                var userApp = _dbcontext.UserApps                     
                    .Where(u => u.Username == userLogin.UserName)                    
                    .FirstOrDefault();
                if (userApp != null)
                {                    

                    List<ActionApp> actionApps = new List<ActionApp>();
                    //var roleApp = _dbcontext.RoleApps.Where(r => r.RoleAppId.Equals(userApp.RoleAppId)).FirstOrDefault();
                    var actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("LogisticsCaptureApp")).FirstOrDefault();
                    var permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == userApp.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("CreatePalletsApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == userApp.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("DeletePalletsApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == userApp.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("SendPalletPickingApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == userApp.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("CompletePalletsApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == userApp.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("ViewOrdersPendingPickingApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == userApp.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("ViewOrdersPendingTicketsApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == userApp.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    if (actionApps.Count != 0)
                    {
                        var expiry = DateTime.Now.AddMinutes(50000);
                        var tookenResult = ValidateCredentials(userLogin, userApp) ? new TookenResult { Tooken = GenerateJWT(userLogin.UserName, expiry, userApp.RoleAppId.ToString()), Expiry = expiry, ActionApps = actionApps, ZoneProductId = userApp.ZoneProductId } : new TookenResult();

                        return Ok(tookenResult);
                    }
                    else
                        throw new Exception("El usuario ingresado no tiene permiso.");                    
                }
                else
                    return NotFound("El usuario ingresado no ha sido encontrado");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUsersApp")]
        public IActionResult GetUsersApp()
        {
            try
            {
                var userApps = new List<DTOUserApp>();
                var users = _dbcontext.UserApps                    
                    .ToList();
                foreach (var user in users)
                {                    
                    var actionApps = new List<ActionApp>();
                    var actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("LogisticsCaptureApp")).FirstOrDefault();
                    var permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == user.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();                   
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("CreatePalletsApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == user.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("DeletePalletsApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == user.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("SendPalletPickingApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == user.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("CompletePalletsApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == user.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);                    

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("ViewOrdersPendingPickingApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == user.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    actionApp = _dbcontext.ActionApps.Where(a => a.NameAction.Equals("ViewOrdersPendingTicketsApp")).FirstOrDefault();
                    permissions = _dbcontext.Permissions.Where(p => p.RoleAppId == user.RoleAppId && p.ActionAppId.Equals(actionApp.ActionAppId)).FirstOrDefault();
                    if (!(permissions is null))
                        actionApps.Add(actionApp);

                    if (actionApps.Count != 0)
                    {
                        userApps.Add(new DTOUserApp()
                        {
                            Username = user.Username,
                            Password = user.Password,
                            PassworNotCry = user.PassworNotCry,
                            ActionApps = actionApps,
                            ZoneProductId = user.ZoneProductId
                        });
                    }
                }
                
                return Ok(userApps);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SetSessionByID")]
        public void SetSessionByID(string CuentaN)
        {
            List<ConnectionDB> connects = GetConnections();
            ConnectionDB connectionDB = connects.Where(x => x.CuentaN == CuentaN).First();
            //  httpContextAccessor.HttpContext.Session.SetString("h", "f");
            httpContextAccessor.HttpContext.Session.Set<ConnectionDB>("ConnectionDB", connectionDB);
            _dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        [HttpPost("SetSessionConnection")]
        public void SetSessionConnection(ConnectionDB ConnectionDB)
        {
            //  httpContextAccessor.HttpContext.Session.SetString("h", "f");
            httpContextAccessor.HttpContext.Session.Set<ConnectionDB>("ConnectionDB", ConnectionDB);
            this.connectionDB = ConnectionDB;
            _dbcontext = new AdeposDBContext(ConnectionDB.Connection);
        }

        [HttpGet("GetConnections")]
        public static List<ConnectionDB> GetConnections()
        {
            string dir = Directory.GetCurrentDirectory();
            string json = System.IO.File.ReadAllText("ConnectionsDB.json");
            List<ConnectionDB> d = JsonConvert.DeserializeObject<List<ConnectionDB>>(json);
            return d;
        }


        [HttpPost("GetUserAppByTooken")]

        public UserApp GetUserAppByTooken(dynamic tooken)
        {
            AuthenticationState state = _tookenState.GetAuthenticationState(tooken);
            string userid = state.User.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault();
            UserApp userapp = _dbcontext.UserApps.Where(x => x.Username == userid).FirstOrDefault();
            return userapp;
        }


        [HttpPost("getactions")]

        public List<ActionApp> GetActionsPermission(dynamic tooken)
        {
            AuthenticationState state = _tookenState.GetAuthenticationState(tooken);
            string roleid = state.User.Claims.Where(x => x.Type == "RoleId").Select(x => x.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(roleid))
            {
                return new List<ActionApp>();
            }
            else
            {
                // return _dbcontext.ActionApps.ToList();
                return GetActionsPermissionByRole(long.Parse(roleid));
            }

        }

        public ActionApp ValidatePermissionByTooken(dynamic tooken, string actionFather, string ActionChild)
        {
            ActionApp resp = new ActionApp();

            AuthenticationState state = _tookenState.GetAuthenticationState(tooken);
            string roleid = state.User.Claims.Where(x => x.Type == "RoleId").Select(x => x.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(roleid))
            {
            }
            ActionApp father = _dbcontext.ActionApps.Where(x => x.IsActive && x.NameAction == actionFather).FirstOrDefault();
            if (father != null)
            {
                ActionApp child = _dbcontext.ActionApps.Where(x => x.IsActive && x.IdFather == father.ActionAppId
                && x.NameAction == ActionChild).FirstOrDefault();
                if (child != null)
                {
                    Permission perm = _dbcontext.Permissions.Where(x => x.RoleAppId == long.Parse(roleid) && x.ActionAppId == child.ActionAppId).FirstOrDefault();
                    if (perm != null)
                    {
                        resp.TransactionIsOk = true;
                        resp.MessageResponse = "Ok";
                    }
                    else
                    {
                        resp.TransactionIsOk = false;
                        resp.MessageResponse = "No tiene permisos";
                    }
                }
                else
                {
                    resp.TransactionIsOk = false;
                    resp.MessageResponse = "No tiene permisos";
                }
            }
            else
            {
                resp.TransactionIsOk = false;
                resp.MessageResponse = "No tiene permisos";
            }
            return resp;
        }

        public List<ActionApp> GetActionsPermissionByRole(long roleid)
        {
            //       List<ActionApp> listActions = _dbcontext.ActionApps.Where(x => x.IsActive && x.MenuId == connectionDB.MenuId).ToList();
            List<ActionApp> listActions = _dbcontext.ActionApps.Where(x => x.IsActive).ToList();
            List<Permission> listPermission = _dbcontext.Permissions.Where(x => x.RoleAppId == roleid).ToList();
            _dbcontext.DetachAll();
            //todos a false

            //seteo los permisos del ultimo nivel para apartir de alli desencadenar los demas
            foreach (ActionApp chi2 in listActions.Where(x => x.Type == "Option").OrderBy(x => x.OrderNum).ToList())
            {
                chi2.HavePermission = listPermission.Where(x => x.ActionAppId == chi2.ActionAppId).Count() > 0;
            }
            listActions = ActionApp.UtilSetPermission(ref listActions);
            return listActions;
        }
        [HttpPost]
        public bool SavePermissionOfListActionApp(RoleApp role)
        {

            _dbcontext.Permissions.RemoveRange(_dbcontext.Permissions.Where(x => x.RoleAppId == role.RoleAppId).ToList());
            _dbcontext.Permissions.AddRange(role.Permissions);
            _dbcontext.SaveChanges(); _dbcontext.DetachAll();
            return true;
        }
        public List<RoleApp> GetRoles()
        {
            return _dbcontext.RoleApps.Where(x => x.CompanyId == connectionDB.SedeId).ToList();
        }


        bool ValidateCredentials(UserApp credentials, UserApp userapp)
        {
            // var user = _configuration.GetSection("UserApp").Get<UserApp>();
            var passwordHasher = new PasswordHasher<string>();
            string hashedpass = passwordHasher.HashPassword(null, credentials.Password);
            return passwordHasher.VerifyHashedPassword(null, userapp.Password, credentials.Password) == PasswordVerificationResult.Success;

        }

        bool ValidateCredentials(DTOUserLogin userLogin, UserApp userapp)
        {
            // var user = _configuration.GetSection("UserApp").Get<UserApp>();
            var passwordHasher = new PasswordHasher<string>();
            string hashedpass = passwordHasher.HashPassword(null, userLogin.Password);
            return passwordHasher.VerifyHashedPassword(null, userapp.Password, userLogin.Password) == PasswordVerificationResult.Success;

        }

        //  public string 

        private string GenerateJWT(string username, DateTime expiry, string RoleId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                new[] { new Claim(ClaimTypes.Name, username), new Claim("RoleId", RoleId) },
                expires: expiry,
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

    }
}
