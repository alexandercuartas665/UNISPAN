using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class StateTransactionGenericService
    {
        private readonly IConfiguration _configuration;
        private readonly StateTransactionGenericController _stateTransactionGenericController;

        public StateTransactionGenericService(IConfiguration configuration, StateTransactionGenericController stateTransactionGenericController)
        {
            _configuration = configuration;
            _stateTransactionGenericController = stateTransactionGenericController;
        }

        public async Task<List<StateTransactionGeneric>> selectAll(StateTransactionGeneric stateTransactionGeneric)
        {
            return await Task.FromResult(_stateTransactionGenericController.selectAll(stateTransactionGeneric));
        }

        public async Task<StateTransactionGeneric> Create(StateTransactionGeneric model)
        {
            return await Task.FromResult(_stateTransactionGenericController.Create(model));
        }

        public async Task<StateTransactionGeneric> Update(StateTransactionGeneric model)
        {
            return await Task.FromResult(_stateTransactionGenericController.Update(model));
        }
        public async Task<StateTransactionGeneric> SelectById(StateTransactionGeneric model)
        {
            return await Task.FromResult(_stateTransactionGenericController.SelectById(model));
        }
    }
}
