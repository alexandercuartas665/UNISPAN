//using adesoft.adepos.webview.Controller;
//using adesoft.adepos.webview.Data.Model;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace adesoft.adepos.webview.Data
//{
//    public class OrderManufacturingService
//    {
//        private readonly IConfiguration _configuration;
//        private readonly OrderManufacturingController _crderManufacturingController;

//        public OrderManufacturingService(IConfiguration configuration, OrderManufacturingController crderManufacturingController)
//        {
//            _configuration = configuration;
//            _crderManufacturingController = crderManufacturingController;
//        }

//        public async Task<List<OrderManufacturing>> selectAll(OrderManufacturing crderManufacturing)
//        {
//            return await Task.FromResult(_crderManufacturingController.selectAll(crderManufacturing));
//        }

//        public async Task<OrderManufacturing> Create(OrderManufacturing model)
//        {
//            return await Task.FromResult(_crderManufacturingController.Create(model));
//        }

//        public async Task<OrderManufacturing> Update(OrderManufacturing model)
//        {
//            return await Task.FromResult(_crderManufacturingController.Update(model));
//        }

//        public async Task<OrderManufacturing> Delete(OrderManufacturing model)
//        {
//            return await Task.FromResult(_crderManufacturingController.Delete(model));
//        }
//        public async Task<OrderManufacturing> SelectById(OrderManufacturing model)
//        {
//            return await Task.FromResult(_crderManufacturingController.SelectById(model));
//        }
//    }
//}
