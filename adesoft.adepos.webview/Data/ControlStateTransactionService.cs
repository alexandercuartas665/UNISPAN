using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class ControlStateTransactionService
    {
        private readonly IConfiguration _configuration;
        private readonly ControlStateTransactionController _controlStateTransactionController;

        public ControlStateTransactionService(IConfiguration configuration, ControlStateTransactionController controlStateTransactionController)
        {
            _configuration = configuration;
            _controlStateTransactionController = controlStateTransactionController;
        }

        public async Task<List<ControlStateTransaction>> selectAll(ControlStateTransaction controlStateTransaction)
        {
            return await Task.FromResult(_controlStateTransactionController.selectAll(controlStateTransaction));
        }

        public async Task<ControlStateTransaction> Create(ControlStateTransaction model)
        {
            return await Task.FromResult(_controlStateTransactionController.Create(model));
        }

        public async Task<ControlStateTransaction> Update(ControlStateTransaction model)
        {
            return await Task.FromResult(_controlStateTransactionController.Update(model));
        }
        public async Task<ControlStateTransaction> SelectById(ControlStateTransaction model)
        {
            return await Task.FromResult(_controlStateTransactionController.SelectById(model));
        }
    }
}
