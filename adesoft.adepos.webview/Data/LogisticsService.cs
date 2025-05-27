using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using adesoft.adepos.webview.Data.DTO.PL;
using adesoft.adepos.webview.Data.Model;

namespace adesoft.adepos.webview.Data
{
    public class LogisticsService
    {
        private readonly LogisticsController _logisticsController;
        private readonly string _wwwrootDirectory; 

        public LogisticsService(LogisticsController logisticsController)
        {
            _logisticsController = logisticsController;
            _wwwrootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        public async Task<string> ImportFile(DTOOrder model)
        {
            return await Task.FromResult(_logisticsController.ImportFile(model));
        }

        public async Task AddOrderReportFilter(DTOOrderReportFilter filter)
        {
            await Task.Run(new Action(() => { _logisticsController.AddOrderReportFilter(filter); }));
        }

        public async Task<List<DTOOrder>> GetOrders(OrderType orderType, DateTime fromDate, DateTime toDate, string searchBy, bool showComments = false, IEnumerable<int> reposableTransIds = null)
        {
            return await Task.FromResult(_logisticsController.GetOrders(orderType, fromDate, toDate, searchBy, showComments, reposableTransIds));
        }        

        public async Task<List<DTOOrderPicture>> GetPictures(OrderType orderType, long orderId, int page, int pageSize)
        {
            return await Task.FromResult(_logisticsController.GetPictures(orderType, orderId, page, pageSize));
        }

        public async Task<List<DTOOrder>> GetPendingTransportOrders()
        {
            return await Task.FromResult(_logisticsController.GetPendingTransportOrders());
        }

        public async Task<DTOOrder> GetOrder(OrderType orderType, long orderId)
        {
            return await Task.FromResult(_logisticsController.GetOrder(orderType, orderId));
        }

        public async Task<List<DTOOrder>> SearchOrders(OrderType orderType, DateTime fromDate, DateTime toDate, string searchBy)
        {
            return await Task.FromResult(_logisticsController.SearchOrders(orderType, fromDate, toDate, searchBy));
        }
        
        public string DownloadPicture(DTOOrderPicture picture)
        {
            string path = Path.Combine(_wwwrootDirectory, string.Format("downloadPicture{0}", Path.GetExtension(picture.Name)));
            var fileStream = new FileStream(path, FileMode.Create);
            using (var stream = new FileStream(picture.Path, FileMode.Open))
            {
                stream.CopyTo(fileStream);
                fileStream.Close();
                return "";
            }
        }

        public async Task<List<DTOLogisticMasterData>> GetCustomers()
        {
            return await Task.FromResult(_logisticsController.GetCustomers());
        }

        public async Task<DTOLogisticMasterData> CreateOrUpdateCustomer(DTOLogisticMasterData dtoCustomer)
        {
            return await Task.FromResult(_logisticsController.CreateOrUpdateCustomer(dtoCustomer));
        }

        public async Task<List<DTOLogisticMasterData>> GetVendors()
        {
            return await Task.FromResult(_logisticsController.GetVendors());
        }


        public async Task<DTOLogisticMasterData> CreateOrUpdateVendor(DTOLogisticMasterData dtoVendor)
        {
            return await Task.FromResult(_logisticsController.CreateOrUpdateVendor(dtoVendor));
        }

        public async Task<List<DTOLogisticMasterData>> GetLogisticMasterData(string type)
        {
            return await Task.FromResult(_logisticsController.GetLogisticMasterData(type));
        }

        public async Task<DTOLogisticMasterData> CreateOrUpdateMasterData(DTOLogisticMasterData dtoLogisticMasterData)
        {
            return await Task.FromResult(_logisticsController.CreateOrUpdateLogisticMasterData(dtoLogisticMasterData));
        }

        public async Task<DTOOrder> CreateOrUpdateOrder(DTOOrder dtoOrder)
        {
            return await Task.FromResult(_logisticsController.CreateOrUpdateOrder(dtoOrder));
        }

        public async Task<bool> DeleteOrder(DTOOrder dtoOrder)
        {
            return await Task.FromResult(_logisticsController.DeleteOrder(dtoOrder));
        }

        public async Task<bool> ReSyncOrder(DTOOrder dtoOrder)
        {
            return await Task.FromResult(_logisticsController.ReSyncOrder(dtoOrder));
        }

        public List<DTOSharedOrder> GetSharedOrders(OrderType orderType, long orderId, int vehicleTypeId, DateTime dispatchDate)
        {
            return _logisticsController.GetSharedOrders(orderType, orderId, vehicleTypeId, dispatchDate);
        }

        public DTOOrder ChangeOrderState(DTOOrder dtoOrder)
        {
            return _logisticsController.ChangeOrderState(dtoOrder);
        }
    }
}
