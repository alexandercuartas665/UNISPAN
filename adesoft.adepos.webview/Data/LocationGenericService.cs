using adesoft.adepos.webview.Controller;
using adesoft.adepos.webview.Data.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class LocationGenericService
    {
        private readonly IConfiguration _configuration;
        private readonly LocationGenericController _locationGenericController;

        public LocationGenericService(IConfiguration configuration, LocationGenericController locationGenericController)
        {
            _configuration = configuration;
            _locationGenericController = locationGenericController;
        }

        public async Task<List<LocationGeneric>> selectAll(LocationGeneric locationGeneric)
        {
            return await Task.FromResult(_locationGenericController.selectAll(locationGeneric));
        }

        public async Task<LocationGeneric> Create(LocationGeneric model)
        {
            return await Task.FromResult(_locationGenericController.Create(model));
        }

        public async Task<LocationGeneric> SaveLocation(LocationGeneric model)
        {
            return await Task.FromResult(_locationGenericController.Create(model));
        }

        public async Task<LocationGeneric> Update(LocationGeneric model)
        {
            return await Task.FromResult(_locationGenericController.Update(model));
        }
        public async Task<LocationGeneric> SelectById(LocationGeneric model)
        {
            return await Task.FromResult(_locationGenericController.SelectById(model));
        }

        public async Task<List<LocationGeneric>> GetLocations(string typeLocationId)
        {
            return await Task.FromResult(_locationGenericController.GetLocations(typeLocationId));
        }
    }
}
