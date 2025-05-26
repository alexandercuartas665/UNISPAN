using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class ParameterService
    {
        private readonly IConfiguration _configuration;
        private readonly ParameterController _parameterController;

        public ParameterService(IConfiguration configuration, ParameterController parameterController)
        {
            _configuration = configuration;
            _parameterController = parameterController;
        }

        public async Task<List<Parameter>> selectAll(Parameter parameter)
        {
            return await Task.FromResult(_parameterController.selectAll(parameter));
        }

        public async Task<Parameter> Create(Parameter model)
        {
            return await Task.FromResult(_parameterController.Create(model));
        }

        public async Task<Parameter> Update(Parameter model)
        {
            return await Task.FromResult(_parameterController.Update(model));
        }
        public async Task<Parameter> SelectById(Parameter model)
        {
            return await Task.FromResult(_parameterController.SelectById(model));
        }

        public async Task<Parameter> SelectByIdentify(string nameIdentify)
        {
            return await Task.FromResult(_parameterController.SelectByIdentify(nameIdentify));
        }
    }
}
