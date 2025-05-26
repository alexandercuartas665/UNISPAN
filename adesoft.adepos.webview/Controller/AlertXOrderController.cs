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
using adesoft.adepos.webview.Bussines;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertXOrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;

        public AlertXOrderController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        public AlertXOrder Create(AlertXOrder alertXOrder)
        {
            _dbcontext.AlertXOrders.Add(alertXOrder);
            _dbcontext.SaveChanges();
            _dbcontext.DetachAll();
            return alertXOrder;
        }


        public AlertXOrder Update(AlertXOrder alertXOrder)
        {
            AlertXOrder find = _dbcontext.AlertXOrders.Where(x => x.AlertXOrderId == alertXOrder.AlertXOrderId).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<AlertXOrder>(alertXOrder).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return alertXOrder;
        }

        public AlertXOrder SelectById(AlertXOrder alertXOrder)
        {
            AlertXOrder find = _dbcontext.AlertXOrders.Where(x => x.AlertXOrderId == alertXOrder.AlertXOrderId).FirstOrDefault();

            return find;
        }
        public List<AlertXOrder> selectAll(AlertXOrder AlertXOrder)
        {
            AlertXOrderBussines bussi = new AlertXOrderBussines(_dbcontext);
            return _dbcontext.AlertXOrders.ToList();
        }


    }
}