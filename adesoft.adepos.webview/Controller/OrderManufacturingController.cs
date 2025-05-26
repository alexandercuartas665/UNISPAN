//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using adesoft.adepos.webview.Data;
//using adesoft.adepos.webview.Data.Model;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using adesoft.adepos.Extensions;
//namespace adesoft.adepos.webview.Controller
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class OrderManufacturingController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;
//        private readonly AdeposDBContext _dbcontext;

//        public OrderManufacturingController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
//        {
//            _configuration = configuration;
//            ConnectionDB connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
//            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
//        }

//        public OrderManufacturing Create(OrderManufacturing orderManufacturing)
//        {
//            try
//            {
//                _dbcontext.OrderManufacturings.Add(orderManufacturing);
//                _dbcontext.SaveChanges();
//                _dbcontext.DetachAll();

//                return orderManufacturing;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }


//        public OrderManufacturing Delete(OrderManufacturing orderManufacturing)
//        {
//            _dbcontext.Entry<OrderManufacturing>(orderManufacturing).State = EntityState.Deleted;
//            _dbcontext.SaveChanges();
//            _dbcontext.DetachAll();
//            return orderManufacturing;
//        }

//        public OrderManufacturing Update(OrderManufacturing orderManufacturing)
//        {
//            OrderManufacturing find = _dbcontext.OrderManufacturings.Where(x => x.OrderManufacturingId == orderManufacturing.OrderManufacturingId).FirstOrDefault();
//            if (find != null)
//            {
//                _dbcontext.Entry<OrderManufacturing>(orderManufacturing).State = EntityState.Modified;
//                _dbcontext.SaveChanges();
//                _dbcontext.DetachAll();
//            }
//            else
//            {

//            }
//            return orderManufacturing;
//        }

//        public OrderManufacturing SelectById(OrderManufacturing orderManufacturing)
//        {
//            OrderManufacturing find = _dbcontext.OrderManufacturings.Where(x => x.OrderManufacturingId == orderManufacturing.OrderManufacturingId).FirstOrDefault();

//            return find;
//        }
//        public List<OrderManufacturing> selectAll(OrderManufacturing OrderManufacturing)
//        {
//            if (OrderManufacturing.TransOption == 0 || OrderManufacturing.TransOption == 1)
//            {
//                return _dbcontext.OrderManufacturings.ToList();
//            }
//            else if (OrderManufacturing.TransOption == 2)
//            {
//                List<OrderManufacturing> listorder = _dbcontext.OrderManufacturings
//                    .Include(x => x.Item).Where(x => x.WarehouseId == OrderManufacturing.WarehouseId).ToList();
//                foreach (OrderManufacturing man in listorder)
//                {
//                    man.detailTransaction = _dbcontext.DetailTransactionGenerics.Where(x => x.DetailTransactionGenericId == man.DetailTransactionGenericId).First();
//                    man.transaction = _dbcontext.TransactionGenerics.Where(x => x.TransactionGenericId == man.TransactionGenericId).First();
//                    man.NumOrderDespacho = man.transaction.DocumentExtern;
//                }
//                return listorder;
//            }
//            else
//            {
//                return null;
//            }
//        }


//    }
//}