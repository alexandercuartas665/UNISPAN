using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.DTO.PL;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.PL;
using adesoft.adepos.webview.Data.Model.Simex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Interfaces
{
    public interface IPlantService
    {
        public void AddPlantFilter(DTOPlantFilter filter);

        public DTOPlantFilter GetPlantFilter(string filterId);

        public List<DTOOrder> GetOrders(OrderType orderType, OrderStatus orderStatus, DateTime fromDate, DateTime toDate);

        public List<DTOOrder> GetBackOrders(OrderType orderType, OrderStatus orderStatus);

        public List<DTOOrder> GetPickingOrders(OrderType orderType, OrderStatus orderStatus);

        public DTOOrder GetOrder(OrderType orderType, long orderId);

        public List<DTOOrderProduct> GetOrderProducts(long orderId, string zoneProductId);

        public List<DTOOrderPallet> GetOrderPallets(long orderId);

        public List<DTOOrderPalletProduct> GetOrderPalletProducts(string orderPalletId);

        public OrderPalletProduct GetOrderPalletProduct(string orderPalletProductId);

        public OrderPallet GetOrderPallet(string orderPalletId);

        public bool SyncOrders(OrderType orderType, DateTime fromDate, DateTime toDate);

        public TransactionGeneric SyncOrder(TransactionGeneric trasactionGeneric);

        public LastSyncOrder GetLastSyncOrders();

        public DTOOrder UpdateOrder(DTOOrder dtoOrder);

        public ZoneProduct GetZoneProduct(string zoneProductId);

        public int GetPalletNo(long orderId);

        public List<DTOZoneProduct> GetZoneProducts();

        public DTOOrderPallet CreateOrUpdate(DTOOrderPallet dtoOrderPallet);

        public DTOZoneProduct CreateOrUpdate(DTOZoneProduct dtoZoneProduct);

        public List<DTOPackagingOrder> GetPendingPackagingOrders();

        public void SyncPallets(List<DTOOrder> orders);

        public List<DTOOrderProductVersion> GetOrderProductVersions(long orderId);

        public DTOOrderProductVersion GetOrderProductVersion(string orderProductVesionId);

        public DTOOrder GetOrderReport(string guidFilter);

        public DTOOrderPallet GetOrderPalletReport(string guidFilter);

        public List<DTOOrder> GetPendingOrders(OrderType orderType);

        public void DeletePallet(DTOOrderPallet dtoOrderPallet);

        public DTOOrder GetPickingStatus(string guidFilter);
    }
}
