using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.DTO.Simex;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.Simex;
using ExcelDataReader;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class SimexService
    {
        private readonly SimexController _simexController;

        public SimexService(SimexController simexController)
        {
            _simexController = simexController;
        }        

        public async Task UploadSalesAsync(Parameter model)
        {
            await Task.FromResult(_simexController.UploadSales(model));            
        }

        public async Task UploadCarteraAsync(Parameter model)
        {
            await Task.FromResult(_simexController.UploadCartera(model));
        }

        public async Task UploadInventSumAsync(Parameter model)
        {
            await Task.FromResult(_simexController.UploadInventSum(model));
        }

        public async Task UploadSalesOrderAsync(Parameter model)
        {
            await Task.FromResult(_simexController.UploadSalesOrder(model));
        }

        public async Task UploadSalesOrderPriceAsync(Parameter model)
        {
            await Task.FromResult(_simexController.UploadSalesOrderPrice(model));
        }

        public async Task<List<DTOYear>> GetYears(string reportName)
        {
            return await Task.FromResult(_simexController.GetYears(reportName));
        }

        public async Task<List<DTOZone>> GetZones()
        {
            return await Task.FromResult(_simexController.GetZones());
        }

        public async Task AddSalesFilter(DTOSimexSalesFilter salesFilter)
        {
            await Task.Run(new Action(() => { _simexController.AddSalesFilter(salesFilter); }));
        }        

        public async Task AddInventSumFilter(DTOSimexInventSumFilter inventSumFilter)
        {
            await Task.Run(new Action(() => { _simexController.AddInventSumFilter(inventSumFilter); }));
        }

        public async Task AddCarteraFilter(DTOSimexCarteraFilter carteraFilter)
        {
            await Task.Run(new Action(() => { _simexController.AddCarteraFilter(carteraFilter); }));
        }

        public async Task AddSalesOrderFilter(DTOSimexSalesOrderFilter salesOrderFilter)
        {
            await Task.Run(new Action(() => { _simexController.AddSalesOrderFilter(salesOrderFilter); }));
        }

        public async Task<List<Presupuesto>> GetPresupuestos(long yearId, long monthId, string categoryId)
        {
            return await Task.FromResult(_simexController.GetPresupuestos(yearId, monthId, categoryId));
        }

        public async Task<Presupuesto> SavePresupuesto(Presupuesto model)
        {
            return await Task.FromResult(_simexController.SavePresupuesto(model));
        }

        public async Task<QtyMinimum> SaveQtyMinimum(QtyMinimum model)
        {
            return await Task.FromResult(_simexController.SaveQtyMinimum(model));
        }

        public async Task<bool> RemovePresupuesto(Presupuesto model)
        {
            return await Task.FromResult(_simexController.RemovePresupuesto(model));
        }

        public async Task<bool> RemoveQtyMimimum(QtyMinimum model)
        {
            return await Task.FromResult(_simexController.RemoveQtyMinimum(model));
        }

        public async Task<LastUpdateModule> GetLastUpdateModule(string module)
        {
            return await Task.FromResult(_simexController.GetLastUpdateModule(module));
        }

        public async Task<List<QtyMinimum>> GetQtyMinimums()
        {
            return await Task.FromResult(_simexController.GetQtyMinimums());
        }
    }
}
