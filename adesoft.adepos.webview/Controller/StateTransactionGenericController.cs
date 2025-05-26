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
    public class StateTransactionGenericController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;

        public StateTransactionGenericController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public StateTransactionGeneric Create(StateTransactionGeneric statetransactiongeneric)
        {
         
            _dbcontext.StateTransactionGenerics.Add(statetransactiongeneric);
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();
            return statetransactiongeneric;
        }


        public StateTransactionGeneric Update(StateTransactionGeneric statetransactiongeneric)
        {
            StateTransactionGeneric find = _dbcontext.StateTransactionGenerics.Where(x => x.StateTransactionGenericId == statetransactiongeneric.StateTransactionGenericId).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<StateTransactionGeneric>(statetransactiongeneric).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return statetransactiongeneric;
        }

        public StateTransactionGeneric SelectById(StateTransactionGeneric statetransactiongeneric)
        {
            StateTransactionGeneric find = _dbcontext.StateTransactionGenerics.Where(x => x.StateTransactionGenericId == statetransactiongeneric.StateTransactionGenericId).FirstOrDefault();

            return find;
        }
        public List<StateTransactionGeneric> selectAll(StateTransactionGeneric StateTransactionGeneric)
        {
            if (StateTransactionGeneric.TransOption == 1)
            {
                return _dbcontext.StateTransactionGenerics.Where(x => x.TypeTransactionId == StateTransactionGeneric.TypeTransactionId).ToList();
            }
            else
            {
                return _dbcontext.StateTransactionGenerics.ToList();
            }

        }


    }
}