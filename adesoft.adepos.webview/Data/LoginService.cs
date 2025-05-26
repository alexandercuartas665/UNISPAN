using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
namespace adesoft.adepos.webview.Data
{
    public class LoginService
    {

        //public TookenResult Login(UserApp credentials, string connectionstring)
        //{
        //    AdeposDBContext _dbcontext = new AdeposDBContext(connectionstring);
        //    UserApp userapp = _dbcontext.UserApps.Where(x => x.Username == credentials.Username).FirstOrDefault();
        //    if (userapp != null)
        //    {
        //        var expiry = DateTime.Now.AddMinutes(20);

        //        return ValidateCredentials(credentials, userapp) ? new TookenResult { Tooken = GenerateJWT(credentials.Username, expiry, userapp.RoleAppId.ToString()), Expiry = expiry } : new TookenResult();
        //    }
        //    else
        //    {
        //        return new TookenResult();
        //    }
        //}
    }
}
