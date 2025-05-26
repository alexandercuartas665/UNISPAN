using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using adesoft.adepos.Extensions;
namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountingAccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;

        public AccountingAccountController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public AccountingAccount Create(AccountingAccount accountingAccount)
        {

            _dbcontext.AccountingAccounts.Add(accountingAccount);
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();
            return accountingAccount;
        }


        public AccountingAccount Update(AccountingAccount accountingAccount)
        {
            AccountingAccount find = _dbcontext.AccountingAccounts.Where(x => x.AccountingAccountId == accountingAccount.AccountingAccountId).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<AccountingAccount>(accountingAccount).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return accountingAccount;
        }

        public AccountingAccount SelectById(AccountingAccount accountingAccount)
        {
            AccountingAccount find = _dbcontext.AccountingAccounts.Where(x => x.AccountingAccountId == accountingAccount.AccountingAccountId).FirstOrDefault();

            return find;
        }
        public List<AccountingAccount> selectAll(AccountingAccount AccountingAccount)
        {
            return _dbcontext.AccountingAccounts.ToList();
        }


    }
}