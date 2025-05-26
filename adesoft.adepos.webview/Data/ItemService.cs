using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class ItemService
    {
        private readonly IConfiguration _configuration;
        private readonly ItemController _itemController;
        //IJSRuntime JSRuntime;
        public ItemService(IConfiguration configuration, ItemController itemController)
        {
            _configuration = configuration;
            _itemController = itemController;
            //this.JSRuntime = JSRuntime;
            //var t = Task.Run(SetInstanceContext);
            //t.Wait();
            //Task.Factory.StartNew(() => SetInstanceContext());
            // Task.WaitAll(task1);
            //var t = Task.Run(GetConnectionDB);
            //t.Wait();
            //string con = t.Result.ToString();
        }

        //public async Task SetInstanceContext()
        //{
        //    var value = await JSRuntime.InvokeAsync<object>("localStorage.getItem", "connectionString");
        //    string resul = value.ToString();
        //}
        public async Task<List<Item>> selectAll(Item roleapp)
        {
            //  string r = await ProtectedSessionStore.GetAsync<string>("connectionString");
            //   object obj = await JSRuntime.InvokeAsync<object>("localStorage.getItem", "connectionString");
            return await Task.FromResult(_itemController.selectAll(roleapp));
        }

        public async Task<Item> Create(Item model)
        {
            return await Task.FromResult(_itemController.Create(model));
        }

        public async Task<Item> Update(Item model)
        {
            return await Task.FromResult(_itemController.Update(model));
        }
        public async Task<Item> SelectById(Item model)
        {
            return await Task.FromResult(_itemController.SelectById(model));
        }


        public async Task<List<UnitMeasurement>> SelectAllUnitMeasurement(UnitMeasurement model)
        {
            return await Task.FromResult(_itemController.SelectAllUnitMeasurement(model));
        }

        public async Task<Item> CreateItemsFromFile(string file)
        {
            return await Task.FromResult(_itemController.CreateItemsFromFile(file));
        }

        public async Task<Item> ReadEquivalence85(string file)
        {
            return await Task.FromResult(_itemController.ReadEquivalence85(file));
        }
    }
}
