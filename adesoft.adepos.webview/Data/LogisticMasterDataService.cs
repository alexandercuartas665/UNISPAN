using adesoft.adepos.Extensions;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Interfaces;
using adesoft.adepos.webview.Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class LogisticMasterDataService : ILogisticMasterDataService
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposDBContext _dbcontext;
        private readonly ConnectionDB _connectionDB;

        public LogisticMasterDataService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (_connectionDB == null)
                _connectionDB = SecurityService.GetConnectionDefault();
            _dbcontext = new AdeposDBContext(_connectionDB.Connection);
        }

        public string GetCustomerName(string identificationNum)
        {
            try
            {
                var customer = _dbcontext.LogisticMasterData
                    .Where(c => c.IdentificationNum == identificationNum
                    && c.Type.Equals("CLIENTE"))                   
                    .FirstOrDefault();

                return customer != null ? customer.Description : "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetModuleName(int moduleId)
        {
            try
            {
                var module = _dbcontext.LogisticMasterData
                    .Where(c => c.Id == moduleId
                    && c.Type.Equals("MODULADOR"))
                    .FirstOrDefault();

                return module != null ? module.Description : "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetCityName(int moduleId)
        {
            try
            {
                var city = _dbcontext.LogisticMasterData
                    .Where(c => c.Id == moduleId
                    && c.Type.Equals("CIUDAD"))
                    .FirstOrDefault();

                return city != null ? city.Description : "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DTOLogisticMasterData> GetLogisticMasterDatas(string type)
        {
            throw new NotImplementedException();
        }
    }
}
