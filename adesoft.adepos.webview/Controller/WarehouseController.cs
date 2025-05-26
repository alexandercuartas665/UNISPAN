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
    public class WarehouseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        ConnectionDB connectionDB;
        public WarehouseController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._dbcontext = new AdeposDBContext(connectionDB.Connection);
        }


        public Warehouse Create(Warehouse warehouse)
        {
            Warehouse find = _dbcontext.Warehouses.Where(x => x.Name == warehouse.Name).FirstOrDefault();
            if (find == null)
            {
                _dbcontext.Warehouses.Add(warehouse);
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return warehouse;
        }


        public Warehouse Update(Warehouse warehouse)
        {
            Warehouse find = _dbcontext.Warehouses.Where(x => x.WarehouseId == warehouse.WarehouseId).FirstOrDefault();
            if (find != null)
            {
                _dbcontext.Entry<Warehouse>(warehouse).State = EntityState.Modified;
                _dbcontext.SaveChanges();
                _dbcontext.DetachAll();
            }
            else
            {

            }
            return warehouse;
        }

        public Warehouse SelectById(Warehouse warehouse)
        {
            Warehouse find = _dbcontext.Warehouses.Where(x => x.WarehouseId == warehouse.WarehouseId).FirstOrDefault();

            return find;
        }

        public async Task SincronizarBodegas()
        {
            ReadDocumentsOfPath read = new ReadDocumentsOfPath(_configuration, connectionDB);
            await read.ReadInventoryStockOfQuantify(true);
        }
        public List<Warehouse> selectAll(Warehouse Warehouse)
        {
            if (Warehouse.TransOption == 0 || Warehouse.TransOption == 1)
            {
                return _dbcontext.Warehouses.ToList();
            }
            else if (Warehouse.TransOption == 2)
            {
                return _dbcontext.Warehouses.Where(x => x.WarehouseId != 1).ToList();
            }
            else
            {
                return _dbcontext.Warehouses.ToList();
            }
        }


    }
}