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
    public class ControlStateTransactionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;

        public ControlStateTransactionController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public ControlStateTransaction Create(ControlStateTransaction controlStateTransaction)
        {
            _dbcontext.ControlStateTransactions.Add(controlStateTransaction);
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();
            return controlStateTransaction;
        }


        public ControlStateTransaction Update(ControlStateTransaction controlStateTransaction)
        {
            ControlStateTransaction find = _dbcontext.ControlStateTransactions.Where(x => x.ControlStateTransactionId == controlStateTransaction.ControlStateTransactionId).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<ControlStateTransaction>(controlStateTransaction).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return controlStateTransaction;
        }

        public ControlStateTransaction SelectById(ControlStateTransaction controlStateTransaction)
        {
            ControlStateTransaction find = _dbcontext.ControlStateTransactions.Where(x => x.ControlStateTransactionId == controlStateTransaction.ControlStateTransactionId).FirstOrDefault();

            return find;
        }
        public List<ControlStateTransaction> selectAll(ControlStateTransaction ControlStateTransaction)
        {
            if (ControlStateTransaction.TransOption == 1)
            {
                return _dbcontext.ControlStateTransactions.Where(x => x.StateTransactionId == ControlStateTransaction.StateTransactionId).ToList();
            }
            else
            {
                return _dbcontext.ControlStateTransactions.ToList();
            }

        }


    }
}