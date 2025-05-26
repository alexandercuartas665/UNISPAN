using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class UserAppService
    {
        private readonly IConfiguration _configuration;
        private readonly UserAppController _userAppController;
        public UserAppService(IConfiguration configuration, UserAppController userAppController)
        {
            _configuration = configuration;
            _userAppController = userAppController;
        }

        public async Task<UserApp> Create(UserApp userapp)
        {
            return await Task.FromResult(_userAppController.Create(userapp));
        }

        public async Task<UserApp> Update(UserApp userapp)
        {
            return await Task.FromResult(_userAppController.Update(userapp));
        }


        public async Task<UserApp> SelectById(UserApp userapp)
        {
            return await Task.FromResult(_userAppController.SelectById(userapp));
        }
        public async Task<List<UserApp>> selectAll(UserApp userapp)
        {
            return await Task.FromResult(_userAppController.selectAll(userapp));
        }
    }
}
