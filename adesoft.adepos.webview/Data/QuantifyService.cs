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
    public class QuantifyService
    {
        private readonly IConfiguration _configuration;
        private readonly QuantifyController _QuantifyController;

        public QuantifyService(IConfiguration configuration, QuantifyController QuantifyController)
        {
            _configuration = configuration;
            _QuantifyController = QuantifyController;
        }


        public async Task<List<DTOYear>> SelectAnosMovEquiposAlquiler()
        {
            return await Task.FromResult(_QuantifyController.SelectAnosMovEquiposAlquiler());
        }

        public async Task AddFilterCompras(DTOFiltersCompras data)
        {
            await Task.Run(new Action(() => { _QuantifyController.AddFilterCompras(data); }));
        }

    }
}
