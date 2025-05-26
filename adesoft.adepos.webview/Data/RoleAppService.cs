using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class RoleAppService
    {
        private readonly IConfiguration _configuration;
        private readonly RoleController _roleController;

        public RoleAppService(IConfiguration configuration, RoleController roleController)
        {
            _configuration = configuration;
            _roleController = roleController;
        }

        public async Task<List<RoleApp>> selectAll(RoleApp roleapp)
        {
            return await Task.FromResult(_roleController.selectAll(roleapp));
        }

        public async Task<RoleApp> Create(RoleApp model)
        {
            return await Task.FromResult(_roleController.Create(model));
        }

        public async Task<RoleApp> Update(RoleApp model)
        {
            return await Task.FromResult(_roleController.Update(model));
        }
        public async Task<RoleApp> SelectById(RoleApp model)
        {
            return await Task.FromResult(_roleController.SelectById(model));
        }
    }
}
