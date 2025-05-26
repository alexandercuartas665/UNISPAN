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
    public class OportunidadesCRMService
    {
        private readonly IConfiguration _configuration;
        private readonly OportunidadesCRMController _OportunidadesController;
        public OportunidadesCRMService(IConfiguration configuration, OportunidadesCRMController controller)
        {
            _configuration = configuration;
            _OportunidadesController = controller;
        }


        public async Task<List<OportunidadesCRM>> selectAll(OportunidadesCRM oportun)
        {
            return await Task.FromResult(_OportunidadesController.selectAll(oportun));
        }

        public async Task<OportunidadesCRM> SelectById(OportunidadesCRM oportun)
        {
            return await Task.FromResult(_OportunidadesController.selectById(oportun));
        }

        public async Task<OportunidadesCRM> Create(OportunidadesCRM model)
        {
            return await Task.FromResult(_OportunidadesController.Create(model));
        }

        public async Task<OportunidadesCRM> Update(OportunidadesCRM model)
        {
            return await Task.FromResult(_OportunidadesController.Update(model));
        }
        public async Task AddFilterCompras(DTOFiltersCompras data)
        {
            await Task.Run(new Action(() => { _OportunidadesController.AddFilterCompras(data); }));
        }

        public async Task StartSyncCRM()
        {
            await Task.Run(new Action(() => { _OportunidadesController.StartSyncCRM(); }));
        }
    }
}
