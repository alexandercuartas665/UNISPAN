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
    public class TransactionGenericService
    {
        private readonly IConfiguration _configuration;
        private readonly TransactionGenericController _categoryController;

        public TransactionGenericService(IConfiguration configuration, TransactionGenericController categoryController)
        {
            _configuration = configuration;
            _categoryController = categoryController;
        }

        public async Task<List<DetailTransactionGeneric>> AgregarItems(List<DetailTransactionGeneric> listdetail)
        {
            return await Task.FromResult(_categoryController.AgregarItems(listdetail));
        }

        public async Task<DetailTransactionGeneric> Update(DetailTransactionGeneric detail)
        {
            return await Task.FromResult(_categoryController.Update(detail));
        }


        public async Task<List<TransactionGeneric>> selectAll(TransactionGeneric category)
        {
            return await Task.FromResult(_categoryController.selectAll(category));
        }

        public async Task<TransactionGeneric> Create(TransactionGeneric model)
        {
            return await Task.FromResult(_categoryController.Create(model));
        }

        public async Task<TransactionGeneric> ImportFile(TransactionGeneric model)
        {
            return await Task.FromResult(_categoryController.ImportFile(model));
        }

        public async Task<TransactionGeneric> Update(TransactionGeneric model)
        {
            return await Task.FromResult(_categoryController.Update(model));
        }

        public async Task<TransactionGeneric> Delete(TransactionGeneric model)
        {
            return await Task.FromResult(_categoryController.Delete(model));
        }

        public async Task<List<DTOCardDistpatch>> Update(List<DTOCardDistpatch> transactions, long option)
        {
            return await Task.FromResult(_categoryController.Update(transactions, option));
        }


        public async Task<TransactionGeneric> SelectById(TransactionGeneric model)
        {
            return await Task.FromResult(_categoryController.SelectById(model));
        }

        public async Task AddFilterCompras(DTOFiltersCompras data)
        {
            await Task.Run(new Action(() => { _categoryController.AddFilterCompras(data); }));
        }
    }
}
