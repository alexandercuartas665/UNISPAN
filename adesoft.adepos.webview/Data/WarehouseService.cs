using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class WarehouseService
    {
        private readonly IConfiguration _configuration;
        private readonly WarehouseController _warehouseController;

        public WarehouseService(IConfiguration configuration, WarehouseController warehouseController)
        {
            _configuration = configuration;
            _warehouseController = warehouseController;
        }

        public async Task<List<Warehouse>> selectAll(Warehouse warehouse)
        {
            return await Task.FromResult(_warehouseController.selectAll(warehouse));
        }

        public async Task<Warehouse> Create(Warehouse model)
        {
            return await Task.FromResult(_warehouseController.Create(model));
        }

        public async Task<Warehouse> Update(Warehouse model)
        {
            return await Task.FromResult(_warehouseController.Update(model));
        }
        public async Task<Warehouse> SelectById(Warehouse model)
        {
            return await Task.FromResult(_warehouseController.SelectById(model));
        }


        public async Task SincronizarBodegas()
        {
            await _warehouseController.SincronizarBodegas();
        }

    }
}
