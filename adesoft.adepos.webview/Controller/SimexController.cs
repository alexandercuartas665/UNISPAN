using adesoft.adepos.Extensions;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.DTO.Simex;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.Simex;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimexController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        public static List<DTOSimexSalesFilter> salesFilters = new List<DTOSimexSalesFilter>();
        public static List<DTOSimexInventSumFilter> inventSumFilters = new List<DTOSimexInventSumFilter>();
        public static List<DTOSimexCarteraFilter> carteraFilters = new List<DTOSimexCarteraFilter>();
        public static List<DTOSimexSalesOrderFilter> salesOrderFilters = new List<DTOSimexSalesOrderFilter>();
        public long index = 1;

        public SimexController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (connectionDB == null)
                connectionDB = SecurityService.GetConnectionDefault();
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public void AddSalesFilter(DTOSimexSalesFilter salesFilter)
        {
            salesFilters.Add(salesFilter);
        }        

        public void AddInventSumFilter(DTOSimexInventSumFilter inventSumFilter)
        {
            inventSumFilters.Add(inventSumFilter);
        }

        public void AddCarteraFilter(DTOSimexCarteraFilter carteraFilter)
        {
            carteraFilters.Add(carteraFilter);
        }

        public void AddSalesOrderFilter(DTOSimexSalesOrderFilter salesOrderFilter)
        {
            salesOrderFilters.Add(salesOrderFilter);
        }

        private string ToString(IExcelDataReader reader, int column)
        {
            var value = reader.GetValue(column);
            if (value is null)
                return "";

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Empty:
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.Boolean:
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.SByte:
                    break;
                case TypeCode.Byte:
                    break;
                case TypeCode.Int16:
                    break;
                case TypeCode.UInt16:
                    break;
                case TypeCode.Int32:
                    break;
                case TypeCode.UInt32:
                    break;
                case TypeCode.Int64:
                    break;
                case TypeCode.UInt64:
                    break;
                case TypeCode.Single:
                    break;
                case TypeCode.Double:
                    return reader.GetDouble(column).ToString();
                case TypeCode.Decimal:
                    return reader.GetDecimal(column).ToString();
                case TypeCode.DateTime:
                    break;
                case TypeCode.String:
                    return reader.GetString(column);
                default:
                    break;
            }

            return "";
        }

        public Sales Create(Sales sales, int idx)
        {
            Sales find = _dbcontext.SimexSales
                .Where(x => x.DocumentNum == sales.DocumentNum                                 
                && x.Element == sales.Element)
                .FirstOrDefault();
            if (find == null)
            {
                try
                {
                    sales.Modified = true;

                    _dbcontext.SimexSales.Add(sales);
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                if(find.Modified)
                {
                    find.Qty += sales.Qty;
                    find.CostTotalAmount += sales.CostTotalAmount;
                    find.SalesTotalAmount += sales.SalesTotalAmount;
                }
                else
                {                    
                    find.Modified = true;
                    find.Qty = sales.Qty;
                    find.CostTotalAmount = sales.CostTotalAmount;
                    find.SalesTotalAmount = sales.SalesTotalAmount;
                }

                try
                {
                    find.Operation1 = sales.Operation1;
                    find.Operation2 = sales.Operation2;
                    find.Operation3 = sales.Operation3;
                    find.Operation4 = sales.Operation4;
                    find.Operation5 = sales.Operation5;
                    find.Operation6 = sales.Operation6;
                    find.CustVendName = sales.CustVendName;
                    find.CustVendClasification = sales.CustVendClasification;
                    find.SalesPerson = sales.SalesPerson;
                    find.SalesPersonClasification = sales.SalesPersonClasification;

                    _dbcontext.SimexSales.Update(find);
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return sales;
        }

        public SalesOrder CreateOrUpdate(SalesOrder salesOrder)
        {
            try
            {
                SalesOrder find = _dbcontext.SimexSalesOrder
                .Where(x => x.SalesId == salesOrder.SalesId
                && x.ItemId == salesOrder.ItemId)
                .FirstOrDefault();

                if (find == null)
                {
                    try
                    {
                        salesOrder.CountryRegionId = "COLOMBIA";
                        salesOrder.Modified = true;

                        _dbcontext.SimexSalesOrder.Add(salesOrder);
                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    if (find.Modified)
                    {
                        find.QtyOrdered += salesOrder.QtyOrdered;
                        find.QtyPendingReceived += salesOrder.QtyPendingReceived;
                        find.QtyPendingInvoiced += salesOrder.QtyPendingInvoiced;
                    }
                    else
                    {
                        find.Modified = true;
                        find.QtyOrdered = salesOrder.QtyOrdered;                        
                        find.QtyPendingReceived = salesOrder.QtyPendingReceived;                        
                        find.QtyPendingInvoiced = salesOrder.QtyPendingInvoiced;
                    }

                    find.DocumentDate = salesOrder.DocumentDate;
                    find.Operation1 = salesOrder.Operation1;
                    find.Operation3 = salesOrder.Operation3;
                    find.Customer = salesOrder.Customer;
                    find.Operation4 = salesOrder.Operation4;
                    find.Operation2 = salesOrder.Operation2;
                    find.Warehouse = salesOrder.Warehouse;
                    find.ItemName = salesOrder.ItemName;

                    try
                    {
                        _dbcontext.SimexSalesOrder.Update(find);
                        _dbcontext.SaveChanges();
                        _dbcontext.DetachAll();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }

                return salesOrder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Cartera Create(Cartera cartera)
        {
            try
            {                
                cartera = _dbcontext.SimexCartera.Add(cartera).Entity;                

                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();

                return cartera;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public InventSum Create(InventSum _inventSum)
        {
            try
            {
                var inventSum = _dbcontext.SimexInventSum.Where(i => i.ItemId == _inventSum.ItemId && i.InventLocationId == _inventSum.InventLocationId).FirstOrDefault();

                if (inventSum is null)
                {
                    inventSum = _dbcontext.SimexInventSum.Add(_inventSum).Entity;                                       
                }
                else
                {
                    inventSum.ItemName = _inventSum.ItemName;
                    inventSum.UnitId = _inventSum.UnitId;
                    inventSum.QtyOnHand = _inventSum.QtyOnHand;

                    _dbcontext.SimexInventSum.Update(inventSum);
                }

                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();

                return inventSum;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UploadSales(Parameter parameter)
        {
            int column = 0;
            int idx = 0;

            List<Sales> salesList;

            if (!string.IsNullOrEmpty(parameter.NameFile) && parameter.NameFile.Contains("remo"))
            {
                string remo = parameter.NameFile.Split("remo")[1].Replace(".xlsx", "").Replace(".xls", "");
                var range = remo.Split('_');
                DateTime fromDate = DateTime.ParseExact(range[1], "ddMMyyyy", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(range[2], "ddMMyyyy", CultureInfo.InvariantCulture);

                salesList = _dbcontext.SimexSales.Where(s => s.DocumentDate >= fromDate && s.DocumentDate <= toDate).ToList();
                if(!(salesList is null) && salesList.Count() != 0)
                {
                    _dbcontext.SimexSales.RemoveRange(salesList);
                    _dbcontext.SaveChanges();
                    _dbcontext.DetachAll();
                }

                column = 0;
            }

            salesList = _dbcontext.SimexSales
                            .Where(s => s.Modified)
                            .ToList();

            foreach (var s in salesList)
            {
                s.Modified = false;
            }

            _dbcontext.UpdateRange(salesList);
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();

            try
            {
                bool readHeader = false;

                Stream stream = new MemoryStream(parameter.FileBuffer);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read()) //Each ROW
                        {
                            if (!readHeader)
                            {
                                readHeader = true;
                                continue;
                            }

                            Sales sales = new Sales();
                            idx++;

                            for (column = 0; column <= 21; column++)
                            {
                                switch (column)
                                {
                                    case 0:
                                        var value = reader.GetValue(column);
                                        if (value is null)
                                        {
                                            if (idx != 0)
                                            {
                                                this._dbcontext.SimexLastUpdateModuleLog.Add(new LastUpdateModule()
                                                {
                                                    Module = "Sales",
                                                    LastUpdateModule_At = DateTime.Now
                                                });
                                                _dbcontext.SaveChanges();
                                                _dbcontext.DetachAll();

                                                salesList = _dbcontext.SimexSales
                                                .Where(s => s.Modified)
                                                .ToList();

                                                foreach (var s in salesList)
                                                {
                                                    s.Modified = false;
                                                }

                                                _dbcontext.UpdateRange(salesList);
                                                _dbcontext.SaveChanges();
                                                _dbcontext.DetachAll();
                                            }

                                            return true;
                                        }

                                        sales.DocumentDate = reader.GetDateTime(column);
                                        break;

                                    case 1:
                                        sales.DocumentNum = reader.GetString(column);
                                        break;

                                    case 2:
                                        sales.Operation2 = reader.GetString(column);
                                        break;

                                    case 3:
                                        sales.Operation6 = this.ToString(reader, column);
                                        break;

                                    case 4:
                                        sales.Operation4 = reader.GetString(column);
                                        if (sales.Operation4.Equals("OCCIDENTE") || sales.Operation4.Equals("SUR"))
                                        {
                                            sales.Operation4 = "SUR OCCIDENTE";
                                        }

                                        break;

                                    case 5:
                                        sales.Operation1 = this.ToString(reader, column);
                                        break;

                                    case 6:
                                        sales.Operation6 = reader.GetString(column);
                                        break;

                                    case 7:
                                        sales.Operation3 = reader.GetString(column);
                                        break;

                                    case 8:
                                        sales.MovementType = int.Parse(this.ToString(reader, column));
                                        break;

                                    case 9:
                                        sales.CustVendName = reader.GetString(column);
                                        break;

                                    case 10:
                                        sales.CustVendClasification = reader.GetString(column);
                                        break;

                                    case 11:
                                        sales.SalesPerson = reader.GetString(column);
                                        break;

                                    case 12:
                                        sales.SalesPersonClasification = reader.GetString(column);
                                        break;

                                    case 13:
                                        sales.Warehouse = int.Parse(this.ToString(reader, column));
                                        break;

                                    case 14:
                                        sales.Element = reader.GetString(column);
                                        break;

                                    case 15:
                                        sales.ElementName = reader.GetString(column);
                                        break;

                                    case 16:
                                        sales.ElementData2 = reader.GetString(column);
                                        break;

                                    case 17:
                                        sales.Qty = long.Parse(this.ToString(reader, column)) * -1;
                                        break;

                                    case 18:
                                        sales.CostUnit = this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                        break;

                                    case 19:
                                        sales.CostTotalAmount = this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                        break;

                                    case 20:
                                        sales.PriceUnit = this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                        break;

                                    case 21:
                                        sales.SalesTotalAmount = this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                        break;
                                }

                            }

                            if (!string.IsNullOrEmpty(sales.DocumentNum) && !string.IsNullOrEmpty(sales.Element))
                            {
                                sales.CountryRegionId = "COLOMBIA";
                                sales.Idx = idx;
                                this.Create(sales, idx);                                
                            }

                        }
                    } while (reader.NextResult()); //Move to NEXT SHEET               
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }  
        }

        public bool UploadCartera(Parameter parameter)
        {
            bool readHeader = false;

            _dbcontext.Database.ExecuteSqlCommand("TRUNCATE TABLE [dbo].[SimexCartera]");

            Stream stream = new MemoryStream(parameter.FileBuffer);
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                do
                {
                    int idx = 1;
                    while (reader.Read()) //Each ROW
                    {
                        if (!readHeader)
                        {
                            readHeader = true;
                            continue;
                        }

                        Cartera cartera = new Cartera();

                        for (int column = 0; column <= 11; column++)
                        {
                            switch (column)
                            {
                                case 0:
                                    var value = reader.GetValue(column);
                                    if (value is null)
                                    {
                                        this._dbcontext.SimexLastUpdateModuleLog.Add(new LastUpdateModule()
                                        {
                                            Module = "Cartera",
                                            LastUpdateModule_At = DateTime.Now
                                        });
                                        this._dbcontext.SaveChanges();

                                        return true;
                                    }

                                    cartera.LedgerAccount = reader.GetString(column);
                                    break;

                                case 1:
                                    cartera.VendorName = string.IsNullOrEmpty(reader.GetString(column)) ? "SIN VENDEDOR" : reader.GetString(column);
                                    break;

                                case 2:
                                    cartera.ThirdAccount = reader.GetString(column);
                                    break;

                                case 3:
                                    cartera.ThirdName = reader.GetString(column);
                                    break;

                                case 4:
                                    cartera.Operation2 = string.IsNullOrEmpty(reader.GetString(column)) ? "SIN OBRA" : reader.GetString(column);

                                    break;

                                case 5:
                                    cartera.Operation4 = string.IsNullOrEmpty(reader.GetString(column)) ? "SIN ZONA" : reader.GetString(column);
                                    cartera.Operation4 = cartera.Operation4.Contains("SUR") ? "SUR OCCIDENTE" : cartera.Operation4;
                                    break;

                                case 6:
                                    cartera.Operation3 = string.IsNullOrEmpty(reader.GetString(column)) ? "SIN CIUDAD" : reader.GetString(column);
                                    break;

                                case 7:
                                    cartera.DocumentDate = reader.GetDateTime(column);
                                    break;

                                case 8:
                                    cartera.PaymentDateEstimated = reader.GetDateTime(column);
                                    break;

                                case 9:
                                    cartera.Reference = reader.GetString(column);
                                    break;

                                case 10:
                                    cartera.AmountBalance = this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                    break;

                                case 11:
                                    cartera.ExpirationDays = this.ToString(reader, column) == "" ? 0 : int.Parse(this.ToString(reader, column));
                                    break;
                            }

                        }

                        if (!string.IsNullOrEmpty(cartera.LedgerAccount) && !string.IsNullOrEmpty(cartera.Reference))                        
                            this.Create(cartera);                        

                    }
                } while (reader.NextResult()); //Move to NEXT SHEET
            }

            return true;
        }

        public bool UploadInventSum(Parameter parameter)
        {
            bool readHeader = false;

            Stream stream = new MemoryStream(parameter.FileBuffer);
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                do
                {
                    int idx = 1;
                    while (reader.Read()) //Each ROW
                    {
                        if (!readHeader)
                        {
                            readHeader = true;
                            continue;
                        }

                        InventSum inventSum = new InventSum();

                        for (int column = 0; column <= 4; column++)
                        {
                            switch (column)
                            {
                                case 0:
                                {
                                    var value = reader.GetValue(column);
                                    if (value is null)
                                    {
                                        this._dbcontext.SimexLastUpdateModuleLog.Add(new LastUpdateModule()
                                        {
                                            Module = "InventSum",
                                            LastUpdateModule_At = DateTime.Now
                                        });
                                        this._dbcontext.SaveChanges();
                                    
                                        return true;
                                    }

                                    inventSum.InventLocationId = this.ToString(reader, column);
                                    break;
                                }

                                case 1:
                                    inventSum.ItemId = reader.GetString(column);
                                    break;

                                case 2:
                                    inventSum.UnitId = reader.GetString(column);
                                    break;

                                case 3:
                                    inventSum.ItemName = reader.GetString(column);
                                    break;

                                case 4:
                                {
                                    /*string value = this.ToString(reader, column);
                                    value = string.IsNullOrEmpty(value) ? "0" : value;
                                    decimal qtyOnHand = decimal.Parse(value, new NumberFormatInfo() { NumberDecimalSeparator = ",", NumberGroupSeparator = "." });*/
                                    decimal qtyOnHand = this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                    inventSum.QtyOnHand = qtyOnHand <= 0 ? 0 : qtyOnHand;
                                    break;
                                }
                                    
                            }

                        }

                        if (!string.IsNullOrEmpty(inventSum.ItemId) )
                            this.Create(inventSum);

                    }
                } while (reader.NextResult()); //Move to NEXT SHEET
            }

            return true;
        }

        public bool UploadSalesOrder(Parameter parameter)
        {
            bool readHeader = false;

            var salesOrders = _dbcontext.SimexSalesOrder
                                            .Where(s => s.Modified)
                                            .ToList();

            foreach (var s in salesOrders)
            {
                s.Modified = false;
            }

            _dbcontext.UpdateRange(salesOrders);
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();

            Stream stream = new MemoryStream(parameter.FileBuffer);
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                int idx = 1;

                do
                {
                    while (reader.Read()) //Each ROW
                    {
                        if (!readHeader)
                        {
                            readHeader = true;
                            continue;
                        }

                        SalesOrder salesOrder = new SalesOrder();

                        for (int column = 0; column <= 11; column++)
                        {
                            switch (column)
                            {
                                case 0:
                                    var value = reader.GetValue(column);
                                    if (value is null)
                                    {
                                        if (idx != 0)
                                        {
                                            this._dbcontext.SimexLastUpdateModuleLog.Add(new LastUpdateModule()
                                            {
                                                Module = "SalesOrder",
                                                LastUpdateModule_At = DateTime.Now
                                            });
                                            this._dbcontext.SaveChanges();
                                        }

                                        // delete data
                                        salesOrders = _dbcontext.SimexSalesOrder
                                            .Where(s => s.Modified == false)
                                            .ToList();
                                        foreach (var so in salesOrders)
                                        {
                                            _dbcontext.SimexSalesOrder.Remove(so);
                                            _dbcontext.SaveChanges();
                                            _dbcontext.DetachAll();
                                        }

                                        salesOrders = _dbcontext.SimexSalesOrder
                                            .Where(s => s.Modified)
                                            .ToList();

                                        foreach (var s in salesOrders)
                                        {
                                            s.Modified = false;                                            
                                        }

                                        _dbcontext.UpdateRange(salesOrders);
                                        _dbcontext.SaveChanges();
                                        _dbcontext.DetachAll();

                                        return true;
                                    }

                                    salesOrder.SalesId = reader.GetString(column);
                                    break;

                                case 1:
                                    salesOrder.DocumentDate = reader.GetDateTime(column);
                                    break;

                                case 2:
                                    salesOrder.Operation1 = reader.GetString(column);
                                    break;

                                case 3:
                                    salesOrder.Operation3 = this.ToString(reader, column);
                                    break;

                                case 4:
                                    salesOrder.Customer = reader.GetString(column);                                    
                                    break;

                                case 5:
                                    salesOrder.Operation4 = reader.GetString(column);
                                    break;

                                case 6:
                                    salesOrder.Operation2 = reader.GetString(column);
                                    break;

                                case 7:
                                    salesOrder.Warehouse = int.Parse(this.ToString(reader, column));
                                    break;

                                case 8:
                                    salesOrder.ItemId = reader.GetString(column);
                                    break;

                                case 9:
                                    salesOrder.ItemName = reader.GetString(column);
                                    break;                                

                                case 10:
                                    salesOrder.QtyPendingReceived = long.Parse(this.ToString(reader, column));
                                    break;

                                case 11:
                                    salesOrder.QtyPendingInvoiced = long.Parse(this.ToString(reader, column));
                                    break;
                            }

                        }

                        if (!string.IsNullOrEmpty(salesOrder.SalesId))
                        {
                            try
                            {
                                this.CreateOrUpdate(salesOrder);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }

                    }
                } while (reader.NextResult()); //Move to NEXT SHEET               
            }            

            return true;
        }

        public bool UploadSalesOrderPrice(Parameter parameter)
        {
            bool readHeader = false;

            Stream stream = new MemoryStream(parameter.FileBuffer);
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                int idx = 1;

                do
                {
                    while (reader.Read()) //Each ROW
                    {
                        if (!readHeader)
                        {
                            readHeader = true;
                            continue;
                        }

                        SalesOrder salesOrderPrice = new SalesOrder();

                        for (int column = 0; column <= 11; column++)
                        {
                            switch (column)
                            {
                                case 1:
                                    var value = reader.GetValue(column);
                                    if (value is null)
                                    {
                                        if (idx != 0)
                                        {
                                            this._dbcontext.SimexLastUpdateModuleLog.Add(new LastUpdateModule()
                                            {
                                                Module = "SalesOrder",
                                                LastUpdateModule_At = DateTime.Now
                                            });
                                            this._dbcontext.SaveChanges();
                                        }

                                        var salesOrders = _dbcontext.SimexSalesOrder
                                            .Where(s => s.Modified)
                                            .ToList();

                                        foreach (var s in salesOrders)
                                        {
                                            s.Modified = false;
                                        }

                                        _dbcontext.UpdateRange(salesOrders);
                                        _dbcontext.SaveChanges();
                                        _dbcontext.DetachAll();

                                        return true;
                                    }

                                    salesOrderPrice.SalesId = reader.GetString(column);
                                    break;

                                case 6:
                                    salesOrderPrice.ItemId = reader.GetString(column);
                                    break;

                                case 8:
                                    salesOrderPrice.QtyOrdered = long.Parse(this.ToString(reader, column));
                                    break;

                                case 9:
                                    salesOrderPrice.TotalAmountLine= this.ToString(reader, column) == "" ? 0 : decimal.Parse(this.ToString(reader, column));
                                    break;
                            }

                        }

                        if (!string.IsNullOrEmpty(salesOrderPrice.SalesId))
                        {
                            try
                            {
                                var salesOrder = _dbcontext.SimexSalesOrder
                                            .Where(s => s.SalesId == salesOrderPrice.SalesId && s.ItemId == salesOrderPrice.ItemId)
                                            .FirstOrDefault();
                                if(!(salesOrder is null) && !salesOrder.Modified)
                                {
                                    salesOrder.Modified = true;
                                    salesOrder.QtyOrdered = salesOrderPrice.QtyOrdered < 0 ? salesOrderPrice.QtyOrdered * -1 : salesOrderPrice.QtyOrdered;
                                    salesOrder.TotalAmountLine = salesOrderPrice.TotalAmountLine;
                                    salesOrder.UnitPrice = ((salesOrder.QtyOrdered != 0) && (salesOrder.TotalAmountLine != 0)) ? salesOrder.TotalAmountLine / salesOrder.QtyOrdered : 0;
                                    salesOrder.AmountPendingRecived = salesOrder.UnitPrice * salesOrder.QtyPendingReceived;
                                    salesOrder.AmountPendingInvoiced = salesOrder.UnitPrice * salesOrder.QtyPendingInvoiced;

                                    _dbcontext.SimexSalesOrder.Update(salesOrder);
                                    _dbcontext.SaveChanges();
                                    _dbcontext.DetachAll();
                                }                                
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }

                    }
                } while (reader.NextResult()); //Move to NEXT SHEET               
            }

            return true;
        }

        public List<DTOYear> GetYears(string reportName)
        {
            List<DTOYear> list = new List<DTOYear>();
            switch (reportName)
            {
                case "Sales":
                    list = _dbcontext.SimexSales
                        .Select(s => new DTOYear
                        {
                            IdYear = (long)s.DocumentDate.Year,
                            Name = s.DocumentDate.Year.ToString()
                        })
                        .Distinct()
                        .ToList();
                    break;

                case "SalesOrder":
                    list = _dbcontext.SimexSalesOrder
                        .Select(s => new DTOYear
                        {
                            IdYear = (long)s.DocumentDate.Year,
                            Name = s.DocumentDate.Year.ToString()
                        })
                        .Distinct()
                        .ToList();
                    break;

                default:
                    break;
            }
            
            return list;
        }

        public List<DTOZone> GetZones()
        {
            var list = _dbcontext.SimexSales
                .Select(s => new DTOZone
                {
                    ZoneId = s.Operation4,
                    Description = s.Operation4
                })
                .Distinct()
                .ToList();

            return list;
        }

        public Expression<Func<TItem, object>> GroupByExpression<TItem>(string[] propertyNames)
        {
            var properties = propertyNames.Select(name => typeof(TItem).GetProperty(name)).ToArray();
            var propertyTypes = properties.Select(p => p.PropertyType).ToArray();
            var tupleTypeDefinition = typeof(Tuple).Assembly.GetType("System.Tuple`" + properties.Length);
            var tupleType = tupleTypeDefinition.MakeGenericType(propertyTypes);
            var constructor = tupleType.GetConstructor(propertyTypes);
            var param = Expression.Parameter(typeof(TItem), "item");
            var body = Expression.New(constructor, properties.Select(p => Expression.Property(param, p)));
            var expr = Expression.Lambda<Func<TItem, object>>(body, param);
            return expr;
        }

        public decimal PriceUnitPromedio(decimal salesAmountTotal, long qty)
        {
            if ((salesAmountTotal != 0) && (qty != 0))
                return salesAmountTotal / qty;

            return 0;
        }

        private DTOSimexSalesReport NewSelect(IEnumerable<string> groupBy, IGrouping<object, Sales> s, string rowTotalHidden)
        {
            DTOSimexSalesReport simexSalesReport = new DTOSimexSalesReport();

            int column = 1;

            foreach (var gb in groupBy)
            {
                var value = "";
                var label = "";

                switch (gb)
                {
                    case "CountryRegionId":
                        value = s.Max(s => s.CountryRegionId);
                        label = "PAIS";
                        break;

                    case "Operation4":
                        value = s.Max(s => s.Operation4);
                        label = "ZONA";
                        break;

                    case "CustVendName":
                        value = s.Max(s => s.CustVendName);
                        label = "CLIENTE";
                        break;

                    case "SalesPerson":
                        value = s.Max(s => s.SalesPerson);
                        label = "COMERCIAL";
                        break;

                    case "ElementName":
                        value = s.Max(s => s.ElementName);
                        label = "PRODUCTO";
                        break;

                    case "Operation2":
                        value = s.Max(s => s.Operation2);
                        label = "OBRA";
                        break;

                    default:
                        break;
                }

                switch (column)
                {
                    case 1:
                        simexSalesReport.GroupBy1 = value;
                        simexSalesReport.GroupBy1Label = label;
                        break;

                    case 2:
                        simexSalesReport.GroupBy2 = value;
                        simexSalesReport.GroupBy2Label = label;
                        break;

                    case 3:
                        simexSalesReport.GroupBy3 = value;
                        simexSalesReport.GroupBy3Label = label;
                        break;

                    case 4:
                        simexSalesReport.GroupBy4 = value;
                        simexSalesReport.GroupBy4Label = label;
                        break;

                    case 5:
                        simexSalesReport.GroupBy5 = value;
                        simexSalesReport.GroupBy5Label = label;
                        break;

                    case 6:
                        simexSalesReport.GroupBy6 = value;
                        simexSalesReport.GroupBy6Label = label;
                        break;

                    default:
                        break;
                }

                column++;
            }

            simexSalesReport.Index = this.index;
            simexSalesReport.QtyAcumulate = s.Sum(s => s.Qty);
            simexSalesReport.SalesTotalAmount = s.Sum(s => s.SalesTotalAmount);
            simexSalesReport.PriceUnitPromedio = this.PriceUnitPromedio(s.Sum(s => s.SalesTotalAmount), s.Sum(s => s.Qty));
            simexSalesReport.RowTotalHidden = rowTotalHidden;

            this.index++;

            return simexSalesReport;
        }

        private DTOSimexInventSumReport NewSelect(IEnumerable<string> groupBy, IGrouping<object, InventSum> i)
        {
            DTOSimexInventSumReport simexInventSumReport = new DTOSimexInventSumReport();

            int column = 1;

            foreach (var gb in groupBy)
            {
                var value = "";
                var label = "";

                switch (gb)
                {
                    case "InventLocationId":
                        value = i.Max(i => i.InventLocationId);
                        label = "BODEGA";
                        break;

                    case "ItemId":
                        value = i.Max(i => i.ItemId);
                        label = "PRODUCTO";
                        break;

                    case "UnitId":
                        simexInventSumReport.UnitId = i.Max(i => i.UnitId);
                        break;

                    case "ItemName":
                        simexInventSumReport.ItemName = i.Max(i => i.ItemName);
                        break;

                    default:
                        break;
                }

                switch (column)
                {
                    case 1:
                        simexInventSumReport.GroupBy1 = value;
                        simexInventSumReport.GroupBy1Label = label;
                        break;

                    case 2:
                        simexInventSumReport.GroupBy2 = value;
                        simexInventSumReport.GroupBy2Label = label;
                        break;

                    default:
                        break;
                }

                column++;
            }

            simexInventSumReport.QtyOnHand = i.Sum(i => i.QtyOnHand);

            if (simexInventSumReport.GroupBy1Label.Equals("PRODUCTO"))
            {
                var qtyMinimum = this.GetQtyMinimum(simexInventSumReport.GroupBy1);
                if(!(qtyMinimum is null ))
                {
                    simexInventSumReport.QtyMinimum = qtyMinimum.Qty;
                }
            }
            else if (simexInventSumReport.GroupBy2Label.Equals("PRODUCTO"))
            {
                var qtyMinimum = this.GetQtyMinimum(simexInventSumReport.GroupBy2);
                if (!(qtyMinimum is null))
                {
                    simexInventSumReport.QtyMinimum = qtyMinimum.Qty;
                }
            }

            return simexInventSumReport;
        }

        private DTOSimexCarteraReport NewSelect(IEnumerable<string> groupBy, IGrouping<object, Cartera> c, DTOSimexCarteraFilter filterCartera)
        {
            var simexCarteraReport = new DTOSimexCarteraReport();            

            int column = 1;            

            foreach (var gb in groupBy)
            {
                var value = "";
                var label = "";

                switch (gb)
                {
                    case "VendorName":
                        value = string.IsNullOrEmpty(c.Max(c => c.VendorName)) ? "SIN COMERCIAL" : c.Max(c => c.VendorName);
                        label = "COMERCIAL";
                        break;

                    case "ThirdAccount":
                        value = c.Max(c => c.ThirdAccount);
                        label = "IDENTIFICACION";
                        break;

                    case "ThirdName":
                        value = c.Max(c => c.ThirdName);
                        label = "CLIENTE";
                        break;

                    case "Operation2":
                        value = string.IsNullOrEmpty(c.Max(c => c.Operation2)) ? "SIN OBRA" : c.Max(c => c.Operation2);
                        label = "OBRA";
                        break;

                    case "Operation4":
                        value = string.IsNullOrEmpty(c.Max(c => c.Operation4)) ? "SIN ZONA" : c.Max(c => c.Operation4);
                        label = "ZONA";
                        break;

                    case "Range":
                        value = c.Max(c => c.Range);
                        label = "RANGO";
                        simexCarteraReport.Range = value;
                        break;

                    default:
                        break;
                }

                switch (column)
                {
                    case 1:
                        simexCarteraReport.GroupBy1 = value;
                        simexCarteraReport.GroupBy1Label = label;
                        break;

                    case 2:
                        simexCarteraReport.GroupBy2 = value;
                        simexCarteraReport.GroupBy2Label = label;
                        break;

                    case 3:
                        simexCarteraReport.GroupBy3 = value;
                        simexCarteraReport.GroupBy3Label = label;
                        break;

                    case 4:
                        simexCarteraReport.GroupBy4 = value;
                        simexCarteraReport.GroupBy4Label = label;
                        break;

                    case 5:
                        simexCarteraReport.GroupBy5 = value;
                        simexCarteraReport.GroupBy5Label = label;
                        break;

                    default:
                        break;
                }

                column++;
            }
            
            if(filterCartera.TypeReportId == 1)
            {
                simexCarteraReport.DocumentDate = c.Max(c => c.DocumentDate);
                simexCarteraReport.PaymentDateEstimated = c.Max(c => c.PaymentDateEstimated);
                simexCarteraReport.Reference = c.Max(c => c.Reference);
                simexCarteraReport.AmountBalance = c.Sum(c => c.AmountBalance);
                simexCarteraReport.ExpirationDays = (simexCarteraReport.PaymentDateEstimated - filterCartera.ToDate).Days;
            }
            else if(filterCartera.TypeReportId == 2)            
            {                
                simexCarteraReport.AmountBalance = c.Sum(c => c.AmountBalance);
                simexCarteraReport.Days1To30 = c.Sum(c => c.Days1To30);
                simexCarteraReport.Days31To60 = c.Sum(c => c.Days31To60);
                simexCarteraReport.Days61To90 = c.Sum(c => c.Days61To90);
                simexCarteraReport.More90 = c.Sum(c => c.More90);
                simexCarteraReport.Current = c.Sum(c => c.Current);                
            }            
            else
            {
                simexCarteraReport.PaymentDateEstimated = c.Max(c => c.PaymentDateEstimated);
                simexCarteraReport.Reference = c.Max(c => c.Reference);
                simexCarteraReport.AmountBalance = c.Sum(c => c.AmountBalance);
                simexCarteraReport.ExpirationDays = (simexCarteraReport.PaymentDateEstimated - filterCartera.ToDate).Days;
            }

            return simexCarteraReport;
        }

        private DTOSimexSalesOrderReport NewSelect(IEnumerable<string> groupBy, IGrouping<object, SalesOrder> s)
        {
            DTOSimexSalesOrderReport simexSalesReport = new DTOSimexSalesOrderReport();

            int column = 1;

            foreach (var gb in groupBy)
            {
                var value = "";
                var label = "";

                switch (gb)
                {
                    case "CountryRegionId":
                        value = s.Max(s => s.CountryRegionId);
                        label = "PAIS";
                        break;

                    case "Operation4":
                        value = s.Max(s => s.Operation4);
                        label = "ZONA";
                        break;

                    case "Customer":
                        value = s.Max(s => s.Customer);
                        label = "CLIENTE";
                        break;

                    case "ItemName":
                        value = s.Max(s => s.ItemName);
                        label = "PRODUCTO";
                        break;

                    case "Operation2":
                        value = s.Max(s => s.Operation2);
                        label = "OBRA";
                        break;

                    default:
                        break;
                }

                switch (column)
                {
                    case 1:
                        simexSalesReport.GroupBy1 = value;
                        simexSalesReport.GroupBy1Label = label;
                        break;

                    case 2:
                        simexSalesReport.GroupBy2 = value;
                        simexSalesReport.GroupBy2Label = label;
                        break;

                    case 3:
                        simexSalesReport.GroupBy3 = value;
                        simexSalesReport.GroupBy3Label = label;
                        break;

                    case 4:
                        simexSalesReport.GroupBy4 = value;
                        simexSalesReport.GroupBy4Label = label;
                        break;

                    case 5:
                        simexSalesReport.GroupBy5 = value;
                        simexSalesReport.GroupBy5Label = label;
                        break;

                    case 6:
                        simexSalesReport.GroupBy6 = value;
                        simexSalesReport.GroupBy6Label = label;
                        break;

                    default:
                        break;
                }

                column++;
            }
            
            simexSalesReport.QtyOrdered = s.Sum(s => s.QtyOrdered);
            simexSalesReport.TotalAmountLine = s.Sum(s => s.TotalAmountLine);
            simexSalesReport.UnitPrice = s.Max(s => s.UnitPrice);
            simexSalesReport.QtyPendingReceived = s.Sum(s => s.QtyPendingReceived);
            simexSalesReport.QtyPendingInvoiced = s.Sum(s => s.QtyPendingInvoiced);
            simexSalesReport.AmountPendingRecived = s.Sum(s => s.AmountPendingRecived);
            simexSalesReport.AmountPendingInvoiced = s.Sum(s => s.AmountPendingInvoiced);

            return simexSalesReport;
        }

        [HttpGet("GetSales")]
        public IActionResult GetSales(string guidfilter)
        {
            DTOSimexSalesFilter filterSales = salesFilters.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();

            string rowTotalHidden = "";
            foreach (var gb in filterSales.GroupBy)
            {
                rowTotalHidden = gb;
            } 
            
            var lambda = this.GroupByExpression<Sales>(filterSales.GroupBy.ToArray());

            var salesList = new List<Sales>();
            var filterList = new List<DTOSimexSalesReport>();

            switch (filterSales.TypeReportId)
            {
                case 1:
                    {
                        long yearIdi = filterSales.DateInit.GetValueOrDefault().Year;
                        long yearIde = filterSales.DateEnd.GetValueOrDefault().Year;
                        long monthIdi = filterSales.DateInit.GetValueOrDefault().Month;
                        long monthIde = filterSales.DateEnd.GetValueOrDefault().Month;

                        salesList = this._dbcontext.SimexSales
                            .Select(s => s)
                            .Where(s => s.DocumentDate >= filterSales.DateInit && s.DocumentDate <= filterSales.DateEnd)
                            .ToList();

                        var _filterList = salesList.AsQueryable()
                            .GroupBy(lambda.Compile())
                            .Select(s => this.NewSelect(filterSales.GroupBy, s, rowTotalHidden))
                            .ToList();

                        var zones = new List<DTOZone>();

                        foreach (var fl in _filterList)
                        {
                            if (filterSales.GroupBy.Count() == 1)
                            {
                                var filter = filterSales.GroupBy.FirstOrDefault();
                                
                                switch (filter)
                                {
                                    case "Operation4":
                                        {
                                            if(zones.Count() == 0)
                                                zones = this.GetZones();                                            

                                            var presupuestoFilter = new List<Presupuesto>();
                                            if((yearIdi == yearIde) && (monthIdi == monthIde))
                                            {
                                                presupuestoFilter = this._dbcontext.SimexPresupuesto
                                                .Select(s => s)
                                                .Where(s => s.YearId == yearIdi && s.MonthId == monthIdi && s.CategoryId == "PESOS" && s.ZoneId == fl.GroupBy1)
                                                .ToList();
                                            }
                                            else
                                            {
                                                presupuestoFilter = this._dbcontext.SimexPresupuesto
                                                .Select(s => s)
                                                .Where(s => ((s.YearId >= yearIdi && s.MonthId >= monthIdi) || (s.YearId <= yearIde && s.MonthId <= monthIde)) && s.CategoryId == "PESOS" && s.ZoneId == fl.GroupBy1)
                                                .ToList();
                                            }
                                            var presupuesto = presupuestoFilter.Sum(s => s.Value);
                                            if (presupuesto != 0)
                                            {
                                                fl.PresupuestoNext = presupuesto;
                                                fl.PercentComplianceNext = fl.SalesTotalAmount != 0 ? ((100 * fl.SalesTotalAmount) / presupuesto) / 100 : 0;
                                            }
                                        }
                                        break;

                                    case "CountryRegionId":
                                        {
                                            var presupuestoFilter = new List<Presupuesto>();
                                            if ((yearIdi == yearIde) && (monthIdi == monthIde))
                                            {
                                                presupuestoFilter = this._dbcontext.SimexPresupuesto
                                                    .Select(s => s)
                                                    .Where(s => s.YearId == yearIdi && s.MonthId == monthIdi && s.CategoryId == "PESOS")
                                                    .ToList();
                                            }
                                            else
                                            {
                                                presupuestoFilter = this._dbcontext.SimexPresupuesto
                                                    .Select(s => s)
                                                    .Where(s => ((s.YearId >= yearIdi && s.MonthId >= monthIdi) || (s.YearId <= yearIde && s.MonthId <= monthIde)) && s.CategoryId == "PESOS")
                                                    .ToList();
                                            }
                                            var presupuesto = presupuestoFilter.Sum(s => s.Value);
                                            if (presupuesto != 0)
                                            {
                                                fl.PresupuestoNext = presupuesto;
                                                fl.PercentComplianceNext = fl.SalesTotalAmount != 0 ? ((100 * fl.SalesTotalAmount) / presupuesto) / 100 : 0;
                                            }
                                        }
                                        break;

                                    default:
                                        break;
                                }
                            }

                            filterList.Add(fl);
                        }

                        if (zones.Count != 0)
                        {
                            foreach (var zone in zones)
                            {
                                var filter = filterList.Where(f => f.GroupBy1 == zone.ZoneId).FirstOrDefault();
                                if(filter is null)
                                {
                                    var presupuestoFilter = new List<Presupuesto>();
                                    if ((yearIdi == yearIde) && (monthIdi == monthIde))
                                    {
                                        presupuestoFilter = this._dbcontext.SimexPresupuesto
                                        .Select(s => s)
                                        .Where(s => s.YearId == yearIdi && s.MonthId == monthIdi && s.CategoryId == "PESOS" && s.ZoneId == zone.ZoneId)
                                        .ToList();
                                    }
                                    else
                                    {
                                        presupuestoFilter = this._dbcontext.SimexPresupuesto
                                        .Select(s => s)
                                        .Where(s => ((s.YearId >= yearIdi && s.MonthId >= monthIdi) || (s.YearId <= yearIde && s.MonthId <= monthIde)) && s.CategoryId == "PESOS" && s.ZoneId == zone.ZoneId)
                                        .ToList();
                                    }

                                    var presupuestoNext = presupuestoFilter.Sum(s => s.Value);     
                                    
                                    if(presupuestoNext != 0)
                                    {
                                        filterList.Add(new DTOSimexSalesReport()
                                        {
                                            GroupBy1 = zone.ZoneId,
                                            GroupBy1Label = "ZONA",
                                            QtyAcumulate = 0,
                                            SalesTotalAmount = 0,
                                            RowTotalHidden = "Operation4",
                                            PresupuestoNext = presupuestoNext,
                                            PercentComplianceNext = 0
                                        });
                                    }                                    
                                }
                            }
                        }
                    }
                    break;

                case 2:
                    {
                        foreach (var yearFilter in filterSales.MultipleValuesYear)
                        {
                            foreach (var month in filterSales.MultipleValuesMonth)
                            {
                                var initDate = new DateTime((int)yearFilter, (int)month, 1);
                                var endDate = initDate.AddMonths(1).AddDays(-1);
                                salesList = this._dbcontext.SimexSales
                                    .Select(s => s)
                                    .Where(s => s.DocumentDate >= initDate && s.DocumentDate <= endDate)
                                    .ToList();

                                var zones = new List<DTOZone>();

                                if (salesList.Count() != 0)
                                {
                                    var _filterList = salesList.AsQueryable()
                                    .GroupBy(lambda.Compile())
                                    .Select(s => this.NewSelect(filterSales.GroupBy, s, rowTotalHidden))
                                    .ToList();

                                    foreach (var fl in _filterList)
                                    {
                                        if (filterSales.GroupBy.Count() == 1)
                                        {
                                            var filter = filterSales.GroupBy.FirstOrDefault();
                                            switch (filter)
                                            {
                                                case "Operation4":
                                                    {
                                                        if (zones.Count() == 0)
                                                            zones = this.GetZones();

                                                        var presupuesto = this._dbcontext.SimexPresupuesto
                                                            .Select(s => s)
                                                            .Where(s => s.YearId == yearFilter && s.MonthId == month && s.CategoryId == "PESOS" && s.ZoneId == fl.GroupBy1)
                                                            .FirstOrDefault();
                                                        if (!(presupuesto is null))
                                                        {
                                                            fl.PresupuestoNext = presupuesto.Value;
                                                            fl.PercentComplianceNext = fl.SalesTotalAmount != 0 ? ((100 * fl.SalesTotalAmount) / presupuesto.Value) / 100 : 0;
                                                        }
                                                    }
                                                    break;

                                                case "CountryRegionId":
                                                    {
                                                        var presupuestoFilter = this._dbcontext.SimexPresupuesto
                                                            .Select(s => s)
                                                            .Where(s => s.YearId == yearFilter && s.MonthId == month && s.CategoryId == "PESOS")
                                                            .ToList();
                                                        var presupuesto = presupuestoFilter.Sum(s => s.Value);
                                                        if (presupuesto != 0)
                                                        {
                                                            fl.PresupuestoNext = presupuesto;
                                                            fl.PercentComplianceNext = fl.SalesTotalAmount != 0 ? ((100 * fl.SalesTotalAmount) / presupuesto) / 100 : 0;
                                                        }
                                                    }
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }

                                        fl.Year = yearFilter.ToString();
                                        fl.Month = this.GetMonthName(month);
                                        fl.Mth = (int)month;
                                        filterList.Add(fl);
                                    }
                                }

                                if (zones.Count != 0)
                                {
                                    foreach (var zone in zones)
                                    {
                                        var filter = filterList.Where(f => f.GroupBy1 == zone.ZoneId).FirstOrDefault();
                                        if (filter is null)
                                        {                                           
                                            var presupuesto = this._dbcontext.SimexPresupuesto
                                                            .Select(s => s)
                                                            .Where(s => s.YearId == yearFilter && s.MonthId == month && s.CategoryId == "PESOS" && s.ZoneId == zone.ZoneId)
                                                            .FirstOrDefault();
                                            decimal presupuestoNext = 0;
                                            if (!(presupuesto is null))                                            
                                                presupuestoNext = presupuesto.Value;

                                            if (presupuestoNext != 0)
                                            {
                                                filterList.Add(new DTOSimexSalesReport()
                                                {
                                                    GroupBy1 = zone.ZoneId,
                                                    GroupBy1Label = "ZONA",
                                                    QtyAcumulate = 0,
                                                    SalesTotalAmount = 0,
                                                    RowTotalHidden = "Operation4",
                                                    PresupuestoNext = presupuestoNext,
                                                    PercentComplianceNext = 0,
                                                    Year = yearFilter.ToString(),
                                                    Month = this.GetMonthName(month),
                                                    Mth = (int)month
                                                });
                                            }                                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                case 3:
                    {
                        foreach (var yearFilter in filterSales.MultipleValuesYear)
                        {
                            var initDate = new DateTime((int)yearFilter, 1, 1);
                            var endDate = new DateTime((int)yearFilter, 12, 31);
                            salesList = this._dbcontext.SimexSales
                                .Select(s => s)
                                .Where(s => s.DocumentDate >= initDate && s.DocumentDate <= endDate)
                                .ToList();

                            var zones = new List<DTOZone>();

                            if (salesList.Count() != 0)
                            {
                                var _filterList = salesList.AsQueryable()
                                .GroupBy(lambda.Compile())
                                .Select(s => this.NewSelect(filterSales.GroupBy, s, rowTotalHidden))
                                .ToList();

                                foreach (var fl in _filterList)
                                {
                                    if (filterSales.GroupBy.Count() == 1)
                                    {
                                        var filter = filterSales.GroupBy.FirstOrDefault();
                                        switch (filter)
                                        {
                                            case "Operation4":
                                                {
                                                    if (zones.Count() == 0)
                                                        zones = this.GetZones();

                                                    var presupuestoFilter = this._dbcontext.SimexPresupuesto
                                                        .Select(s => s)
                                                        .Where(s => s.YearId == yearFilter && s.CategoryId == "PESOS" && s.ZoneId == fl.GroupBy1)
                                                        .ToList();
                                                    var presupuesto = presupuestoFilter.Sum(s => s.Value);
                                                    if (presupuesto != 0)
                                                    {
                                                        fl.PresupuestoNext = presupuesto;
                                                        fl.PercentComplianceNext = fl.SalesTotalAmount != 0 ? ((100 * fl.SalesTotalAmount) / presupuesto) / 100 : 0;
                                                    }
                                                }
                                                break;

                                            case "CountryRegionId":
                                                {
                                                    var presupuestoFilter = this._dbcontext.SimexPresupuesto
                                                        .Select(s => s)
                                                        .Where(s => s.YearId == yearFilter && s.CategoryId == "PESOS")
                                                        .ToList();
                                                    var presupuesto = presupuestoFilter.Sum(s => s.Value);
                                                    if (presupuesto != 0)
                                                    {
                                                        fl.PresupuestoNext = presupuesto;
                                                        fl.PercentComplianceNext = fl.SalesTotalAmount != 0 ? ((100 * fl.SalesTotalAmount) / presupuesto) / 100 : 0;
                                                    }
                                                }
                                                break;

                                            default:
                                                break;
                                        }
                                    }

                                    fl.Year = yearFilter.ToString();
                                    filterList.Add(fl);
                                }
                            }

                            if (zones.Count != 0)
                            {
                                foreach (var zone in zones)
                                {
                                    var filter = filterList.Where(f => f.GroupBy1 == zone.ZoneId).FirstOrDefault();
                                    if (filter is null)
                                    {
                                        var presupuestoFilter = this._dbcontext.SimexPresupuesto
                                                        .Select(s => s)
                                                        .Where(s => s.YearId == yearFilter && s.CategoryId == "PESOS" && s.ZoneId == zone.ZoneId)
                                                        .ToList();
                                        var presupuestoNext = presupuestoFilter.Sum(s => s.Value);

                                        if (presupuestoNext != 0)
                                        {
                                            filterList.Add(new DTOSimexSalesReport()
                                            {
                                                GroupBy1 = zone.ZoneId,
                                                GroupBy1Label = "ZONA",
                                                QtyAcumulate = 0,
                                                SalesTotalAmount = 0,
                                                RowTotalHidden = "Operation4",
                                                PresupuestoNext = presupuestoNext,
                                                PercentComplianceNext = 0,
                                                Year = yearFilter.ToString()
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
            

            decimal amountTotal = 0;
            foreach (var filter in filterList)
            {
                amountTotal += filter.SalesTotalAmount;
            }

            foreach (var filter in filterList)
            {
                filter.ParticipationPercent = (filter.SalesTotalAmount * 100 / amountTotal) / 100;
            }

            return Ok(filterList);
        }

        [HttpGet("GetInventSum")]
        public IActionResult GetInventSum(string guidfilter)
        {
            DTOSimexInventSumFilter filterInventSum = inventSumFilters.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();

            var newGroupBy = new List<string>();
            var groupBy = filterInventSum.GroupBy.ToList();
            foreach (var gb in groupBy)
            {
                if(gb.Equals("ItemId"))
                {
                    newGroupBy.Add("UnitId");
                    newGroupBy.Add("ItemName");
                }
            }
            groupBy.AddRange(newGroupBy);

            var lambda = this.GroupByExpression<InventSum>(groupBy.ToArray());

            List<InventSum> inventSumList = null;
            if(string.IsNullOrEmpty(filterInventSum.SearchBy))
            {
                inventSumList = this._dbcontext.SimexInventSum.ToList();
            }
            else
            {
                if(filterInventSum.GroupBy.Count() == 2)
                {
                    inventSumList = this._dbcontext.SimexInventSum
                        .Where(i => i.InventLocationId.Contains(filterInventSum.SearchBy)
                        || i.ItemId.Contains(filterInventSum.SearchBy)
                        || i.ItemName.Contains(filterInventSum.SearchBy))
                        .ToList();
                }
                else
                {
                    switch(filterInventSum.GroupBy.ToList().FirstOrDefault())
                    {
                        case "InventLocationId":
                            inventSumList = this._dbcontext.SimexInventSum
                                .Where(i => i.InventLocationId.Contains(filterInventSum.SearchBy))
                                .ToList();
                            break;

                        case "ItemId":
                            inventSumList = this._dbcontext.SimexInventSum
                                .Where(i => i.ItemId.Contains(filterInventSum.SearchBy)
                                || i.ItemName.Contains(filterInventSum.SearchBy))
                            .ToList();

                            break;
                    }
                }
            }

            var _filterList = inventSumList.AsQueryable()
                                .GroupBy(lambda.Compile())
                                .Select(i => this.NewSelect(groupBy.ToArray(), i))
                                .ToList();

            return Ok(_filterList);
        }

        [HttpGet("GetCartera")]
        public IActionResult GetCartera(string guidfilter)
        {

            DTOSimexCarteraFilter filterCartera = carteraFilters.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();

            List<string> groupBy = null;

            switch (filterCartera.TypeReportId)
            {
                case 1:
                    groupBy = new List<string>()
                    {
                        "VendorName",
                        "ThirdAccount",
                        "ThirdName",
                        "Operation2",
                        "Operation4"
                    };
                    break;

                case 2:
                    groupBy = filterCartera.GroupBy.ToList();
                    if(groupBy.Count > 1)
                    {
                        groupBy.Remove("Range");
                    }
                    break;

                default:
                    groupBy = new List<string>()
                    {                        
                        "ThirdAccount",
                        "ThirdName",
                        "Reference"
                    };                    
                    break;
            }            
            
            var carteraList = this._dbcontext.SimexCartera
                .Where(c => c.DocumentDate <= filterCartera.ToDate)
                .ToList();

            if (filterCartera.TypeReportId == 2)
            {
                foreach (var cartera in carteraList)
                {
                    var expirationDays = (cartera.PaymentDateEstimated - filterCartera.ToDate).Days;
                    if (expirationDays <= -1 && expirationDays >= -30)
                    {
                        cartera.Range = "1 A 30 DIAS";
                        cartera.Days1To30 = cartera.AmountBalance;
                    }
                    else
                    {
                        if (expirationDays <= -31 && expirationDays >= -60)
                        {
                            cartera.Range = "31 A 60 DIAS";
                            cartera.Days31To60 = cartera.AmountBalance;
                        }
                        else
                        {
                            if (expirationDays <= -61 && expirationDays >= -90)
                            {
                                cartera.Range = "61 A 90 DIAS";
                                cartera.Days61To90 = cartera.AmountBalance;
                            }
                            else
                            {
                                if (expirationDays < -90)
                                {
                                    cartera.Range = "MAS DE 90 DIAS";
                                    cartera.More90 = cartera.AmountBalance;
                                }
                                else
                                {
                                    cartera.Range = "CORRIENTE";
                                    cartera.Current = cartera.AmountBalance;
                                }
                            }
                        }
                    }

                    cartera.Operation2 = string.IsNullOrEmpty(cartera.Operation2) ? "SIN OBRA" : cartera.Operation2;
                    cartera.Operation3 = "";
                    cartera.Operation4 = string.IsNullOrEmpty(cartera.Operation4) ? "SIN ZONA" : cartera.Operation4;
                }
            }

            var lambda = this.GroupByExpression<Cartera>(groupBy.ToArray());

            var filterList = new List<DTOSimexCarteraReport>();
            var _filterList = carteraList.AsQueryable()
                .GroupBy(lambda.Compile())
                .Select(c => this.NewSelect(groupBy, c, filterCartera))
                .ToList();

            switch (filterCartera.FilterById)
            {
                case "Range":
                    foreach (var rangeById in filterCartera.RangeBy)
                    {
                        var rangeList = new List<DTOSimexCarteraReport>();
                        switch (rangeById)
                        {
                            case "All":
                                rangeList.AddRange(_filterList);
                                break;

                            case "1-30":
                                rangeList.AddRange(_filterList.Where(c => (-c.ExpirationDays >= 1 && -c.ExpirationDays <= 30)).ToList());
                                break;

                            case "31-60":
                                rangeList.AddRange(_filterList.Where(c => (-c.ExpirationDays >= 31 && -c.ExpirationDays <= 60)).ToList());
                                break;

                            case "61-90":
                                rangeList.AddRange(_filterList.Where(c => (-c.ExpirationDays >= 61 && -c.ExpirationDays <= 90)).ToList());
                                break;

                            case "+90":
                                rangeList.AddRange(_filterList.Where(c => (-c.ExpirationDays > 90)).ToList());
                                break;

                            case "Current":
                                rangeList.AddRange(_filterList.Where(c => (c.ExpirationDays >= 0)).ToList());
                                break;

                            default:
                                break;
                        }
                        if (rangeList.Count != 0)
                            filterList.AddRange(rangeList);
                    }
                    break;

                case "Customer":
                    if (!string.IsNullOrEmpty(filterCartera.SearchBy))
                    {
                        filterList.AddRange(_filterList.Where(c => (c.GroupBy2.Contains(filterCartera.SearchBy) || c.GroupBy3.Contains(filterCartera.SearchBy))).ToList());
                    }
                    else
                    {
                        filterList.AddRange(_filterList);
                    }
                    break;

                case "Invoice":
                    if (!string.IsNullOrEmpty(filterCartera.SearchBy))
                    {
                        filterList.AddRange(_filterList.Where(c => c.Reference.Contains(filterCartera.SearchBy)).ToList());
                    }
                    else
                    {
                        filterList.AddRange(_filterList);
                    }
                    break;

                case "SalesPerson":
                    if (!string.IsNullOrEmpty(filterCartera.SearchBy))
                    {
                        filterList.AddRange(_filterList.Where(c => c.GroupBy1.Contains(filterCartera.SearchBy)).ToList());
                    }
                    else
                    {
                        filterList.AddRange(_filterList);
                    }
                    break;

                case "Operation2":
                    if (!string.IsNullOrEmpty(filterCartera.SearchBy))
                    {
                        filterList.AddRange(_filterList.Where(c => c.GroupBy4.Contains(filterCartera.SearchBy)).ToList());
                    }
                    else
                    {
                        filterList.AddRange(_filterList);
                    }
                    break;

                case "Operation4":
                    if (!string.IsNullOrEmpty(filterCartera.SearchBy))
                    {
                        filterList.AddRange(_filterList.Where(c => c.GroupBy5.Contains(filterCartera.SearchBy)).ToList());
                    }
                    else
                    {
                        filterList.AddRange(_filterList);
                    }
                    break;
            }

            if(filterCartera.GroupBy.Count() == 1)
            {
                if (filterCartera.GroupBy.Contains("Range"))
                {
                    foreach (var filter in filterList)
                    {
                        switch (filter.GroupBy1)
                        {
                            case "CORRIENTE":
                                filter.Range = "A";
                                break;

                            case "1 A 30 DIAS":
                                filter.Range = "B";
                                break;

                            case "31 A 60 DIAS":
                                filter.Range = "C";
                                break;

                            case "61 A 90 DIAS":
                                filter.Range = "D";
                                break;

                            case "MAS DE 90 DIAS":
                                filter.Range = "E";
                                break;
                        }
                    }

                    return Ok(filterList.OrderBy(f => f.Range).ToList());
                }
            }

            return Ok(filterList.OrderByDescending(f => f.AmountBalance).ToList());
        }

        [HttpGet("GetSalesOrder")]
        public IActionResult GetSalesOrder(string guidfilter)
        {

            DTOSimexSalesOrderFilter filterSalesOrder = salesOrderFilters.Where(x => x.GuidFilter == guidfilter).FirstOrDefault();

            var lambda = this.GroupByExpression<SalesOrder>(filterSalesOrder.GroupBy.ToArray());

            var salesOrderList = new List<SalesOrder>();
            var filterList = new List<DTOSimexSalesOrderReport>();

            switch (filterSalesOrder.TypeReportId)
            {
                case 1:
                    long yearIdi = filterSalesOrder.DateInit.GetValueOrDefault().Year;
                    long yearIde = filterSalesOrder.DateEnd.GetValueOrDefault().Year;
                    long monthIdi = filterSalesOrder.DateInit.GetValueOrDefault().Month;
                    long monthIde = filterSalesOrder.DateEnd.GetValueOrDefault().Month;

                    salesOrderList = this._dbcontext.SimexSalesOrder
                        .Select(s => s)
                        .Where(s => s.DocumentDate >= filterSalesOrder.DateInit && s.DocumentDate <= filterSalesOrder.DateEnd)
                        .ToList();

                    filterList = salesOrderList.AsQueryable()
                        .GroupBy(lambda.Compile())
                        .Select(s => this.NewSelect(filterSalesOrder.GroupBy, s))
                        .ToList();                    

                    break;

                case 2:
                    {
                        foreach (var yearFilter in filterSalesOrder.MultipleValuesYear)
                        {
                            foreach (var month in filterSalesOrder.MultipleValuesMonth)
                            {
                                var initDate = new DateTime((int)yearFilter, (int)month, 1);
                                var endDate = initDate.AddMonths(1).AddDays(-1);
                                salesOrderList = this._dbcontext.SimexSalesOrder
                                    .Select(s => s)
                                    .Where(s => s.DocumentDate >= initDate && s.DocumentDate <= endDate)
                                    .ToList();                                

                                if (salesOrderList.Count() != 0)
                                {
                                    var _filterList = salesOrderList.AsQueryable()
                                    .GroupBy(lambda.Compile())
                                    .Select(s => this.NewSelect(filterSalesOrder.GroupBy, s))
                                    .ToList();

                                    foreach (var fl in _filterList)
                                    {                                        
                                        fl.Year = yearFilter.ToString();
                                        fl.Month = this.GetMonthName(month);
                                        fl.Mth = (int)month;
                                        filterList.Add(fl);
                                    }
                                }                                
                            }
                        }
                    }
                    break;

                case 3:
                    {
                        foreach (var yearFilter in filterSalesOrder.MultipleValuesYear)
                        {
                            var initDate = new DateTime((int)yearFilter, 1, 1);
                            var endDate = new DateTime((int)yearFilter, 12, 31);
                            salesOrderList = this._dbcontext.SimexSalesOrder
                                .Select(s => s)
                                .Where(s => s.DocumentDate >= initDate && s.DocumentDate <= endDate)
                                .ToList();

                            var zones = new List<DTOZone>();

                            if (salesOrderList.Count() != 0)
                            {
                                var _filterList = salesOrderList.AsQueryable()
                                .GroupBy(lambda.Compile())
                                .Select(s => this.NewSelect(filterSalesOrder.GroupBy, s))
                                .ToList();

                                foreach (var fl in _filterList)
                                {
                                    fl.Year = yearFilter.ToString();
                                    filterList.Add(fl);
                                }
                            }                            
                        }
                    }
                    break;

                default:
                    break;
            }

            return Ok(filterList);
        }

        private string GetMonthName(long month)
        {
            switch (month)
            {
                case 1:
                    return "Enero";

                case 2:
                    return "Febrero";

                case 3:
                    return "Marzo";

                case 4:
                    return "Abril";

                case 5:
                    return "Mayo";

                case 6:
                    return "Junio";

                case 7:
                    return "Julio";

                case 8:
                    return "Agosto";

                case 9:
                    return "Septiembre";

                case 10:
                    return "Octubre";

                case 11:
                    return "Noviembre";

                case 12:
                    return "Diciembre";

                default:
                    return "";
            }
        }

        public List<Presupuesto> GetPresupuestos(long yearId, long monthId, string categoryId)
        {
            var list = _dbcontext.SimexPresupuesto
                .Select(s => s)
                .Where(s => s.YearId == yearId && s.MonthId == monthId && s.CategoryId == categoryId)
                .ToList();

            return list;
        }

        public List<QtyMinimum> GetQtyMinimums()
        {
            var list = _dbcontext.SimexQtyMinimum
                .Select(q => q)
                .ToList();

            return list;
        }

        public QtyMinimum GetQtyMinimum(string itemId)
        {
            var qtyMinimum = _dbcontext.SimexQtyMinimum
                .Select(q => q)
                .Where(q => q.ItemId == itemId)
                .FirstOrDefault();

            return qtyMinimum;
        }

        public Presupuesto SavePresupuesto(Presupuesto model)
        {
            var presupuesto = _dbcontext.SimexPresupuesto
                .Select(s => s)
                .Where(s => s.YearId == model.YearId && s.MonthId == model.MonthId && s.CategoryId == model.CategoryId && s.ZoneId == model.ZoneId)
                .FirstOrDefault();

            if (presupuesto is null)
            {
                _dbcontext.SimexPresupuesto.Add(model);
                presupuesto = model;
            }
            else
            {
                presupuesto.Value = model.Value;
                _dbcontext.SimexPresupuesto.Update(presupuesto);                
            }
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();

            return presupuesto;
        }

        public QtyMinimum SaveQtyMinimum(QtyMinimum model)
        {
            var qtyMinimum = _dbcontext.SimexQtyMinimum
                .Select(q => q)
                .Where(q => q.ItemId == model.ItemId)
                .FirstOrDefault();

            if (qtyMinimum is null)
            {
                _dbcontext.SimexQtyMinimum.Add(model);
                qtyMinimum = model;
            }
            else
            {
                qtyMinimum.Qty = model.Qty;
                _dbcontext.SimexQtyMinimum.Update(qtyMinimum);
            }
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();

            return qtyMinimum;
        }

        public bool RemovePresupuesto(Presupuesto model)
        {
            var presupuesto = _dbcontext.SimexPresupuesto
                .Where(s => s.YearId == model.YearId && s.MonthId == model.MonthId && s.ZoneId == model.ZoneId)
                .FirstOrDefault();

            if (presupuesto is null)
            {
                return false;
            }
            else
            {
                _dbcontext.SimexPresupuesto.Remove(presupuesto);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
                return true;
            }
        }

        public bool RemoveQtyMinimum(QtyMinimum model)
        {
            var qtyMinimum = _dbcontext.SimexQtyMinimum
                .Where(q => q.ItemId == model.ItemId)
                .FirstOrDefault();

            if (qtyMinimum is null)
            {
                return false;
            }
            else
            {
                _dbcontext.SimexQtyMinimum.Remove(qtyMinimum);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
                return true;
            }
        }

        public LastUpdateModule GetLastUpdateModule(string module)
        {
            var lastUpdateModule = _dbcontext.SimexLastUpdateModuleLog
                .Where(s => s.Module == module)
                .OrderByDescending(l => l.LastUpdateModule_At)
                .FirstOrDefault();

            return lastUpdateModule;
        }
    }
}
