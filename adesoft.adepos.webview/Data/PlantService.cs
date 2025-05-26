using adesoft.adepos.Extensions;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.DTO.PL;
using adesoft.adepos.webview.Data.Interfaces;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.PL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks; // Add this namespace


namespace adesoft.adepos.webview.Data
{
    public class PlantService : IPlantService
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        private readonly ConnectionDB _connectionDB;

        public static List<DTOPlantFilter> PlantFilters = new List<DTOPlantFilter>();

        private readonly ILogisticMasterDataService _logisticMasterDataService;

        public PlantService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogisticMasterDataService logisticMasterDataService)
        {
            _configuration = configuration;
            _connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (_connectionDB == null)
                _connectionDB = SecurityService.GetConnectionDefault();
            _dbcontext = new AdeposDBContext(_connectionDB.Connection);

            _logisticMasterDataService = logisticMasterDataService;
        }

        public void AddPlantFilter(DTOPlantFilter plantFilter)
        {
            PlantFilters.Add(plantFilter);
        }

        public DTOPlantFilter GetPlantFilter(string filterId)
        {
            return PlantFilters.Where(b => b.FilterId.Equals(filterId)).FirstOrDefault();
        }

        public static List<DTOOrderProduct> GetOrderProducts(AdeposDBContext _dbcontext, Order order)
        {
            try
            {
                var orderProducts = _dbcontext.OrderProducts
                    .Include(o => o.ZoneProduct)
                    .Where(o => o.OrderId.Equals(order.Id))
                    .Select(o => new DTOOrderProduct()
                    {
                        Description = o.Description,
                        ItemId = o.ItemId,
                        OrderId = o.OrderId,
                        OrderProductId = o.Id,
                        ZoneProductId = o.ZoneProductId,
                        ZoneProduct = new DTOZoneProduct()
                        {
                            ZoneProductId = o.ZoneProduct.Id,
                            Name = o.ZoneProduct.Name,
                            AltName = o.ZoneProduct.AltName
                        },
                        Qty = o.Qty,
                        QtyPending = o.QtyPending,
                        Reference = o.Reference,
                        Weight = o.Weight,
                        VersionCode = "",
                        LastQty = 0
                    })
                    .ToList();

                var orderProductVersion = _dbcontext.OrderProductVersions
                    .Include(o => o.OrderProductLogs)
                    .Where(o => o.OrderId.Equals(order.Id) && !o.Version.Equals(order.Version))
                    .OrderByDescending(o => o.Version)
                    .FirstOrDefault();
                if (orderProductVersion != null)
                {
                    foreach (var orderProduct in orderProducts)
                    {
                        var orderProductLog = orderProductVersion.OrderProductLogs
                            .Where(o => o.ItemId.Equals(orderProduct.ItemId) && o.OrderId.Equals(orderProduct.OrderId))
                            .FirstOrDefault();
                        if (orderProductLog != null)
                        {
                            orderProduct.VersionCode = orderProductVersion.VersionCode;
                            orderProduct.LastQty = orderProductLog.Qty;
                        }
                    }
                }

                return orderProducts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOOrder GetOrder(OrderType orderType, long orderId)
        {
            try
            {
                var order = _dbcontext.Orders
                    .Where(o => o.Id.Equals(orderId) && o.OrderType.Equals(orderType))
                    .Select(o => SelectOrder(_logisticMasterDataService, o))
                    .FirstOrDefault();

                var orderProducts = _dbcontext.OrderProducts
                    .Include(op => op.ZoneProduct)
                    .Where(op => op.OrderId.Equals(order.OrderId))
                    .Select(op => SelectOrderProduct(op))
                    .ToList();

                order.Products = orderProducts;

                if (order.Version != 0)
                {
                    var lastVersion = _dbcontext.OrderProductVersions
                    .Where(opv => opv.OrderId == orderId && opv.Version == (order.Version - 1))
                    .FirstOrDefault();

                    foreach (var product in orderProducts)
                    {
                        var productLog = _dbcontext.OrderProductLogs
                            .Where(opl => opl.OrderProductVersionId.Equals(lastVersion.Id)
                                && opl.ItemId.Equals(product.ItemId)
                                && opl.OrderId.Equals(orderId))
                            .FirstOrDefault();
                        if (productLog != null)
                        {
                            product.LastQty = productLog.Qty;
                            product.LastVersionCode = lastVersion.VersionCode;
                        }
                    }
                }

                var zoneProductIds = order.Products
                        .Select(op => op.ZoneProductId)
                        .Distinct()
                        .ToList();

                var zoneProducts = _dbcontext.ZoneProducts
                    .Where(zp => zoneProductIds.Contains(zp.Id))
                    .Select(zp => new DTOZoneProduct()
                    {
                        ZoneProductId = zp.Id,
                        Name = zp.Name,
                        AltName = zp.AltName
                    })
                    .ToList();

                order.ZoneProducts = zoneProducts;

                return order;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DTOOrder SelectOrder(ILogisticMasterDataService _logisticMasterDataService, Order order)
        {
            return new DTOOrder()
            {
                OrderId = order.Id,
                OrderType = order.OrderType,
                OrderNum = order.OrderNum,
                CustomerName = _logisticMasterDataService.GetCustomerName(order.CustomerAccount),
                Works = order.Works,
                TransDate = order.DispatchDateTime,
                TransDateFilter = order.DispatchDateTime.ToString("yyyy").Equals("0001") ? "Sin programar" : order.DispatchDateTime.ToString("dd/MM/yyyy"),
                Progress = (double)order.Progress,
                Status = order.Status,
                Wight = order.Wight,
                CreatedOn = order.CreatedOn,
                ModifiedOn = order.ModifiedOn,
                Version = order.Version,
                PalletNo = order.PalletNo,
                ModuleId = order.ModuleId,
                Module = _logisticMasterDataService.GetModuleName(order.ModuleId),
                DispatchDateTime = order.DispatchDateTime,
                ReturnDateTime = order.ReturnDateTime,
                CustomerAccount = order.CustomerAccount,
                SalesPersonId = order.SalesPersonId,
                CityId = order.CityId,
                City = _logisticMasterDataService.GetCityName(order.CityId),
                ReponsableTransId = order.ReponsableTransId,
                VehicleTypeId = order.VehicleTypeId,
                Email = order.Email,
            };
        }

        public static DTOOrderProduct SelectOrderProduct(OrderProduct orderProduct)
        {
            return new DTOOrderProduct()
            {
                Description = orderProduct.Description,
                ItemId = orderProduct.ItemId,
                OrderId = orderProduct.OrderId,
                OrderProductId = orderProduct.Id,
                Qty = orderProduct.Qty,
                LastQty = 0,
                QtyPending = orderProduct.QtyPending,
                Reference = orderProduct.Reference,
                Weight = orderProduct.Weight,
                ZoneProductId = string.IsNullOrEmpty(orderProduct.ZoneProductId) ? "" : orderProduct.ZoneProductId,
                ZoneProduct = string.IsNullOrEmpty(orderProduct.ZoneProductId) ? new DTOZoneProduct() : new DTOZoneProduct()
                {
                    Name = orderProduct.ZoneProduct.Name,
                    ZoneProductId = orderProduct.ZoneProduct.Id,
                    AltName = orderProduct.ZoneProduct.AltName
                }
            };
        }

        public List<DTOOrder> GetOrders(OrderType orderType, OrderStatus orderStatus, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var fDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
                var tDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

                var orders = new List<DTOOrder>();

                switch (orderType)
                {
                    case OrderType.Dispatch:
                    case OrderType.Return:
                        {
                            orders = _dbcontext.Orders
                                .Where(o => o.OrderType.Equals(orderType)
                                && (orders.Equals(OrderStatus.None) || o.Status.Equals(orderStatus))
                                && (fromDate.Equals(DateTime.MinValue) || (o.CreatedOn >= fDate))
                                && (toDate.Equals(DateTime.MinValue) || (o.CreatedOn <= tDate)))
                                .Select(o => SelectOrder(_logisticMasterDataService, o))
                                .ToList();
                            break;
                        }

                    case OrderType.None:
                        {
                            var dispatchs = _dbcontext.Orders
                                .Where(o => o.OrderType == OrderType.Dispatch
                                && (fromDate.Equals(DateTime.MinValue) || (o.CreatedOn >= fDate))
                                && (toDate.Equals(DateTime.MinValue) || (o.CreatedOn <= tDate)))
                                .Select(o => SelectOrder(_logisticMasterDataService, o))
                                .ToList();

                            if (dispatchs.Count != 0)
                                orders.AddRange(dispatchs);

                            var returns = _dbcontext.Orders
                                .Where(o => o.OrderType == OrderType.Return
                                && (fromDate.Equals(DateTime.MinValue) || (o.CreatedOn >= fDate))
                                && (toDate.Equals(DateTime.MinValue) || (o.CreatedOn <= tDate)))
                                .Select(o => SelectOrder(_logisticMasterDataService, o))
                                .ToList();

                            if (returns.Count != 0)
                                orders.AddRange(returns);

                            break;
                        }
                }

                foreach (var order in orders)
                {
                    var orderProducts = _dbcontext.OrderProducts
                        .Include(op => op.ZoneProduct)
                        .Where(op => op.OrderId.Equals(order.OrderId))
                        .Select(op => SelectOrderProduct(op))
                        .ToList();

                    //decimal totalQty = 0;
                    decimal totalPalletWeight = 0;

                    //orderProducts.ForEach(op =>
                    //{
                    //    totalQty += op.Qty;
                    //});

                    var zoneProductIds = orderProducts
                        .Select(op => op.ZoneProductId)
                        .Distinct()
                        .ToList();

                    var zoneProducts = _dbcontext.ZoneProducts
                        .Where(zp => zoneProductIds.Contains(zp.Id))
                        .Select(zp => new DTOZoneProduct()
                        {
                            ZoneProductId = zp.Id,
                            Name = zp.Name,
                            AltName = zp.AltName
                        })
                        .ToList();

                    order.ZoneProducts = zoneProducts;
                    order.Products = orderProducts;

                    var orderPallets = this.GetOrderPallets(order.OrderId);
                    order.OrderPallets = orderPallets;

                    if (orderPallets.Count != 0)
                    {
                        foreach (var orderPallet in orderPallets)
                        {
                            foreach (var orderPalletProduct in orderPallet.OrderPalletProducts)
                            {
                                var orderProduct = order.Products
                                    .Where(p => p.ItemId == orderPalletProduct.ItemId)
                                    .FirstOrDefault();

                                if (orderProduct != null && (orderProduct.Qty != 0))
                                {
                                    var totalWeight = orderProduct.Weight / orderProduct.Qty;

                                    //orderProduct.Qty -= orderPalletProduct.Qty;

                                    //if (orderPallet.Status == OrderPalletStatus.Syncronized)
                                    {
                                        totalPalletWeight += (totalWeight * orderPalletProduct.Qty);
                                    }
                                }
                            }
                        }
                    }

                    order.TotalPalletWeight = totalPalletWeight;

                    if (orderPallets.Count() != 0)
                    {
                        order.Progress = Math.Round((double)((totalPalletWeight * 100) / order.Wight), 2);
                    }
                    else
                    {
                        order.Progress = 0;
                    }
                }

                return orders.OrderByDescending(o => o.TransDate).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DTOOrder> GetPendingOrders(OrderType orderType)
        {
            try
            {
                var orders = new List<DTOOrder>();

                switch (orderType)
                {
                    case OrderType.Dispatch:
                    case OrderType.Return:
                        {
                            orders = _dbcontext.Orders
                                .Where(o => o.OrderType.Equals(orderType)
                                    && o.Status.Equals(OrderStatus.Draft))
                                .Select(o => SelectOrder(_logisticMasterDataService, o))
                                .ToList();
                            break;
                        }

                    case OrderType.None:
                        {
                            var dispatchs = _dbcontext.Orders
                                .Where(o => o.OrderType == OrderType.Dispatch
                                    && o.Status.Equals(OrderStatus.Draft))
                                .Select(o => SelectOrder(_logisticMasterDataService, o))
                                .ToList();

                            if (dispatchs.Count != 0)
                                orders.AddRange(dispatchs);

                            var returns = _dbcontext.Orders
                                .Where(o => o.OrderType == OrderType.Return
                                    && o.Status.Equals(OrderStatus.Draft))
                                .Select(o => SelectOrder(_logisticMasterDataService, o))
                                .ToList();

                            if (returns.Count != 0)
                                orders.AddRange(returns);

                            break;
                        }
                }

                foreach (var order in orders)
                {
                    var orderProducts = _dbcontext.OrderProducts
                        .Include(op => op.ZoneProduct)
                        .Where(op => op.OrderId.Equals(order.OrderId))
                        .Select(op => SelectOrderProduct(op))
                        .ToList();

                    if (order.Version != 0)
                    {
                        var orderProductVersion = _dbcontext.OrderProductVersions
                            .Where(ov => ov.Version == (order.Version - 1)
                                && ov.OrderId.Equals(order.OrderId))
                            .FirstOrDefault();

                        if (orderProductVersion != null)
                        {
                            var orderProductLogs = _dbcontext.OrderProductLogs
                                .Where(opl => opl.OrderId.Equals(order.OrderId)
                                    && opl.OrderProductVersionId.Equals(orderProductVersion.Id))
                                .ToList();

                            orderProducts.ForEach(op =>
                            {
                                var orderProductLog = orderProductLogs.Find(opl => opl.ItemId.Equals(op.ItemId));
                                if (orderProductLog != null)
                                {
                                    op.LastQty = orderProductLog.Qty;
                                }
                            });
                        }
                    }

                    decimal totalPalletWight = 0;

                    var zoneProductIds = orderProducts
                        .Select(op => op.ZoneProductId)
                        .Distinct()
                        .ToList();

                    var zoneProducts = _dbcontext.ZoneProducts
                        .Where(zp => zoneProductIds.Contains(zp.Id))
                        .Select(zp => new DTOZoneProduct()
                        {
                            ZoneProductId = zp.Id,
                            Name = zp.Name,
                            AltName = zp.AltName,
                        })
                        .ToList();

                    order.ZoneProducts = zoneProducts;
                    order.Products = orderProducts;

                    var orderPallets = this.GetOrderPallets(order.OrderId);
                    order.OrderPallets = orderPallets;

                    if (orderPallets.Count != 0)
                    {
                        bool versionChange = false;
                        foreach (var orderPallet in orderPallets)
                        {
                            foreach (var orderPalletProduct in orderPallet.OrderPalletProducts)
                            {
                                totalPalletWight += orderPalletProduct.Weight;
                            }

                            if (!versionChange)
                            {
                                versionChange = orderPallet.Status == OrderPalletStatus.VersionChange;
                            }
                        }
                        order.VersionChange = versionChange;
                    }

                    if (orderPallets.Count() != 0)
                    {
                        order.Progress = Math.Round((double)((totalPalletWight * 100) / order.Wight), 2);
                    }
                    else
                    {
                        order.Progress = 0;
                    }
                }

                var noProgramOrders = orders
                    .Where(o => o.DispatchDateTime.Value.ToString("yyyy").Equals("0001"))
                    .ToList();

                var programOrders = orders
                    .Where(o => !o.DispatchDateTime.Value.ToString("yyyy").Equals("0001"))
                    .OrderBy(o => o.DispatchDateTime)
                    .ToList();

                orders = programOrders;
                orders.AddRange(noProgramOrders);

                return orders;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DTOOrder> GetBackOrders(OrderType orderType, OrderStatus orderStatus)
        {
            return new List<DTOOrder>();
            //try
            //{
            //    var orders = this.GetOrders(orderType, orderStatus, DateTime.MinValue, DateTime.MinValue);

            //    foreach (var order in orders)
            //    {
            //        var backOrderPallets = order.OrderPallets
            //            .Where(op => op.Status.Equals(OrderPalletStatus.None) || op.Status.Equals(OrderPalletStatus.Syncronized))
            //            .ToList();

            //        order.OrderPallets = backOrderPallets;
            //    }

            //    foreach (var order in orders)
            //    {
            //        List<DTOZoneProduct> zoneProducts = new List<DTOZoneProduct> ();
            //        foreach (var zoneProduct in order.ZoneProducts)
            //        {
            //            var qty = order.Products.Where(p => p.ZoneProductId.Equals(zoneProduct.ZoneProductId)).Sum(p => p.Qty);
            //            if(qty != 0)
            //            {
            //                zoneProducts.Add(zoneProduct);
            //            }
            //        }

            //        order.ZoneProducts = zoneProducts;
            //    }

            //    return orders;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public List<DTOOrder> GetPickingOrders(OrderType orderType, OrderStatus orderStatus)
        {
            return new List<DTOOrder>();
            //try
            //{
            //    var orders = this.GetOrders(orderType, OrderStatus.None, DateTime.MinValue, DateTime.MinValue);

            //    foreach (var order in orders)
            //    {
            //        var backOrderPallets = order.OrderPallets
            //            .Where(op => op.Status.Equals(OrderPalletStatus.InProgress) || op.Status.Equals(OrderPalletStatus.Completed))
            //            .ToList();

            //        order.OrderPallets = backOrderPallets;
            //    }

            //    var pickingOrders = orders
            //        .Where(po => po.OrderPallets.Count != 0)
            //        .ToList();

            //    return pickingOrders;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public bool SyncOrders(OrderType orderType, DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (var dbContextTransaction = _dbcontext.Database.BeginTransaction())
                {
                    var trans = _dbcontext.TransactionGenerics
                        .Where(tg => tg.DateEnd.Date >= fromDate.Date && tg.DateEnd.Date <= toDate)
                        .ToList();
                    var newOrders = new List<Order>();
                    var updateOrders = new List<Order>();

                    foreach (var t in trans)
                    {
                        var order = _dbcontext.Orders
                            .Where(o => o.OrderNum.Equals(t.DocumentExtern) && o.Status.Equals(OrderStatus.None))
                            .FirstOrDefault();
                        if (order is null)
                        {
                            order = new Order()
                            {
                                OrderType = OrderType.Dispatch,
                                Status = OrderStatus.Draft,
                                Progress = 0,
                                OrderNum = t.DocumentExtern,
                                Works = t.NameWork,
                                DispatchDateTime = t.DateEnd,
                                TransactionGenericId = t.TransactionGenericId,
                                CreatedOn = DateTime.Now
                            };

                            newOrders.Add(order);
                        }
                        else
                        {
                            order.DispatchDateTime = t.DateEnd;
                            order.OrderType = OrderType.Dispatch;
                            order.Status = OrderStatus.Draft;

                            updateOrders.Add(order);
                        }
                    }

                    if (newOrders.Count != 0)
                    {
                        _dbcontext.Orders.AddRange(newOrders);
                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();

                        foreach (var o in newOrders)
                        {
                            var products = _dbcontext.DetailTransactionGenerics
                            .Where(d => d.TransactionGenericId.Equals(o.TransactionGenericId))
                            .Include(d => d.Item)
                            .Select(d => new OrderProduct()
                            {
                                Id = Guid.NewGuid().ToString(),
                                OrderId = o.Id,
                                ItemId = d.ItemId,
                                Reference = d.Item.Referencia,
                                Description = d.Item.Description,
                                ZoneProductId = d.Item.ZoneProductId,
                                Qty = d.Cant,
                                Weight = (d.Cant * d.Item.Weight) / 1000
                            })
                            .ToList();

                            _dbcontext.OrderProducts.AddRange(products);
                            _dbcontext.SaveChanges();
                            _dbcontext.DetachAll();
                        }
                    }

                    if (updateOrders.Count != 0)
                    {
                        foreach (var uo in updateOrders)
                        {
                            var totalWeight = 0;

                            var productsUpd = _dbcontext.OrderProducts
                                .Where(p => p.OrderId.Equals(uo.Id))
                                .ToList();

                            var productsNew = new List<OrderProduct>();

                            var products = _dbcontext.DetailTransactionGenerics
                            .Where(d => d.TransactionGenericId.Equals(uo.TransactionGenericId))
                            .Include(d => d.Item)
                            .ToList();

                            foreach (var product in products)
                            {
                                var productUpd = productsUpd.Where(p => p.OrderId.Equals(uo.Id) && p.ItemId.Equals(product.ItemId)).FirstOrDefault();
                                if (!(productUpd is null))
                                {
                                    productUpd.Reference = product.Item.Referencia;
                                    productUpd.Description = product.Item.Description;
                                    productUpd.Qty = product.Cant;
                                    productUpd.ZoneProductId = product.Item.ZoneProductId;
                                    productUpd.Weight = (product.Cant * product.Item.Weight) / 1000;
                                }
                                else
                                {
                                    productsNew.Add(new OrderProduct()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        OrderId = uo.Id,
                                        ItemId = product.ItemId,
                                        Reference = product.Item.Referencia,
                                        Description = product.Item.Description,
                                        ZoneProductId = product.Item.ZoneProductId,
                                        Qty = product.Cant,
                                        Weight = (product.Cant * product.Item.Weight) / 1000
                                    });
                                }
                            }

                            _dbcontext.OrderProducts.UpdateRange(productsUpd);
                            _dbcontext.SaveChanges();
                            _dbcontext.DetachAll();

                            if (productsNew.Count() != 0)
                            {
                                _dbcontext.OrderProducts.AddRange(productsNew);
                                _dbcontext.SaveChanges();
                                _dbcontext.DetachAll();
                            }
                        }

                        _dbcontext.Orders.UpdateRange(updateOrders);
                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();
                    }

                    this.SaveLastSyncOrder();

                    dbContextTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public void SaveLastSyncOrder()
        {
            var lastSyncOrder = new LastSyncOrder
            {
                LastSyncDate = DateTime.Now
            };

            _dbcontext.LastSyncOrders.Add(lastSyncOrder);
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();
        }

        public LastSyncOrder GetLastSyncOrders()
        {
            var lastUpdateModule = _dbcontext.LastSyncOrders
                .OrderByDescending(l => l.LastSyncDate)
                .FirstOrDefault();

            return lastUpdateModule;
        }

        public DTOOrder UpdateOrder(DTOOrder dtoOrder)
        {
            var order = _dbcontext.Orders.Where(o => o.Id == dtoOrder.OrderId).FirstOrDefault();

            order.OrderNum = dtoOrder.OrderNum != null ? dtoOrder.OrderNum : "";
            order.OPNum = dtoOrder.OPNum;
            order.Works = dtoOrder.Works;
            order.SalesPersonId = dtoOrder.SalesPersonId;
            order.ModuleId = dtoOrder.ModuleId;
            order.CityId = dtoOrder.CityId;
            order.ReponsableTransId = dtoOrder.ReponsableTransId;
            order.DispatchDateTime = dtoOrder.DispatchDateTime.Value;
            order.ReturnDateTime = dtoOrder.ReturnDateTime.Value;
            order.Wight = dtoOrder.Wight;
            order.VehicleTypeId = dtoOrder.VehicleTypeId;
            order.CustomerAccount = dtoOrder.CustomerAccount;

            order.PlateNum = dtoOrder.PlateNum;
            order.VendorAccount = dtoOrder.VendorAccount;
            order.DriverName = dtoOrder.DriverName;
            order.InvoiceAmount = dtoOrder.InvoiceAmount;
            order.InvoiceDate = dtoOrder.InvoiceDate;
            order.InvoiceNum = dtoOrder.InvoiceNum;
            order.DispatchId = dtoOrder.OrderId;

            _dbcontext.Orders.Update(order);
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();

            return dtoOrder;
        }

        public static DTOOrderPallet OrderPalletSelect(AdeposDBContext _dbcontext, OrderPallet orderPallet)
        {
            DTOOrderPallet dtoOrderPaller = new DTOOrderPallet()
            {
                OrderPalletId = orderPallet.Id,
                OrderId = orderPallet.OrderId,
                PalletNo = orderPallet.PalletNo,
                QRData = orderPallet.QRData,
                Status = orderPallet.Status,
                CreatedOn = orderPallet.CreatedOn,
                ModifiedOn = orderPallet.ModifiedOn,
                ZoneProductId = orderPallet.ZoneProductId,
                ZoneProduct = new DTOZoneProduct()
                {
                    ZoneProductId = orderPallet.ZoneProduct.Id,
                    Name = orderPallet.ZoneProduct.Name,
                    AltName = orderPallet.ZoneProduct.AltName
                },
            };

            var orderPalletProducts = _dbcontext.OrderPalletProducts
                .Where(opp => opp.OrderPalletId.Equals(orderPallet.Id))
                .Select(opp => new DTOOrderPalletProduct()
                {
                    OrderPalletProductId = opp.Id,
                    OrderPalletId = opp.OrderPalletId,
                    ItemId = opp.ItemId,
                    Description = opp.Description,
                    Weight = opp.Weight,
                    Qty = opp.Qty,
                    SelectedQty = opp.Qty,
                    Selected = true,
                    Reference = opp.Reference,
                    OrderId = opp.OrderId
                })
                .ToList();

            dtoOrderPaller.OrderPalletProducts = orderPalletProducts;

            return dtoOrderPaller;
        }

        public List<DTOOrderPallet> GetOrderPallets(long orderId)
        {
            try
            {
                var pallets = _dbcontext.OrderPallets
                    .Include(p => p.ZoneProduct)
                    .Where(p => p.OrderId.Equals(orderId))
                    .OrderBy(p => p.PalletNo)
                    .Select(p => OrderPalletSelect(_dbcontext, p))
                    .ToList();

                return pallets;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public OrderPallet GetOrderPallet(string orderPalletId)
        {
            try
            {
                var pallet = _dbcontext.OrderPallets
                    .Include(p => p.ZoneProduct)
                    .Where(p => p.Id.Equals(orderPalletId))
                    .FirstOrDefault();

                return pallet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ZoneProduct GetZoneProduct(string zoneProductId)
        {
            try
            {
                var zoneProduct = _dbcontext.ZoneProducts
                    .Where(z => z.Id.Equals(zoneProductId))
                    .FirstOrDefault();

                return zoneProduct;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DTOZoneProduct> GetZoneProducts()
        {
            try
            {
                var zoneProducts = _dbcontext.ZoneProducts
                    .Select(z => z.ToModel())
                    .ToList();
                return zoneProducts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetPalletNo(long orderId)
        {
            try
            {
                var count = _dbcontext.OrderPallets.Count(p => p.OrderId.Equals(orderId));
                return count + 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DTOOrderPallet CreateOrUpdate(DTOOrderPallet dtoOrderPallet)
        {
            try
            {
                OrderPallet orderPallet = null;

                if (!string.IsNullOrEmpty(dtoOrderPallet.OrderPalletId))
                {
                    orderPallet = this.GetOrderPallet(dtoOrderPallet.OrderPalletId);
                    if (orderPallet is null)
                        throw new Exception("La estiba no fue encontrada");

                    orderPallet.Status = dtoOrderPallet.Status;

                    _dbcontext.OrderPallets.Update(orderPallet);
                }
                else
                {
                    orderPallet = new OrderPallet()
                    {
                        Id = Guid.NewGuid().ToString(),
                        OrderId = dtoOrderPallet.OrderId,
                        PalletNo = dtoOrderPallet.PalletNo,
                        Status = OrderPalletStatus.Syncronized,
                        ZoneProductId = dtoOrderPallet.ZoneProductId,
                        QRData = string.Empty
                    };

                    _dbcontext.OrderPallets.Add(orderPallet);
                }

                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();

                return this.GetOrderPallet(orderPallet.Id).ToModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DTOZoneProduct CreateOrUpdate(DTOZoneProduct dtoZoneProduct)
        {
            try
            {
                ZoneProduct zoneProduct = null;

                if (!string.IsNullOrEmpty(dtoZoneProduct.ZoneProductId))
                {
                    zoneProduct = this.GetZoneProduct(dtoZoneProduct.ZoneProductId);
                    if (zoneProduct is null)
                        throw new Exception("La zona no fue encontrada");

                    zoneProduct.Name = dtoZoneProduct.Name;
                    zoneProduct.AltName = dtoZoneProduct.AltName;

                    _dbcontext.ZoneProducts.Update(zoneProduct);
                }
                else
                {
                    zoneProduct = new ZoneProduct()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = dtoZoneProduct.Name,
                        AltName = dtoZoneProduct.AltName
                    };

                    _dbcontext.ZoneProducts.Add(zoneProduct);
                }

                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();

                return this.GetZoneProduct(zoneProduct.Id).ToModel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeletePallet(DTOOrderPallet dtoOrderPallet)
        {

            var orderPalletProducts = _dbcontext.OrderPalletProducts
            .Where(op => op.OrderPalletId == dtoOrderPallet.OrderPalletId)
            .ToList();

            _dbcontext.OrderPalletProducts.RemoveRange(orderPalletProducts);

            var orderPallet = _dbcontext.OrderPallets
                .Where(op => op.Id == dtoOrderPallet.OrderPalletId)
                .FirstOrDefault();

            _dbcontext.OrderPallets.Remove(orderPallet);

            _dbcontext.SaveChanges(true);
            _dbcontext.DetachAll();
        }

        public List<DTOOrderPalletProduct> GetOrderPalletProducts(string orderPalletId)
        {
            try
            {
                var orderPalletProducts = _dbcontext.OrderPalletProducts
                    .Include(p => p.OrderPallet)
                    //.Include(p => p.OrderProduct)
                    .Where(p => p.OrderPalletId.Equals(orderPalletId))
                    .Select(p => p.ToModel())
                    .ToList();

                return orderPalletProducts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public OrderPalletProduct GetOrderPalletProduct(string orderPalletProductId)
        {
            try
            {
                var orderPalletProduct = _dbcontext.OrderPalletProducts
                    .Include(p => p.ZoneProduct)
                    .Where(p => p.Id.Equals(orderPalletProductId))
                    .FirstOrDefault();

                return orderPalletProduct;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DTOOrderProduct> GetOrderProducts(long orderId, string zoneProductId)
        {
            try
            {
                var orderProducts = _dbcontext.OrderProducts
                    .Where(op => op.OrderId.Equals(orderId) && op.ZoneProductId.Equals(zoneProductId))
                    .Select(op => op.ToModel())
                    .ToList();

                return orderProducts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TransactionGeneric SyncOrder(TransactionGeneric transactionGeneric)
        {
            using (var dbContextTransaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    int version = 0;
                    var order = _dbcontext.Orders
                            .Where(o => o.TransactionGenericId.Equals(transactionGeneric.TransactionGenericId)
                            && (o.Status.Equals(OrderStatus.None) || o.Status.Equals(OrderStatus.Paused)))
                            .FirstOrDefault();

                    if (!(order is null))
                    {
                        order.OrderNum = transactionGeneric.DocumentExtern;
                        order.Works = transactionGeneric.NameWork;
                        order.CustomerAccount = transactionGeneric.CustomerAccount;
                        order.SalesPersonId = transactionGeneric.SalesPersonId;
                        order.ModuleId = transactionGeneric.ModuleId;
                        order.CityId = transactionGeneric.CityId;
                        order.ReponsableTransId = transactionGeneric.ReponsableTransId;
                        order.VehicleTypeId = transactionGeneric.VehicleTypeId;
                        order.Wight = transactionGeneric.Wight;
                        order.Status = OrderStatus.Draft;
                        order.ModifiedOn = DateTime.Now;

                        version = order.Version + 1;
                        order.Version = version;

                        _dbcontext.Orders.Update(order);
                    }
                    else
                    {

                        order = new Order()
                        {
                            OrderType = OrderType.Dispatch,
                            Status = OrderStatus.Draft,
                            Progress = 0,
                            OrderNum = transactionGeneric.DocumentExtern,
                            Works = transactionGeneric.NameWork,
                            DispatchDateTime = !transactionGeneric.Scheduled ? DateTime.MinValue : transactionGeneric.DateEnd,
                            TransactionGenericId = transactionGeneric.TransactionGenericId,
                            CustomerAccount = transactionGeneric.CustomerAccount,
                            SalesPersonId = transactionGeneric.SalesPersonId,
                            ModuleId = transactionGeneric.ModuleId,
                            CityId = transactionGeneric.CityId,
                            ReponsableTransId = transactionGeneric.ReponsableTransId,
                            VehicleTypeId = transactionGeneric.VehicleTypeId,
                            Wight = transactionGeneric.Wight,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            Version = version,
                            PalletNo = 1
                        };

                        _dbcontext.Orders.Add(order);

                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();
                    }

                    OrderProductVersion orderProductVersion = new OrderProductVersion();
                    orderProductVersion.Id = Guid.NewGuid().ToString();
                    orderProductVersion.CreatedOn = DateTime.Now;
                    orderProductVersion.OrderId = order.Id;
                    orderProductVersion.VersionCode = $"V{version}";
                    orderProductVersion.Version = version;

                    _dbcontext.OrderProductVersions.Add(orderProductVersion);

                    var products = new List<OrderProduct>();
                    if (!version.Equals(0))
                    {
                        products = _dbcontext.OrderProducts
                            .Where(p => p.OrderId.Equals(order.Id))
                            .ToList();

                        _dbcontext.OrderProducts.RemoveRange(products);
                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();
                    }

                    products = _dbcontext.DetailTransactionGenerics
                        .Where(d => d.TransactionGenericId.Equals(order.TransactionGenericId))
                        .Include(d => d.Item)
                        .Select(d => new OrderProduct()
                        {
                            Id = Guid.NewGuid().ToString(),
                            OrderId = order.Id,
                            ItemId = d.ItemId,
                            Reference = d.Item.Referencia,
                            Description = d.Item.Description,
                            ZoneProductId = d.Item.ZoneProductId,
                            Qty = d.Cant,
                            QtyPending = d.Cant,
                            Weight = (d.Cant * d.Item.Weight) / 1000
                        })
                        .ToList();

                    _dbcontext.OrderProducts.AddRange(products);

                    var productLogs = products
                        .Select(p => new OrderProductLog()
                        {
                            OrderProductVersionId = orderProductVersion.Id,
                            Id = p.Id,
                            Description = p.Description,
                            ItemId = p.ItemId,
                            OrderId = p.OrderId,
                            Qty = p.Qty,
                            QtyPending = p.QtyPending,
                            Reference = p.Reference,
                            Weight = p.Weight,
                            ZoneProductId = p.ZoneProductId
                        })
                        .ToList();

                    _dbcontext.OrderProductLogs.AddRange(productLogs);

                    transactionGeneric = _dbcontext.TransactionGenerics.Where(t => t.TransactionGenericId.Equals(transactionGeneric.TransactionGenericId)).FirstOrDefault();
                    if (!(transactionGeneric is null))
                    {
                        transactionGeneric.Scheduled = true;
                        _dbcontext.TransactionGenerics.Update(transactionGeneric);
                    }

                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();

                    var orderPallets = _dbcontext.OrderPallets
                        .Where(op => op.OrderId == order.Id)
                        .ToList();

                    if (orderPallets.Count != 0 && (order.Version > 0))
                    {
                        foreach (var product in products)
                        {
                            orderProductVersion = _dbcontext.OrderProductVersions
                                .Where(opv => opv.Version == order.Version - 1)
                                .FirstOrDefault();

                            var orderProductLog = _dbcontext.OrderProductLogs
                                .Where(opl => opl.OrderProductVersionId == orderProductVersion.Id
                                && opl.ItemId == product.ItemId)
                                .FirstOrDefault();

                            var pickedQty = _dbcontext.OrderPalletProducts
                                .Where(opp => opp.OrderId == order.Id
                                && opp.ItemId == product.ItemId)
                                .Sum(opp => opp.Qty);

                            var orderPalletProducts = _dbcontext.OrderPalletProducts
                                .Where(opp => opp.OrderId == order.Id
                                && opp.ItemId == product.ItemId)
                                .ToList();

                            if ((pickedQty != 0) && (product.Qty != pickedQty) && (orderProductLog != null) && (orderProductLog.Qty != product.Qty))
                            {
                                foreach (var orderPalletProduct in orderPalletProducts)
                                {
                                    var orderPalletsControlVersion = _dbcontext.OrderPallets
                                        .Where(op => op.Id == orderPalletProduct.OrderPalletId
                                        && op.OrderId == order.Id)
                                        .ToList();

                                    orderPalletsControlVersion.ForEach(op =>
                                    {
                                        op.Status = OrderPalletStatus.VersionChange;
                                    });

                                    if (orderPalletsControlVersion.Count != 0)
                                    {
                                        orderPalletProduct.IsModified = true;
                                    }

                                    _dbcontext.OrderPallets.UpdateRange(orderPalletsControlVersion);

                                    _dbcontext.SaveChanges();
                                    _dbcontext.DetachAll();
                                }
                            }
                            else
                            {
                                foreach (var orderPalletProduct in orderPalletProducts)
                                {
                                    var orderPalletsControlVersion = _dbcontext.OrderPallets
                                        .Where(op => op.Id == orderPalletProduct.OrderPalletId
                                        && op.OrderId == order.Id)
                                        .ToList();

                                    orderPalletsControlVersion.ForEach(op =>
                                    {
                                        op.Status = OrderPalletStatus.Syncronized;
                                    });

                                    _dbcontext.OrderPallets.UpdateRange(orderPalletsControlVersion);

                                    _dbcontext.SaveChanges();
                                    _dbcontext.DetachAll();
                                }
                            }

                            _dbcontext.OrderPalletProducts.UpdateRange(orderPalletProducts);
                            _dbcontext.SaveChanges();
                            _dbcontext.DetachAll();
                        }

                        var orderPalletProductsDelete = new List<OrderPalletProduct>();

                        var orderPalletProducts2 = _dbcontext.OrderPalletProducts
                            .Where(opp => opp.OrderId == order.Id)
                            .ToList();

                        foreach (var orderPalletProduct in orderPalletProducts2)
                        {
                            var product = products.Where(p => p.ItemId == orderPalletProduct.ItemId).FirstOrDefault();
                            if (product is null)
                            {
                                orderPalletProductsDelete.Add(orderPalletProduct);
                            }
                        }

                        foreach (var orderPalletProduct in orderPalletProductsDelete)
                        {
                            orderPalletProduct.Qty = -orderPalletProduct.Qty;

                            var orderPalletsDelete = _dbcontext.OrderPallets
                                .Where(op => op.Id == orderPalletProduct.OrderPalletId
                                && op.OrderId == orderPalletProduct.OrderId)
                                .ToList();

                            orderPalletsDelete.ForEach(opd =>
                            {
                                opd.Status = OrderPalletStatus.VersionChange;
                            });

                            _dbcontext.OrderPallets.UpdateRange(orderPalletsDelete);
                            _dbcontext.SaveChanges();
                            _dbcontext.DetachAll();
                        }

                        _dbcontext.OrderPalletProducts.UpdateRange(orderPalletProductsDelete);
                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();
                    }
                    dbContextTransaction.Commit();
                    //ENVIA EL CORREO ELECTRONICO AL CLIENTE
                    string msg = string.Empty;
                    msg = "Se ha enviado un correo de confirmacion a la direccion de correo.";
                    try
                    {
                        Task.Run(() =>
                        {
                            SmtpClient client = new SmtpClient("smtp.office365.com", 587);
                            client.UseDefaultCredentials = false;
                            client.EnableSsl = true;
                            client.Credentials = new NetworkCredential("formularios@unispan.com.co", "Sistemas2024*");
                            MailMessage mailMessage = new MailMessage();
                            mailMessage.From = new MailAddress("formularios@unispan.com.co");
                            mailMessage.To.Add("sistemas@unispan.com.co");
                            mailMessage.IsBodyHtml = true;
                            mailMessage.Body = "<p>CORREO ELECTRONICO ENVIADO DESDE UNISPAN.APP.</p>"
                                                + "<p>este es un correo de prueba enviados desde el sistema: </p> <a href=\"http://18.223.15.205:8010/\">CrediMax.com<a>"; 
                            mailMessage.Subject = "Registro exitoso en credimax";
                            client.Send(mailMessage);
                        });
                    }
                    catch (Exception ex)
                    {

                    }



                    return transactionGeneric;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<DTOPackagingOrder> GetPendingPackagingOrders()
        {
            try
            {
                var orders = _dbcontext.Orders
                    .Where(o => o.Status.Equals(OrderStatus.None))
                    .Select(o => new DTOPackagingOrder()
                    {
                        OrderId = o.Id,
                        OrderNum = o.OrderNum,
                        OrderType = o.OrderType,
                        Works = o.Works,
                        DispatchDateTime = o.DispatchDateTime
                    })
                    .ToList();

                return orders;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ValidateVersion(DTOOrder dtoOrder)
        {

        }

        public string CreateQRCode(string qrCodeText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeInfo = qrGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeInfo);
            Bitmap qrBitmap = qrCode.GetGraphic(60);

            MemoryStream stream = new MemoryStream();
            qrBitmap.Save(stream, ImageFormat.Png);
            byte[] buffer = stream.ToArray();

            string qrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(buffer));
            return qrUri;
        }

        public void SyncPallets(List<DTOOrder> orders)
        {
            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var dtoOrdersUpd = orders.Where(o => (o.OrderPallets != null) && (o.OrderPallets.Count > 0)).ToList();
                    foreach (var dtoOrderUpd in dtoOrdersUpd)
                    {
                        var order = _dbcontext.Orders
                            .Where(o => o.Id == dtoOrderUpd.OrderId)
                            .FirstOrDefault();

                        if (!(order is null) && (dtoOrderUpd.ModifiedOn >= order.ModifiedOn))
                        {
                            order.PalletNo = dtoOrderUpd.PalletNo;
                            order.ModifiedOn = dtoOrderUpd.ModifiedOn;

                            _dbcontext.Orders.Update(order);

                            foreach (var dtoOrderPallet in dtoOrderUpd.OrderPallets)
                            {
                                var orderPalletOrig = this.GetOrderPallet(dtoOrderPallet.OrderPalletId);
                                if (orderPalletOrig is null)
                                {
                                    if (dtoOrderPallet.Status != OrderPalletStatus.Deleted)
                                    {
                                        var orderPallet = new OrderPallet()
                                        {
                                            Id = dtoOrderPallet.OrderPalletId,
                                            OrderId = dtoOrderPallet.OrderId,
                                            PalletNo = dtoOrderPallet.PalletNo,
                                            QRData = this.CreateQRCode(dtoOrderPallet.OrderPalletId),
                                            Status = (dtoOrderPallet.Status == OrderPalletStatus.None ? OrderPalletStatus.Syncronized : dtoOrderPallet.Status),
                                            ZoneProductId = dtoOrderPallet.ZoneProductId,
                                            CreatedOn = dtoOrderPallet.CreatedOn,
                                            ModifiedOn = dtoOrderPallet.ModifiedOn,
                                            Version = dtoOrderUpd.Version
                                        };

                                        _dbcontext.OrderPallets.Add(orderPallet);
                                    }
                                }
                                else
                                {
                                    if (dtoOrderPallet.Status != OrderPalletStatus.Deleted)
                                    {
                                        orderPalletOrig.Status = (dtoOrderPallet.Status == OrderPalletStatus.None ? OrderPalletStatus.Syncronized : dtoOrderPallet.Status);
                                        orderPalletOrig.ModifiedOn = dtoOrderPallet.ModifiedOn;
                                        _dbcontext.OrderPallets.Update(orderPalletOrig);
                                    }
                                    else
                                    {
                                        this.DeletePallet(dtoOrderPallet);
                                    }
                                }

                                if (dtoOrderPallet.Status != OrderPalletStatus.Deleted)
                                {
                                    var orderPalletProducts = _dbcontext.OrderPalletProducts
                                        .Where(opp => opp.OrderPalletId.Equals(dtoOrderPallet.OrderPalletId))
                                        .ToList();
                                    if (orderPalletProducts.Count != 0)
                                    {
                                        _dbcontext.OrderPalletProducts.RemoveRange(orderPalletProducts);

                                        _dbcontext.SaveChanges();
                                        _dbcontext.DetachAll();
                                    }

                                    foreach (var dtoOrderPalletProduct in dtoOrderPallet.OrderPalletProducts)
                                    {
                                        var orderPalletProductOrig = this.GetOrderPalletProduct(dtoOrderPalletProduct.OrderPalletProductId);
                                        if (orderPalletProductOrig is null)
                                        {
                                            var orderPalletProduct = new OrderPalletProduct()
                                            {
                                                Id = dtoOrderPalletProduct.OrderPalletProductId,
                                                OrderPalletId = dtoOrderPalletProduct.OrderPalletId,
                                                Description = dtoOrderPalletProduct.Description,
                                                Reference = dtoOrderPalletProduct.Reference,
                                                ItemId = dtoOrderPalletProduct.ItemId,
                                                Weight = dtoOrderPalletProduct.Weight,
                                                Qty = dtoOrderPalletProduct.SelectedQty,
                                                ZoneProductId = dtoOrderPallet.ZoneProductId,
                                                OrderId = dtoOrderPallet.OrderId
                                            };

                                            _dbcontext.OrderPalletProducts.Add(orderPalletProduct);
                                        }
                                    }
                                }

                                _dbcontext.SaveChanges();
                                _dbcontext.DetachAll();
                            }

                            var orderPallets = _dbcontext.OrderPallets
                                .Where(op => op.OrderId == dtoOrderUpd.OrderId)
                                .ToList();

                            if (orderPallets.Count != 0 && (order.Version > 0))
                            {
                                var orderProducts = _dbcontext.OrderProducts
                                .Where(op => op.OrderId == dtoOrderUpd.OrderId)
                                .ToList();

                                foreach (var product in orderProducts)
                                {
                                    var pickedQty = _dbcontext.OrderPalletProducts
                                            .Where(opp => opp.OrderId == order.Id
                                            && opp.ItemId == product.ItemId)
                                            .Sum(opp => opp.Qty);

                                    if ((pickedQty != 0) && (product.Qty < pickedQty))
                                    {
                                        var orderPalletProducts = _dbcontext.OrderPalletProducts
                                            .Where(opp => opp.OrderId == order.Id
                                            && opp.ItemId == product.ItemId)
                                            .ToList();

                                        foreach (var orderPalletProduct in orderPalletProducts)
                                        {
                                            var orderPalletsControlVersion = _dbcontext.OrderPallets
                                                .Where(op => op.Id == orderPalletProduct.OrderPalletId
                                                && op.OrderId == order.Id)
                                                .ToList();

                                            orderPalletsControlVersion.ForEach(op =>
                                            {
                                                op.Status = OrderPalletStatus.VersionChange;
                                            });

                                            if (orderPalletsControlVersion.Count != 0)
                                            {
                                                orderPalletProduct.IsModified = true;
                                            }

                                            _dbcontext.OrderPallets.UpdateRange(orderPalletsControlVersion);

                                            _dbcontext.SaveChanges();
                                            _dbcontext.DetachAll();
                                        }

                                        _dbcontext.OrderPalletProducts.UpdateRange(orderPalletProducts);
                                        _dbcontext.SaveChanges();
                                        _dbcontext.DetachAll();
                                    }
                                    else
                                    {
                                        var orderPalletProducts = _dbcontext.OrderPalletProducts
                                            .Where(opp => opp.OrderId == order.Id
                                            && opp.ItemId == product.ItemId)
                                            .ToList();

                                        foreach (var orderPalletProduct in orderPalletProducts)
                                        {
                                            var orderPalletControlVersion = _dbcontext.OrderPallets
                                                .Where(op => op.Id == orderPalletProduct.OrderPalletId
                                                && op.OrderId == order.Id)
                                                .FirstOrDefault();

                                            if (orderPalletControlVersion != null && orderPalletControlVersion.Status == OrderPalletStatus.VersionChange)
                                            {
                                                orderPalletControlVersion.Status = OrderPalletStatus.Syncronized;

                                                _dbcontext.OrderPallets.Update(orderPalletControlVersion);

                                                _dbcontext.SaveChanges();
                                                _dbcontext.DetachAll();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<DTOOrderProductVersion> GetOrderProductVersions(long orderId)
        {
            try
            {
                var orderProductVersions = _dbcontext.OrderProductVersions
                    .Where(o => o.OrderId.Equals(orderId))
                    .Select(o => new DTOOrderProductVersion()
                    {
                        OrderProductVersionId = o.Id,
                        OrderId = o.OrderId,
                        Version = o.Version,
                        VersionCode = o.VersionCode,
                        CreatedOn = o.CreatedOn
                    })
                    .OrderBy(o => o.Version)
                    .ToList();

                return orderProductVersions;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOOrderProductVersion GetOrderProductVersion(string orderProductVesionId)
        {
            try
            {
                var orderProductVersion = _dbcontext.OrderProductVersions
                    .Where(o => o.Id.Equals(orderProductVesionId))
                    .Select(o => new DTOOrderProductVersion()
                    {
                        OrderProductVersionId = o.Id,
                        OrderId = o.OrderId,
                        Version = o.Version,
                        VersionCode = o.VersionCode,
                        CreatedOn = o.CreatedOn,
                        OrderProductLogs = GetOrderProductLogs(_dbcontext, o)
                    }).FirstOrDefault();

                return orderProductVersion;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<DTOOrderProductLog> GetOrderProductLogs(AdeposDBContext _dbcontext, OrderProductVersion orderProductVersion)
        {
            List<DTOOrderProductLog> orderProductLogs = _dbcontext.OrderProductLogs
                .Include(o => o.ZoneProduct)
                .Where(o => o.OrderProductVersionId.Equals(orderProductVersion.Id))
                .Select(o => new DTOOrderProductLog()
                {
                    Description = o.Description,
                    ItemId = o.ItemId,
                    OrderId = o.OrderId,
                    OrderProductLogId = o.Id,
                    Qty = o.Qty,
                    QtyPending = o.QtyPending,
                    Reference = o.Reference,
                    Weight = o.Weight,
                    ZoneProductId = o.ZoneProductId,
                    ZoneProduct = new DTOZoneProduct()
                    {
                        ZoneProductId = o.ZoneProduct.Id,
                        Name = o.ZoneProduct.Name,
                        AltName = o.ZoneProduct.AltName,
                    }
                })
                .ToList();

            return orderProductLogs;
        }

        public DTOOrder GetOrderReport(string guidFilter)
        {
            try
            {
                DTOPlantFilter plantFilter = PlantFilters.Where(x => x.FilterId == guidFilter).FirstOrDefault();

                List<DTOOrderProduct> products = new List<DTOOrderProduct>();

                if (plantFilter.OrderSelected != null && !string.IsNullOrEmpty(plantFilter.ZoneProductId))
                {
                    products = plantFilter.OrderSelected.Products.Where(p => p.ZoneProductId.Equals(plantFilter.ZoneProductId)).ToList();
                    plantFilter.OrderSelected.Products = products;
                }

                return plantFilter.OrderSelected;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOOrderPallet GetOrderPalletReport(string guidFilter)
        {
            try
            {
                DTOPlantFilter plantFilter = PlantFilters.Where(x => x.FilterId == guidFilter).FirstOrDefault();
                DTOOrder order = plantFilter.OrderSelected;
                DTOOrderPallet orderPallet = plantFilter.OrderPalletSelected;

                if (orderPallet != null)
                {
                    var dispatchDateTime = order.DispatchDateTime.Value;
                    orderPallet.OrderNum = order.OrderNum;
                    orderPallet.Works = order.Works;
                    orderPallet.DispatchDate = !dispatchDateTime.ToString("yyyy").Equals("0001") ? dispatchDateTime.ToString("dd-MM-yyyy") : "";
                    orderPallet.DispatchTime = !dispatchDateTime.ToString("yyyy").Equals("0001") ? dispatchDateTime.ToString("hh:mm tt") : "";
                    orderPallet.ZoneName = orderPallet.ZoneProduct.Name;
                    orderPallet.Module = order.Module;
                }

                return orderPallet;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOOrder GetPickingStatus(string guidFilter)
        {
            try
            {
                DTOPlantFilter plantFilter = PlantFilters.Where(x => x.FilterId == guidFilter).FirstOrDefault();
                DTOOrder order = plantFilter.OrderSelected;

                var products = order.Products
                    .Where(p => string.IsNullOrEmpty(plantFilter.ZoneProductId) || p.ZoneProductId.Equals(plantFilter.ZoneProductId))
                    .ToList();

                foreach (var product in products)
                {
                    var pickQty = _dbcontext.OrderPalletProducts
                        .Where(opp => opp.OrderId.Equals(order.OrderId)
                        && opp.ItemId.Equals(product.ItemId)
                        && opp.ZoneProductId.Equals(product.ZoneProductId))
                        .Sum(opp => opp.Qty);

                    order.PickingStatusDetails.Add(new DTOPickingStatusDetail()
                    {
                        ItemId = product.ItemId,
                        Description = product.Description,
                        Qty = product.Qty,
                        Weight = product.Weight,
                        Reference = product.Reference,
                        ZoneProduct = product.ZoneProduct.Name,
                        LastQty = product.LastQty,
                        PickQty = pickQty
                    });


                }

                return order;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
