using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class AlertXOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly AlertXOrderController _alertXOrderController;

        public AlertXOrderService(IConfiguration configuration, AlertXOrderController alertXOrderController)
        {
            _configuration = configuration;
            _alertXOrderController = alertXOrderController;
        }

        public async Task<List<AlertXOrder>> selectAll(AlertXOrder alertXOrder)
        {
            return await Task.FromResult(_alertXOrderController.selectAll(alertXOrder));
        }

        public async Task<AlertXOrder> Create(AlertXOrder model)
        {
            return await Task.FromResult(_alertXOrderController.Create(model));
        }

        public async Task<AlertXOrder> Update(AlertXOrder model)
        {
            return await Task.FromResult(_alertXOrderController.Update(model));
        }
        public async Task<AlertXOrder> SelectById(AlertXOrder model)
        {
            return await Task.FromResult(_alertXOrderController.SelectById(model));
        }
    }
}
