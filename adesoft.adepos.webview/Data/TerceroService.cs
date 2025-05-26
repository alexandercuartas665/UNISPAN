using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class TerceroService
    {
        private readonly IConfiguration _configuration;
        private readonly TerceroController _terceroController;

        public TerceroService(IConfiguration configuration, TerceroController terceroController)
        {
            _configuration = configuration;
            _terceroController = terceroController;
        }

        public async Task<List<Tercero>> selectAll(Tercero roleapp)
        {

            return await Task.FromResult(_terceroController.selectAll(roleapp));
        }

        public async Task<List<Tercero>> GetEmpleados()
        {

            return await Task.FromResult(_terceroController.GetEmpleados());
        }

        public async Task<Tercero> Create(Tercero model)
        {
            return await Task.FromResult(_terceroController.Create(model));
        }

        public async Task<Tercero> Update(Tercero model)
        {
            return await Task.FromResult(_terceroController.Update(model));
        }
        public async Task<Tercero> SelectById(Tercero model)
        {
            return await Task.FromResult(_terceroController.SelectById(model));
        }

        public async Task<Tercero> SelectById2(Tercero model)
        {
            return await Task.FromResult(_terceroController.SelectById2(model));
        }

        public async Task<List<TypeTercero>> selectAllTypeTercero(TypeTercero typetercero)
        {
            return await Task.FromResult(_terceroController.selectAllTypeTercero(typetercero));
        }

    }
}
