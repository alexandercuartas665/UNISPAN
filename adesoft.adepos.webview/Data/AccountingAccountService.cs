using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class AccountingAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly AccountingAccountController _accountingAccountController;

        public AccountingAccountService(IConfiguration configuration, AccountingAccountController accountingAccountController)
        {
            _configuration = configuration;
            _accountingAccountController = accountingAccountController;
        }

        public async Task<List<AccountingAccount>> selectAll(AccountingAccount accountingAccount)
        {
            return await Task.FromResult(_accountingAccountController.selectAll(accountingAccount));
        }

        public async Task<AccountingAccount> Create(AccountingAccount model)
        {
            return await Task.FromResult(_accountingAccountController.Create(model));
        }

        public async Task<AccountingAccount> Update(AccountingAccount model)
        {
            return await Task.FromResult(_accountingAccountController.Update(model));
        }
        public async Task<AccountingAccount> SelectById(AccountingAccount model)
        {
            return await Task.FromResult(_accountingAccountController.SelectById(model));
        }
    }
}
