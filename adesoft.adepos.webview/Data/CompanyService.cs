using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class CompanyService
    {
        private readonly IConfiguration _configuration;
        private readonly CompanyController _companyController;

        public CompanyService(IConfiguration configuration, CompanyController companyController)
        {
            _configuration = configuration;
            _companyController = companyController;
        }

        public async Task<List<Company>> selectAll(Company roleapp)
        {

            return await Task.FromResult(_companyController.selectAll(roleapp));
        }

        public async Task<Company> Create(Company model)
        {
            return await Task.FromResult(_companyController.Create(model));
        }

        public async Task<Company> Update(Company model)
        {
            return await Task.FromResult(_companyController.Update(model));
        }
        public async Task<Company> SelectById(Company model)
        {
            return await Task.FromResult(_companyController.SelectById(model));
        }
        public async Task<Company> SelectCurrent(Company model)
        {
            return await Task.FromResult(_companyController.SelectCurrent(model));
        }


    }
}
