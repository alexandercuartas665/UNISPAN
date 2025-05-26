using adesoft.adepos.webview.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Interfaces
{
    public interface ILogisticMasterDataService
    {
        public List<DTOLogisticMasterData> GetLogisticMasterDatas(string type);

        public string GetCustomerName(string identificationNum);

        public string GetModuleName(int moduleId);

        public string GetCityName(int moduleId);
    }
}
