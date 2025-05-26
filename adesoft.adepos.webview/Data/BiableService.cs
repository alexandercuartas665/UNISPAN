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
    public class BiableService
    {
        private readonly IConfiguration _configuration;
        private readonly BiableController _biableController;

        public BiableService(IConfiguration configuration, BiableController biableController)
        {
            _configuration = configuration;
            _biableController = biableController;
        }


        public async Task<List<DTOTercero>> selectAll(DTOTercero report)
        {
            return await Task.FromResult(_biableController.selectAllProveedores(report));
        }
        public async Task<List<DTOYear>> SelectAnosCompras()
        {
            return await Task.FromResult(_biableController.SelectAnosCompras());
        }
        public async Task<List<DTOYear>> SelectAnosMovInventario(int TipoMovId)
        {
            return await Task.FromResult(_biableController.SelectAnosMovInventario(TipoMovId));
        }
        public async Task<List<DTOYear>> SelectAnosContable()
        {
            return await Task.FromResult(_biableController.SelectAnosContable());
        }

        public async Task<List<DTOYear>> BuildAnosParameter()
        {
            return await Task.FromResult(_biableController.BuildAnosParameter());
        }
        public async Task AddFilterCompras(DTOFiltersCompras data)
        {
            await Task.Run(new Action(() => { _biableController.AddFilterCompras(data); }));
        }

    }
}
