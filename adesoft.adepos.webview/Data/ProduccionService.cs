using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class ProduccionService
    {
        private readonly IConfiguration _configuration;
        private readonly ProduccionController _ProduccionController;

        public ProduccionService(IConfiguration configuration, ProduccionController ProduccionController)
        {
            _configuration = configuration;
            _ProduccionController = ProduccionController;
        }
        public async Task<List<Production>> SelectAll(Production dat)
        {
            return await Task.FromResult(_ProduccionController.selectAll(dat));
        }
        public async Task<Production> Create(Production dat)
        {
            return await Task.FromResult(_ProduccionController.Create(dat));
        }

        public async Task<Production> Update(Production dat)
        {
            return await Task.FromResult(_ProduccionController.Update(dat));
        }
        public async Task<List<TypeActivity>> selectAllTypeActivitys()
        {
            return await Task.FromResult(_ProduccionController.selectAllTypeActivitys());
        }
        public async Task<List<Rendimiento>> GenerarRendimiento(Rendimiento rend)
        {
            return await Task.FromResult(_ProduccionController.GenerarRendimiento(rend));
        }
        public async Task<Production> SelectProductionById(Production prodc)
        {
            return await Task.FromResult(_ProduccionController.SelectProductionById(prodc));
        }
        public async Task<List<TypeActivity>> selectAll(TypeActivity TypeActivity)
        {
            return await Task.FromResult(_ProduccionController.selectAll(TypeActivity));
        }

        public async Task<TypeActivity> Create(TypeActivity model)
        {
            return await Task.FromResult(_ProduccionController.Create(model));
        }

        public async Task<TypeActivity> Update(TypeActivity model)
        {
            return await Task.FromResult(_ProduccionController.Update(model));
        }
        public async Task<TypeActivity> SelectById(TypeActivity model)
        {
            return await Task.FromResult(_ProduccionController.SelectById(model));
        }

     
    }
}
