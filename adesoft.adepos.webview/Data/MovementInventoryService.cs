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
    public class MovementInventoryService
    {
        private readonly IConfiguration _configuration;
        private readonly MovementInventoryController _movementInventoryController;

        public MovementInventoryService(IConfiguration configuration, MovementInventoryController movementInventoryController)
        {
            _configuration = configuration;
            _movementInventoryController = movementInventoryController;
        }

        public async Task<List<MovementInventory>> selectAll(MovementInventory movementInventory)
        {
            return await Task.FromResult(_movementInventoryController.selectAll(movementInventory));
        }

        public async Task<MovementInventory> Create(MovementInventory model)
        {
            return await Task.FromResult(_movementInventoryController.Create(model));
        }

        public async Task<MovementInventory> Update(MovementInventory model)
        {
            return await Task.FromResult(_movementInventoryController.Update(model));
        }
        public async Task<MovementInventory> SelectById(MovementInventory model)
        {
            return await Task.FromResult(_movementInventoryController.SelectById(model));
        }

        public async Task<List<DTOInventary>> selectAll(DTOInventary model)
        {
            return await Task.FromResult(_movementInventoryController.selectAll(model));
        }
    }
}
