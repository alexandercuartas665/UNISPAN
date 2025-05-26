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
    public class NominaService
    {
        private readonly IConfiguration _configuration;
        private readonly NominaController _nominaController;

        public NominaService(IConfiguration configuration, NominaController nominaController)
        {
            _configuration = configuration;
            _nominaController = nominaController;
        }
        #region NominaProgramation
        public async Task<List<NominaProgramation>> selectAll(NominaProgramation nomina)
        {
            return await Task.FromResult(_nominaController.selectAll(nomina));
        }

        public async Task<NominaProgramation> Create(NominaProgramation model)
        {
            return await Task.FromResult(_nominaController.Create(model));
        }

        public async Task<NominaProgramation> Update(NominaProgramation model)
        {
            return await Task.FromResult(_nominaController.Update(model));
        }
        public async Task<NominaProgramation> SelectById(NominaProgramation model)
        {
            return await Task.FromResult(_nominaController.SelectById(model));
        }
        #endregion
        #region NominaNovedad
        public async Task<List<NominaNovedad>> selectAll(NominaNovedad nomina)
        {
            return await Task.FromResult(_nominaController.selectAll(nomina));
        }
        public async Task<NominaProgramation> GeneratePlanos(NominaProgramation model)
        {
            return await Task.FromResult(_nominaController.GeneratePlanos(model));
        }
        public async Task<NominaNovedad> Create(NominaNovedad model)
        {
            return await Task.FromResult(_nominaController.Create(model));
        }

        public async Task<NominaNovedad> Update(NominaNovedad model)
        {
            return await Task.FromResult(_nominaController.Update(model));
        }
        public async Task<NominaNovedad> SelectById(NominaNovedad model)
        {
            return await Task.FromResult(_nominaController.SelectById(model));
        }
        #endregion

        #region CodeNovedad
        public async Task<List<CodeNovedad>> selectAll(CodeNovedad nomina)
        {
            return await Task.FromResult(_nominaController.selectAll(nomina));
        }

        #endregion

        public async Task AddEmployFilter(DTOHREmployFilter employFilter)
        {
            await Task.Run(new Action(() => { _nominaController.AddEmployFilter(employFilter); }));
        }

        public async Task<bool> UploadEmployesAsync(Parameter parameter)
        {
            return await Task.FromResult(_nominaController.UploadEmployes(parameter));
        }

        public async Task<bool> UploadExternalEmployesAsync(Parameter parameter)
        {
            return await Task.FromResult(_nominaController.UploadExternalEmployes(parameter));
        }

        public async Task<bool> UploadNewnessAsync(Parameter parameter)
        {
            return await Task.FromResult(_nominaController.UploadNewness(parameter));
        }

        public async Task<List<LocationGeneric>> GetLocations(string typeLocation)
        {
            return await Task.FromResult(_nominaController.GetLocations(typeLocation));
        }
        
        //public async Task<List<DTOChartData>> GetChartData(DateTime fromDate, DateTime toDate, string valueType)
        //{
        //    return await Task.FromResult(_nominaController.GetChartData(fromDate, toDate, valueType));
        //}
    }
}
